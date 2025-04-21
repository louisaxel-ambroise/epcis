[![Last Commit (develop)](https://img.shields.io/github/last-commit/louisaxel-ambroise/epcis/main.svg?logo=github)](https://github.com/FasTnT/epcis/commits/develop)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/5c0fa82713fd4960b5b91d95b4143e7f)](https://www.codacy.com/gh/FasTnT/epcis-ef-core/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=FasTnT/epcis-ef-core&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/5c0fa82713fd4960b5b91d95b4143e7f)](https://www.codacy.com/gh/FasTnT/epcis-ef-core/dashboard?utm_source=github.com&utm_medium=referral&utm_content=FasTnT/epcis-ef-core&utm_campaign=Badge_Coverage)
[![.NET](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml)

# [FasTnT EPCIS repository](https://louisaxel-ambroise.github.io/epcis/)

FasTnT EPCIS is a lightweight GS1 EPCIS 1.2 and 2.0 repository written in C# using .NET 10 and backed with EntityFramework Core.

The repository fully supports the following databases:
 - SqlServer *(provider: SqlServer)*
 - PostGreSQL *(provider: Postgres)*
 - Sqlite *(provider: Sqlite)*

## Environments and Testing

There is a [sandbox](https://louisaxel-ambroise.github.io/epcis/server.html) available if you want to quickly test this EPCIS repository capabilities.

A [Postman team](https://www.postman.com/fastnt-epcis) is also available that contains collections for both XML/SOAP and JSON endpoints. Alternatively, a [SoapUI project](https://github.com/louisaxel-ambroise/epcis/blob/main/Documents/EPCIS%201.2%20queries-soapui-project.xml) can be found in the source code that contains queries for the v1.2 implementation.

## Setup

1. Download the source code: `$ git clone https://www.github.com/louisaxel-ambroise/epcis && cd epcis`
2. Apply database migrations: `$ dotnet ef database update --project src/FasTnT.Host --connection "Data Source=fastnt.db;" -- --FasTnT.Database.Provider "Sqlite"`
3. Start the repository: `$ dotnet run --project src\FasTnT.Host\FasTnT.Host.csproj --urls "http://localhost:5102/" --connectionStrings:FasTnT.Database "Data Source=fastnt.db;" --FasTnT.Database.Provider "Sqlite"`

That's it! You have a properly working EPCIS repository using Sqlite as storage.

You can replace the Connection String and Database Provider with the values that suits better your needs (Sqlite, SqlServer or PostGreSQL).

You can also setup FasTnT EPCIS using the Docker image or in Azure very easily. Check the [wiki](https://github.com/louisaxel-ambroise/epcis/wiki/Installation) for more details.

## HTTP Endpoints

The API is secured using HTTP Basic auth by default. 
The users are not stored in the database, but a hash of the authorization value is stored alongside the request. By default the events and masterdata returned in a query are restricted to the ones captured with the same authorization header.

### EPCIS endpoints:

FasT&T provides a full implementation of the EPCIS 1.2 and 2.0 specification. The endpoints are:

- Capture: `POST /Capture`
- Query : `GET /events`
- SOAP Service : `POST /Query.svc`

**Capture** and **Query** endpoints supports requests with `content-type: application/json` (EPCIS v2.0), `content-type: application/xml` or `content-type: text/xml` header and JSON or XML payload.

**Queries** endpoint supports SOAP requests on endpoint `/Query.svc`.

See the [wiki](https://github.com/louisaxel-ambroise/wiki) for more details.

#### Implemented Features

This is the list of implemented 1.2 and 2.0 features in the repository:

- Capture
  - [x] Capture a list of Events
  - [x] Capture a single Event
  - [x] Capture Master Data (CBV)
- Queries:
  - [x] List events
  - [x] Event pagination
  - [x] Create/Delete a named query
  - [x] Execute a named query
- Soap Queries:
  - [x] GetVendorVersion
  - [x] GetStandardVersion
  - [x] GetQueryNames
  - [x] GetSubsciptionIDs
  - Poll
    - [x] SimpleEventQuery
    - [x] SimpleMasterDataQuery
- Subscriptions:
  - [x] Subscribe to an EPCIS request (webhook)
  - [x] Subscribe to an EPCIS request (websocket)
  - [x] Trigger subscriptions that register to specific trigger name
  - [x] Execute subscription based on schedule
- Discovery endpoints
   - [x] EventType discovery endpoint
   - [x] EPCs discovery endpoint
   - [x] Business Steps discovery endpoint
   - [x] Business Locations discovery endpoint
   - [x] Read Points discovery endpoint
   - [x] Dispositions discovery endpoint
- Query Callback:
  - [x] CallbackResults
  - [x] CallbackQueryTooLargeException
  - [x] CallbackImplementationException


The OpenApi definition of the EPCIS 2.0 endpoints is available at the URL `/v2_0/openapi.json`. See the [wiki](https://github.com/louisaxel-ambroise/epcis/wiki) for more details.
   
#### Restrictions

- Only `rollback` value is accepted for `GS1-Capture-Error-Behaviour` header
- Only `Never_Translates` value is accepted for `GS1-EPC-Format` header
- Requests are captured synchronously in the repository

# Authors

External contributions on this EPCIS repository are welcome from anyone.
This project was created an is primarily maintained by [Louis-Axel Ambroise](https://github.com/louisaxel-ambroise).

# License

This project is licensed under the Apache 2.0 license - see the LICENSE file for details

Contact: fastnt@pm.me
