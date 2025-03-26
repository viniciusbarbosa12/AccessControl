variable "resource_group_name" {
  type        = string
  default     = "rg-access-control"
  description = "Resource Group name"
}

variable "location" {
  type        = string
  default     = "East US"
  description = "Azure region"
}

variable "app_service_plan_name" {
  type        = string
  default     = "access-control-plan"
  description = "App Service Plan name"
}

variable "app_service_plan_tier" {
  type        = string
  default     = "Free"
  description = "App Service Plan tier"
}

variable "app_service_plan_size" {
  type        = string
  default     = "F1"
  description = "App Service Plan size"
}

variable "app_service_name" {
  type        = string
  default     = "access-control-api-app"
  description = "App Service name"
}

variable "dotnet_version" {
  type        = string
  default     = "v7.0"
  description = ".NET runtime version"
}

variable "environment" {
  type        = string
  default     = "Production"
  description = "App environment (ASPNETCORE_ENVIRONMENT)"
}
