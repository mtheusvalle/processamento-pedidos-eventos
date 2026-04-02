# Cluster ECS
resource "aws_ecs_cluster" "main" {
  name = "cluster-processamento-pedidos"
}

# SG do Application Load Balancer
resource "aws_security_group" "alb_sg" {
  name        = "sg_alb"
  description = "Acesso HTTP da internet para o ALB"
  vpc_id      = aws_vpc.main.id

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "sg-alb-pedidos"
  }
}

# SG das Tasks do ECS
resource "aws_security_group" "ecs_tasks_sg" {
  name        = "sg_ecs_tasks"
  description = "Acesso entre o ALB e o ECS"
  vpc_id      = aws_vpc.main.id

  # Libera acesso vindo do Load Balancer
  ingress {
    from_port       = 8080
    to_port         = 8080
    protocol        = "tcp"
    security_groups = [aws_security_group.alb_sg.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "sg-ecs-tasks"
  }
}

# Application Load Balancer (Fica na rede pública)
resource "aws_lb" "checkout_alb" {
  name               = "alb-checkout-api"
  internal           = false
  load_balancer_type = "application"
  security_groups    = [aws_security_group.alb_sg.id]
  subnets            = [aws_subnet.public_1a.id, aws_subnet.public_1b.id]

  enable_deletion_protection = false
}

# Target Group que roteia pro Checkout API (Fargate) na 8080
resource "aws_lb_target_group" "checkout_tg" {
  name        = "tg-checkout-api"
  port        = 8080
  protocol    = "HTTP"
  vpc_id      = aws_vpc.main.id
  target_type = "ip"

  health_check {
    healthy_threshold   = 3
    unhealthy_threshold = 3
    timeout             = 3
    path                = "/swagger/v1/swagger.json" # Endpoint para teste
    interval            = 30
  }
}

# Listener do Load Balancer
resource "aws_lb_listener" "http" {
  load_balancer_arn = aws_lb.checkout_alb.arn
  port              = "80"
  protocol          = "HTTP"

  default_action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.checkout_tg.arn
  }
}

# -----------------
# Task e Serviço API
# -----------------
resource "aws_ecs_task_definition" "checkout_api" {
  family                   = "checkout-api-task"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

  container_definitions = jsonencode([
    {
      name  = "checkout-api"
      image = "${aws_ecr_repository.checkout_api.repository_url}:latest"
      portMappings = [
        {
          containerPort = 8080
          hostPort      = 8080
          protocol      = "tcp"
        }
      ]
      environment = [
        { name = "ASPNETCORE_ENVIRONMENT", value = "Development" },
        { name = "RabbitMQ__HostName", value = replace(aws_mq_broker.rabbitmq.instances[0].endpoints[0], "amqps://", "") },
        { name = "RabbitMQ__UserName", value = "rabbit_admin" },
        { name = "RabbitMQ__Password", value = "RabbitAdmin123!" },
        { name = "ConnectionStrings__CheckoutDb", value = "Host=${aws_db_instance.postgres.address};Database=checkout_db;Username=postgres_admin;Password=Dev123Password!" }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          "awslogs-group"         = "/ecs/checkout-api"
          "awslogs-region"        = var.aws_region
          "awslogs-stream-prefix" = "ecs"
        }
      }
    }
  ])
}

resource "aws_ecs_service" "checkout_api" {
  name            = "svc-checkout-api"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.checkout_api.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = [aws_subnet.private_1a.id, aws_subnet.private_1b.id]
    security_groups  = [aws_security_group.ecs_tasks_sg.id]
    assign_public_ip = false
  }

  load_balancer {
    target_group_arn = aws_lb_target_group.checkout_tg.arn
    container_name   = "checkout-api"
    container_port   = 8080
  }
}

# --------------------
# Task e Serviço Worker
# --------------------
resource "aws_ecs_task_definition" "payment_worker" {
  family                   = "payment-worker-task"
  network_mode             = "awsvpc"
  requires_compatibilities = ["FARGATE"]
  cpu                      = 256
  memory                   = 512
  execution_role_arn       = aws_iam_role.ecs_task_execution_role.arn
  task_role_arn            = aws_iam_role.ecs_task_role.arn

  container_definitions = jsonencode([
    {
      name  = "payment-worker"
      image = "${aws_ecr_repository.payment_worker.repository_url}:latest"
      environment = [
        { name = "RabbitMQ__HostName", value = replace(aws_mq_broker.rabbitmq.instances[0].endpoints[0], "amqps://", "") },
        { name = "RabbitMQ__UserName", value = "rabbit_admin" },
        { name = "RabbitMQ__Password", value = "RabbitAdmin123!" },
        { name = "ConnectionStrings__PaymentDb", value = "Host=${aws_db_instance.postgres.address};Database=payment_db;Username=postgres_admin;Password=Dev123Password!" }
      ]
      logConfiguration = {
        logDriver = "awslogs"
        options = {
          "awslogs-group"         = "/ecs/payment-worker"
          "awslogs-region"        = var.aws_region
          "awslogs-stream-prefix" = "ecs"
        }
      }
    }
  ])
}

resource "aws_ecs_service" "payment_worker" {
  name            = "svc-payment-worker"
  cluster         = aws_ecs_cluster.main.id
  task_definition = aws_ecs_task_definition.payment_worker.arn
  desired_count   = 1
  launch_type     = "FARGATE"

  network_configuration {
    subnets          = [aws_subnet.private_1a.id, aws_subnet.private_1b.id]
    security_groups  = [aws_security_group.ecs_tasks_sg.id]
    assign_public_ip = false
  }
}

# Criação dos grupos de log que mandamos no task definition
resource "aws_cloudwatch_log_group" "checkout_api" {
  name              = "/ecs/checkout-api"
  retention_in_days = 7
}

resource "aws_cloudwatch_log_group" "payment_worker" {
  name              = "/ecs/payment-worker"
  retention_in_days = 7
}
