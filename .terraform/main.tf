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

resource "azurerm_storage_account" "fastnt_storage" {
  name                      = "fastnt${var.environment}st"
  resource_group_name       = azurerm_resource_group.fastnt_main.name
  location                  = var.location
  account_tier              = "Standard"
  account_replication_type  = "LRS"
  min_tls_version		    = "TLS1_2"
  enable_https_traffic_only = true
  queue_properties {
   hour_metrics {
      enabled               = true
      include_apis          = true
      retention_policy_days = 7
      version               = "1.0"
    }
    minute_metrics {
      enabled               = false
      include_apis          = false
      retention_policy_days = 7
      version               = "1.0"
    }
	logging {
	  write                 = true
	  read                  = true
	  delete                = true
	  retention_policy_days = 7
	  version               = "1.0"
	}
  }
  network_rules {
    default_action             = "Deny"
  }
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

resource "azurerm_mssql_database_extended_auditing_policy" "sqldb-audit-policy" {
  database_id                             = azurerm_mssql_database.sqldb.id
  storage_endpoint                        = azurerm_storage_account.fastnt_storage.primary_blob_endpoint
  storage_account_access_key              = azurerm_storage_account.fastnt_storage.primary_access_key
  storage_account_access_key_is_secondary = false
  retention_in_days                       = 90
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

resource "azurerm_application_insights_web_test" "api_webtest" {
  name                    = "${local.base_resource_name}-api-appinsights-webtest"
  location                = var.location
  resource_group_name     = azurerm_resource_group.fastnt_main.name
  application_insights_id = azurerm_application_insights.api_appi.id
  kind                    = "ping"
  frequency               = 900
  timeout                 = 60
  enabled                 = true
  geo_locations           = ["emea-fr-pra-edge", "emea-nl-ams-azr", "emea-ru-msa-edge"]

  configuration = <<XML
<WebTest Name="WebTest1" Id="ABD48585-0831-40CB-9069-682EA6BB3583" Enabled="True" CssProjectStructure="" CssIteration="" Timeout="0" WorkItemIds="" xmlns="http://microsoft.com/schemas/VisualStudio/TeamTest/2010" Description="" CredentialUserName="" CredentialPassword="" PreAuthenticate="True" Proxy="default" StopOnError="False" RecordedResultFile="" ResultsLocale="">
  <Items>
    <Request Method="GET" Guid="a5f10126-e4cd-570d-961c-cea43999a200" Version="1.1" Url="https://${local.base_resource_name}-api.azurewebsites.net" ThinkTime="0" Timeout="300" ParseDependentRequests="True" FollowRedirects="True" RecordResult="True" Cache="False" ResponseTimeGoal="0" Encoding="utf-8" ExpectedHttpStatusCode="200" ExpectedResponseUrl="" ReportingName="" IgnoreHttpStatusCode="False" />
  </Items>
</WebTest>
XML
}


resource "azurerm_app_service" "api_app" {
  name                = "${local.base_resource_name}-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  app_service_plan_id = azurerm_app_service_plan.plan.id
  client_cert_enabled = var.enable_cliet_cert
  https_only          = true
  tags                = local.tags
  site_config {
    dotnet_framework_version  = "v7.0"
    use_32_bit_worker_process = true
    http2_enabled             = true
  }
  auth_settings {
    enabled = false
  }
  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "${var.aspnetcore_environment}"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.api_appi.instrumentation_key}"
    "APPLICATIONINSIGHTS_CONNECTION_STRING" : "${azurerm_application_insights.api_appi.connection_string}"
    "ApplicationInsightsAgent_EXTENSION_VERSION" : "~2"
    "ConnectionStrings:FasTnT.Database" : "Server=tcp:${azurerm_mssql_server.sql-db-server.fully_qualified_domain_name},1433;Initial Catalog=${local.base_resource_name}-sql-db;Persist Security Info=False;User ID=${azurerm_mssql_server.sql-db-server.administrator_login};Password=${azurerm_mssql_server.sql-db-server.administrator_login_password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
  }
}

resource "azurerm_app_service_source_control" "api_source_control" {
  app_id   = azurerm_app_service.api_app.id
  repo_url = "https://github.com/louisaxel-ambroise/epcis"
  branch   = "main"
}