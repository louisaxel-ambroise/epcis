[![Codacy Badge](https://api.codacy.com/project/badge/Grade/22befd4bc63c410d8ac41f903fd079dc)](https://app.codacy.com/gh/FasTnT/epcis-ef-core?utm_source=github.com&utm_medium=referral&utm_content=FasTnT/epcis-ef-core&utm_campaign=Badge_Grade_Settings)
[![.NET](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml)

# epcis-ef-core
EF Core version of EPCIS repository

FasTnT EPCIS is a lightweight GS1 EPCIS 1.2 repository written in C# using .NET 5 and backed using EntityFramework Core.

## Setup

Prerequisites:
- PostGreSQL 9.5 or higher
- .NET 5 SDK

Steps:
1. Download the source code, and create a new user/database in PostGreSQL for FasTnT ;
2. Start the repository with the command `$ dotnet run -p src\FasTnT.Host\FasTnT.Host.csproj --urls "http://localhost:5102/" --connectionStrings:FasTnT.Database "{your connectionstring}"` ;

That's it! You have a properly working EPCIS 1.2 repository.

## HTTP Endpoints

### EPCIS 1.2 endpoints:

The API is secured using HTTP Basic authentication. The default username:password value is `admin:P@ssw0rd`

- Capture: `POST /v1_2/Capture`
- Queries : `POST /v1_2/Query.svc`

**Capture** endpoint only supports requests with `content-type: application/xml` or `content-type: text/xml` header and XML payload.

**Queries** endpoint supports SOAP requests on endpoint `/v1_2/Query.svc`.

See the [wiki](https://github.com/FasTnT/epcis-ef-core/wiki) for more details.

## Implemented Features

This is the list of planned and implemented features in the repository:

- Capture
  - [x] Events
  - [x] Capture Master Data (CBV) - Hierarchy is not yet supported
- Queries:
  - [x] GetVendorVersion
  - [x] GetStandardVersion
  - [x] GetQueryNames
  - [ ] GetSubsciptionIDs
  - Poll
    - [x] SimpleEventQuery
    - [ ] SimpleMasterDataQuery - exists, but query parameters are not applied
- Query Callback:
  - [ ] CallbackResults
  - [ ] CallbackQueryTooLargeException
  - [ ] CallbackImplementationException
- Subscriptions:
  - [ ] Subscribe to an EPCIS request
  - [ ] Unsubscribe from EPCIS repository
  - [ ] Trigger subscriptions that register to specific trigger name

# Authors

External contributions on FasTnT EPCIS repository are welcome from anyone.
This project was created an is primarily maintained by [Louis-Axel Ambroise](https://github.com/louisaxel-ambroise).

# License

This project is licensed under the Apache 2.0 license - see the LICENSE file for details

Contact: fastnt@pm.me
