# RBS.Auth
## Room Booking Service authorization service.
![Build status](https://github.com/IllyaKh/RBS.Auth/actions/workflows/production-ci-cd.yaml/badge.svg)
RBS.Auth is a part of RBS system, that allows to Sign In/Sign Up users to the system.


## Features

- Login customer to the system (generate personal access token)
- Register customer
- Check availability of the server

## Endpoints

| Action | Request Type | Endpoint|
| ------ | ------ | ------ |
| Sign In | POST | /api/Auth/Login | 
| Sign Up | PUT | /api/Auth/Register | 
| Check availability | GET | /api | 

## Installation

RBS.Auth requires [.NET 6.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0) to run.

Install the dotnet and start the server.

```sh
cd RBS.Auth.WebApi\RBS.Auth.WebApi
dotnet run RBS.Auth.WebApi -c Release
```

