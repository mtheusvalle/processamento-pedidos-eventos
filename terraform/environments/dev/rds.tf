# Configuração do Grupo de Sub-redes para o RDS (Usando as sub-redes privadas)
resource "aws_db_subnet_group" "rds_subnet_group" {
  name       = "sng-processamento-pedidos"
  subnet_ids = [aws_subnet.private_1a.id, aws_subnet.private_1b.id]

  tags = {
    Name = "Subnet group para RDS"
  }
}

# Security Group para o RDS
resource "aws_security_group" "rds_sg" {
  name        = "sg_rds"
  description = "Acesso ao Postgres apenas do ECS"
  vpc_id      = aws_vpc.main.id

  # Libera a porta do PostgreSQL apenas para containers que tenham o SG do ECS
  ingress {
    from_port       = 5432
    to_port         = 5432
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
    Name = "sg-postgres"
  }
}

# Instância RDS Postgres consolidada (Vai conter o checkout_db e payment_db)
resource "aws_db_instance" "postgres" {
  identifier             = "db-processamento-pedidos"
  engine                 = "postgres"
  engine_version         = "15.4" # Versão equivalente ao docker-compose (15)
  instance_class         = "db.t3.micro"
  allocated_storage      = 20
  storage_type           = "gp2"
  
  # Credenciais (Em produção, o ideal é injetar de um SecretsManager)
  username               = "postgres_admin"
  password               = "Dev123Password!" # O Terraform marcará isso como sensível
  
  # Não criar um banco de dados default além do 'postgres' default; o script da aplicação deve criar.
  db_name                = "postgres"
  
  vpc_security_group_ids = [aws_security_group.rds_sg.id]
  db_subnet_group_name   = aws_db_subnet_group.rds_subnet_group.name

  publicly_accessible    = false
  skip_final_snapshot    = true # Apenas para ambiente dev, exclui sem perguntar
  multi_az               = false

  tags = {
    Name = "rds-postgres-pedidos"
  }
}
