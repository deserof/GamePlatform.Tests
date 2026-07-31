# GamePlatform.Tests

## User Secrets (credentials)

Do not commit real tester email/password. They are loaded from .NET User Secrets (local) or environment variables (CI).

Priority: `appsettings.{env}.json` → User Secrets → environment variables.

### Setup (local)

```bash
dotnet user-secrets init --project Player.Api.Tests --id gameplatform-tests-player-api

dotnet user-secrets set "TestSettings:Tester:Email" "YOUR_EMAIL" --project Player.Api.Tests
dotnet user-secrets set "TestSettings:Tester:Password" "YOUR_PASSWORD" --project Player.Api.Tests

dotnet user-secrets list --project Player.Api.Tests
```

### CI / remote

```bash
# PowerShell
$env:TestSettings__Tester__Email = "YOUR_EMAIL"
$env:TestSettings__Tester__Password = "YOUR_PASSWORD"

# bash
export TestSettings__Tester__Email="YOUR_EMAIL"
export TestSettings__Tester__Password="YOUR_PASSWORD"
```

## Generate Player API HTTP client

Run from the repository root:

```bash
nswag openapi2csclient /input:GamePlatform.Tests.Infrastructure/OpenApi/PlayerApi.json /classname:PlayerApiClient /namespace:GamePlatform.Tests.Infrastructure.Clients /output:GamePlatform.Tests.Infrastructure/Clients/PlayerApiClient.cs /GenerateClientInterfaces:true /UseBaseUrl:false /InjectHttpClient:true /DisposeHttpClient:false /GenerateExceptionClasses:true /ExceptionClass:PlayerApiException /JsonLibrary:SystemTextJson /GenerateOptionalParameters:true /OperationGenerationMode:SingleClientFromOperationId
```

## Contract mismatches (OpenAPI vs real API)

OpenAPI schema is left as provided. Steps use real response shapes where the contract is wrong.

| Endpoint | OpenAPI | Actual API |
|---|---|---|
| `POST /api/tester/login` | `TokenDTO`: `access_token`, `token_type`, `expires_in`, `scope` | `{ "accessToken": "...", "user": { ... } }` |
| `POST /api/automationTask/create` | `PlayerResponseDTO.id` as `integer` | `_id` as **string** (Mongo-like ObjectId); also returns password fields / `__v` |
| `POST /api/automationTask/getOne` | status **201**, `PlayerResponseDTO.id` as `integer` | status **200**, `id` as **string** |
| `GET /api/automationTask/getAll` | single `PlayerResponseDTO` | **array** of players; each item has `id` as **string** (not `_id`) |
| `DELETE /api/automationTask/deleteOne/{id}` | `id` path param as `integer` | `id` is a **string** ObjectId |

### Inconsistencies inside the real API

- Create returns `_id`, while getOne/getAll return `id` for the same identifier.
- Login and player endpoints do not match the documented DTO field names/types.

Workarounds live in `AuthSteps` / `PlayerSteps` and models `LoginResponse`, `PlayerApiModel` (not in the generated NSwag client).
