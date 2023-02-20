[![Codacy Badge](https://app.codacy.com/project/badge/Grade/5c0fa82713fd4960b5b91d95b4143e7f)](https://www.codacy.com/gh/FasTnT/epcis-ef-core/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=FasTnT/epcis-ef-core&amp;utm_campaign=Badge_Grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/5c0fa82713fd4960b5b91d95b4143e7f)](https://www.codacy.com/gh/FasTnT/epcis-ef-core/dashboard?utm_source=github.com&utm_medium=referral&utm_content=FasTnT/epcis-ef-core&utm_campaign=Badge_Coverage)
[![.NET](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml/badge.svg)](https://github.com/FasTnT/epcis-ef-core/actions/workflows/dotnet.yml)

# [EPCIS](https://louisaxel-ambroise.github.io/epcis/)

FasTnT EPCIS is a lightweight GS1 EPCIS 1.2 and 2.0 repository written in C# using .NET7 and backed using EntityFramework Core.

The repository fully supports the following databases:
 - SqlServer *(provider: SqlServer)*
 - PostGreSQL *(provider: Postgres)*
 - Sqlite *(provider: Sqlite)*

There is a [sandbox](https://louisaxel-ambroise.github.io/epcis/server.html) available if you want to quickly test this repository capabilities.

## Setup

1. Download the source code, and setup a database for FasTnT ;
2. Start the repository with the command `$ dotnet run -p src\FasTnT.Host\FasTnT.Host.csproj --urls "http://localhost:5102/" --connectionStrings:FasTnT.Database "{your connectionstring}" --FasTnT.Database.Provider "{yourProvider}"` ;

That's it! You have a properly working EPCIS repository.

The default value for the database provider is *SqlServer*.

You can also setup FasTnT EPCIS using the Docker image or in Azure quite easily. Check the [wiki](https://github.com/louisaxel-ambroise/epcis/wiki/Installation) for more details.

## HTTP Endpoints

The API is secured using HTTP Basic auth by default. 
The users are not stored in the database, but a hash of the authorization value is stored alongside the request. By default the events and masterdata returned in a query are restricted to the ones captured with the same authorization header.

### EPCIS 1.2 endpoints:

FasT&T provides a full implementation of the EPCIS 1.2 specification. The endpoints are:

- Capture: `POST /v1_2/Capture`
- Queries : `POST /v1_2/Query.svc`

**Capture** endpoint only supports requests with `content-type: application/xml` or `content-type: text/xml` header and XML payload.

**Queries** endpoint supports SOAP requests on endpoint `/v1_2/Query.svc`.

See the [wiki](https://github.com/louisaxel-ambroise/wiki) for more details.

#### Implemented Features

This is the list of implemented 1.2 features in the repository:

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

  
### EPCIS 2.0 endpoints:

A subset of EPCIS 2.0 specification is currently implemented in FasT&T repository. The endpoints are:

- Capture: `POST /v2_0/Capture`
- Query : `GET /v2_0/events`

**Capture** endpoint supports requests with both `content-type: application/xml` or `content-type: application/json` headers and payload.

**Queries** endpoint supports HTTP requests and supports both `accept: application/json` and `accept: application/xml` headers.

The subscriptions will always receive the results in JSON format.

The OpenApi definition of the v2.0 endpoints is available at the URL `/v2_0/openapi.json`. See the [wiki](https://github.com/louisaxel-ambroise/epcis/wiki) for more details.

#### Implemented Features

This is the list of planned and implemented 2.0 features in the repository:

- Capture
  - [x] Capture list of Events
  - [x] Capture a single Event
  - [x] Capture CBV masterdata
- Queries:
  - [x] List events
  - [x] Event pagination
  - [x] Create a named query
  - [x] Execute a named query
- Subscriptions:
  - [x] Subscribe to an EPCIS request (webhook)
  - [x] Subscribe to an EPCIS request (websocket)
- Discovery endpoints
   - [x] EventType discovery endpoint
   - [x] EPCs discovery endpoint
   - [x] Business Steps discovery endpoint
   - [x] Business Locations discovery endpoint
   - [x] Read Points discovery endpoint
   - [x] Dispositions discovery endpoint

# Authors

External contributions on this EPCIS repository are welcome from anyone.
This project was created an is primarily maintained by [Louis-Axel Ambroise](https://github.com/louisaxel-ambroise).

# License

This project is licensed under the Apache 2.0 license - see the LICENSE file for details

Contact: fastnt@pm.me
