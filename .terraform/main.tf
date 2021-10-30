terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">= 2.26"
    }
  }
  backend "azurerm" {
    storage_account_name = ""
    access_key           = ""
    container_name       = "tfstate"
    key                  = "terraform.tfstate"
  }

  required_version = ">= 1.0.0"
}

provider "azurerm" {
  features {}

  subscription_id = var.subscription_id
}

locals {
  tags = {
    Environment = upper(var.environment)
  }
  base_resource_name = lower("fastnt-${var.environment}")
}

resource "azurerm_resource_group" "fastnt_main" {
  name     = local.base_resource_name
  location = var.location
  tags     = local.tags
}

resource "azurerm_mssql_server" "sql-db-server" {
  name                         = "${local.base_resource_name}-sql-server"
  resource_group_name          = azurerm_resource_group.fastnt_main.name
  location                     = var.location
  version                      = "12.0"
  administrator_login          = var.database_username
  administrator_login_password = var.database_password
  minimum_tls_version          = "1.2"
  tags                         = local.tags
}

resource "azurerm_mssql_database" "sqldb" {
  name                        = "${local.base_resource_name}-sql-db"
  server_id                   = azurerm_mssql_server.sql-db-server.id
  collation                   = "SQL_Latin1_General_CP1_CI_AS"
  max_size_gb                 = var.sql_max_size_gb
  sku_name                    = var.sql_sku
  tags                        = local.tags
}

resource "azurerm_mssql_firewall_rule" "sql-db-server-allow-azure" {
  name = "AllowAccessToAzureServices"
  server_id = azurerm_mssql_server.sql-db-server.id
  start_ip_address = "0.0.0.0"
  end_ip_address = "0.0.0.0"
}

resource "azurerm_app_service_plan" "plan" {
  name                = "${local.base_resource_name}-app-plan"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  sku {
    tier = var.app_tier
    size = var.app_size
  }
  tags = local.tags
}

resource "azurerm_application_insights" "api_appi" {
  name                = "${local.base_resource_name}-api-appinsights"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  application_type    = "web"
  tags                = local.tags
}

resource "azurerm_app_service" "api_app" {
  name                = "${local.base_resource_name}-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  app_service_plan_id = azurerm_app_service_plan.plan.id
  tags                = local.tags
  app_settings = {
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.api_appi.instrumentation_key}"
    "APPLICATIONINSIGHTS_CONNECTION_STRING" : "${azurerm_application_insights.api_appi.connection_string}"
    "ApplicationInsightsAgent_EXTENSION_VERSION" : "~2"
    "ConnectionStrings:FasTnT.Database" : "Server=tcp:${azurerm_mssql_server.sql-db-server.fully_qualified_domain_name},1433;Initial Catalog=${local.base_resource_name}-sql-db;Persist Security Info=False;User ID=${azurerm_mssql_server.sql-db-server.administrator_login};Password=${azurerm_mssql_server.sql-db-server.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

resource "azurerm_app_service_source_control" "api_source_control" {
  app_id   = azurerm_app_service.api_app.id
  repo_url = "https://github.com/FasTnT/epcis-ef-core"
  branch   = "main"
}