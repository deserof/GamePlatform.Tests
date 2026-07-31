# GamePlatform.Tests

## Generate Player API HTTP client

Run from the repository root:

```bash
nswag openapi2csclient /input:GamePlatform.Tests.Infrastructure/OpenApi/PlayerApi.json /classname:PlayerApiClient /namespace:GamePlatform.Tests.Infrastructure.Clients /output:GamePlatform.Tests.Infrastructure/Clients/PlayerApiClient.cs /GenerateClientInterfaces:true /UseBaseUrl:false /InjectHttpClient:true /DisposeHttpClient:false /GenerateExceptionClasses:true /ExceptionClass:PlayerApiException /JsonLibrary:SystemTextJson /GenerateOptionalParameters:true /OperationGenerationMode:SingleClientFromOperationId
```
