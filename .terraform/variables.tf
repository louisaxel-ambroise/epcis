variable "subscription_id" {
  type      = string
  sensitive = true
}

variable "environment" {
  type    = string
  default = "fastnt-dev"
}

variable "aspnetcore_environment" {
  type    = string
  default = "development"
}

variable "location" {
  type    = string
  default = "westeurope"
}

variable "app_size" {
  type = string
  description = "The size for the app service plan. Defaults to F1"
  default = "F1"
}