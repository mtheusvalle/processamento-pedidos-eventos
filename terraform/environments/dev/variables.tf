variable "aws_region" {
  description = "A região da AWS onde os recursos serão criados"
  type        = string
  default     = "us-east-1"
}

variable "vpc_cidr" {
  description = "O bloco CIDR (Range de IPs) da VPC principal"
  type        = string
  default     = "10.0.0.0/16" 
}
