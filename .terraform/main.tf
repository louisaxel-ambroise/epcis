terraform {
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = ">=3.37.0"
    }
  }

  required_version = ">= 1.0.0"
}

provider "azurerm" {
  features {}

  subscription_id = var.subscription_id
}

locals {
  tags = {
    Solution = "fastnt"
    Environment = upper(var.environment)
  }
  base_resource_name = lower("${var.environment}")
}

resource "azurerm_resource_group" "fastnt_main" {
  name     = local.base_resource_name
  location = var.location
  tags     = local.tags
}

resource "azurerm_service_plan" "plan" {
  name                = "${local.base_resource_name}-app-plan"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  os_type			  = "Windows"
  sku_name		      = var.app_size
  tags = local.tags
}

resource "azurerm_application_insights" "api_appi" {
  name                = "${local.base_resource_name}-api-appinsights"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  application_type    = "web"
  tags                = local.tags
}

resource "azurerm_windows_web_app" "api_app" {
  name                = "${local.base_resource_name}-api"
  location            = var.location
  resource_group_name = azurerm_resource_group.fastnt_main.name
  service_plan_id	  = azurerm_service_plan.plan.id
  https_only          = true
  tags                = local.tags
  site_config {
	always_on		    = false
	application_stack {
		current_stack 	  = "dotnet"
		dotnet_version    = "v7.0"
	}
  }
  auth_settings {
    enabled = false
  }
  app_settings = {
    "ASPNETCORE_ENVIRONMENT" = "${var.aspnetcore_environment}"
    "APPINSIGHTS_INSTRUMENTATIONKEY" = "${azurerm_application_insights.api_appi.instrumentation_key}"
    "APPLICATIONINSIGHTS_CONNECTION_STRING" : "${azurerm_application_insights.api_appi.connection_string}"
    "SCM_COMMAND_IDLE_TIMEOUT" : "3600"
    "ApplicationInsightsAgent_EXTENSION_VERSION" : "~2"
    "ConnectionStrings:FasTnT.Database" : "Data Source=${local.base_resource_name}.db;"
    "FasTnT.Database.Provider" : "Sqlite"
  }
}

resource "azurerm_app_service_source_control" "api_source_control" {
  app_id   = azurerm_windows_web_app.api_app.id
  repo_url = "https://github.com/louisaxel-ambroise/epcis"
  branch   = "main"
}