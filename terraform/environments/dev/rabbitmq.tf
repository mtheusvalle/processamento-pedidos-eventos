# Security Group para o Amazon MQ (RabbitMQ)
resource "aws_security_group" "rabbitmq_sg" {
  name        = "sg_rabbitmq"
  description = "Acesso ao RabbitMQ apenas do ECS"
  vpc_id      = aws_vpc.main.id

  # AMQP porta 5671 (AMQPS Seguro)
  ingress {
    from_port       = 5671
    to_port         = 5671
    protocol        = "tcp"
    security_groups = [aws_security_group.ecs_tasks_sg.id]
  }

  ingress {
    from_port       = 443
    to_port         = 443
    protocol        = "tcp"
    security_groups = [aws_security_group.ecs_tasks_sg.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }

  tags = {
    Name = "sg-rabbitmq"
  }
}

# Amazon MQ - Broker Gerenciado para RabbitMQ
resource "aws_mq_broker" "rabbitmq" {
  broker_name = "rabbitmq-processamento-pedidos"

  engine_type        = "RabbitMQ"
  engine_version     = "3.11.20"
  host_instance_type = "mq.m5.large" # t3.micro RabbitMQ só permite single-instance, m5 suporta dependendo da região, porém vamos usar t3.micro para dev

  # Security e Redes
  subnet_ids         = [aws_subnet.private_1a.id]
  security_groups    = [aws_security_group.rabbitmq_sg.id]
  publicly_accessible = false

  user {
    username = "rabbit_admin"
    password = "RabbitAdmin123!" # Deve ter 12 digitos
  }

  tags = {
    Name = "mq-rabbitmq-pedidos"
  }
}
