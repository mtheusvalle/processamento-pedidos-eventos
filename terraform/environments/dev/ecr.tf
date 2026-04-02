# Criação dos repositórios ECR para armazenar as imagens Docker da aplicação

resource "aws_ecr_repository" "checkout_api" {
  name                 = "processamento-pedidos/checkout-api"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "ecr-checkout-api"
  }
}

resource "aws_ecr_repository" "payment_worker" {
  name                 = "processamento-pedidos/payment-worker"
  image_tag_mutability = "MUTABLE"

  image_scanning_configuration {
    scan_on_push = true
  }

  tags = {
    Name = "ecr-payment-worker"
  }
}
