[![Codacy Badge](https://api.codacy.com/project/badge/Grade/22befd4bc63c410d8ac41f903fd079dc)](https://app.codacy.com/gh/FasTnT/epcis-ef-core?utm_source=github.com&utm_medium=referral&utm_content=FasTnT/epcis-ef-core&utm_campaign=Badge_Grade_Settings)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/2c8ff479623d4cfc82a24523bf3dee6d)](https://www.codacy.com/gh/FasTnT/epcis-ef-core/dashboard?utm_source=github.com&utm_medium=referral&utm_content=FasTnT/epcis-ef-core&utm_campaign=Badge_Coverage)
[![.NET](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml)

# [EPCIS](https://fastnt.github.io/)
EF Core version of EPCIS repository

FasTnT EPCIS is a lightweight GS1 EPCIS 1.2 repository written in C# using .NET 5 and backed using EntityFramework Core.

There is a [sandbox](https://fastnt.github.io/sandbox.html) available if you want to quickly test this repository capabilities.

## Setup

Prerequisites:
- SQL Server 2016 or +
- .NET 5 SDK

Steps:
1. Download the source code, and create a new user/database in SQL Server for FasTnT ;
2. Start the repository with the command `$ dotnet run -p FasTnT.Host\FasTnT.Host.csproj --urls "http://localhost:5102/" --connectionStrings:FasTnT.Database "{your connectionstring}"` ;

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
  - [x] Capture Master Data (CBV)
- Queries:
  - [x] GetVendorVersion
  - [x] GetStandardVersion
  - [x] GetQueryNames
  - [x] GetSubsciptionIDs
  - Poll
    - [x] SimpleEventQuery
    - [x] SimpleMasterDataQuery
- Query Callback:
  - [x] CallbackResults
  - [x] CallbackQueryTooLargeException
  - [x] CallbackImplementationException
- Subscriptions:
  - [x] Subscribe to an EPCIS request
  - [x] Unsubscribe from EPCIS repository
  - [x] Trigger subscriptions that register to specific trigger name
  - [x] Execute subscription based on schedule

# Authors

External contributions on FasTnT EPCIS repository are welcome from anyone.
This project was created an is primarily maintained by [Louis-Axel Ambroise](https://github.com/louisaxel-ambroise).

# License

This project is licensed under the Apache 2.0 license - see the LICENSE file for details

Contact: fastnt@pm.me
