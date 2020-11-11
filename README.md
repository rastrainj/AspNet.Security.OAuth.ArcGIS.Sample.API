# AspNet.Security.OAuth.ArcGIS.Sample.API

[ASP.NET Core](https://docs.microsoft.com/es-es/aspnet/core/?view=aspnetcore-3.1 "ASP.NET Core") API Sample to test ArcGIS Authentication middleware.

## How to build

AspNet.Security.OAuth.ArcGIS.Sample.API is built against the latest NET Core 3.

- [Install](https://dotnet.microsoft.com/download/dotnet-core/3.1) the [required](https://github.com/rastrainj/AspNet.Security.OAuth.ArcGIS.Sample.API/blob/main/global.json) .NET Core SDK.
- Run `dotnet build` in the root of the repo.

## Usage

- Manage user secrets:

```json
{
  "Credentials": {
    "ClientId": "your-AGOL-client-id-here",
    "ClientSecret": "your-AGOL-client-secret-",
    "AuthorizationEndpoint": "your-AGOL-authorization-endpoint-here",
    "TokenEndpoint": "your-AGOL-token-endpoint-here"
  }
}
```

- Run `dotnet run -p .\AspNet.Security.OAuth.ArcGIS.Sample.API\AspNet.Security.OAuth.ArcGIS.Sample.API.csproj`

- Test API with [Swagger](https://localhost:5001/index.html) or [directly](https://localhost:5001/api/Sample/anonymous).
