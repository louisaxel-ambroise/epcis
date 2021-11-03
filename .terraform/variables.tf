variable "subscription_id" {
  type      = string
  sensitive = true
}

variable "environment" {
  type    = string
  default = "dev"
}

variable "aspnetcore_environment" {
  type    = string
  default = "production"
}

variable "location" {
  type    = string
  default = "westeurope"
}

variable "database_username" {
  type        = string
  description = "Administrator username for the database server"
  sensitive   = true
}

variable "database_password" {
  type        = string
  description = "Administrator password for the database server"
  sensitive   = true
}

variable "app_tier" {
  type = string
  description = "The tier for the app service plan. Defaults to Basic"
  default = "Basic"
}

variable "app_size" {
  type = string
  description = "The size for the app service plan. Defaults to B1"
  default = "B1"
}

variable "enable_cliet_cert" {
  type = bool
  default = true
}

variable "sql_max_size_gb" {
  type        = string
  description = "Max size of the database in GB. Defaults to 2."
  default     = "2"
}

variable "sql_sku" {
  type        = string
  description = "SKU for the database. Defaults to Basic."
  default     = "Basic"
}