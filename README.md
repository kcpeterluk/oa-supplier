# OA Supplier

OA Supplier is a supplier management solution with a .NET backend and a separate Vue client.

## Applications

### ASP.NET Core backend

`src/OA.Supplier.WebApp` is the .NET 10 web application. It hosts:

- A Blazor Server UI.
- ASP.NET Core Identity authentication.
- Supplier API endpoints under `/api/suppliers`.
- SQL Server persistence for supplier data and identity data.

The domain, application, and infrastructure projects under `src/` support this application.

### Vue frontend

`src/oa.supplier.webclient` is a Vue 3 and Vite client. It signs in through the backend and calls:

- `GET /api/suppliers`
- `GET /api/suppliers/overlaps`

During local development, Vite proxies `/api` requests to the backend at `http://localhost:5215`.

## Setup

### Prerequisites

Install the following tools:

- .NET 10 SDK
- Node.js and npm
- Docker

### Restore dependencies

From the repository root, restore the .NET solution:

```bash
dotnet restore OA.Supplier.slnx
```

Install the Vue client dependencies:

```bash
npm ci --prefix src/oa.supplier.webclient
```

### Start SQL Server

The development backend configuration expects SQL Server on port `55001`.

If the effective `SupplierDbContext` connection string from `src/OA.Supplier.WebApp/appsettings.json` or `src/OA.Supplier.WebApp/appsettings.Development.json` does not set `Integrated Security=true`, the app reads SQL Server credentials from environment variables during development:

```bash
export MSSQL_SA_ID='<your SQL Server user>'
export MSSQL_SA_PASSWORD='<your SQL Server password>'
```

The `MSSQL_SA_PASSWORD` value must match the password configured for the local SQL Server instance.

Start a local SQL Server container:

```bash
docker run \
  --name oa-supplier-sql \
  --env ACCEPT_EULA=Y \
  --env MSSQL_SA_PASSWORD="${MSSQL_SA_PASSWORD}" \
  --publish 55001:1433 \
  --detach \
  mcr.microsoft.com/mssql/server:2025-latest
```

If the container already exists, start it instead:

```bash
docker start oa-supplier-sql
```

After the backend is running, open the Blazor home page and use the `Initialise Database` button to create the local database schema.

### Configure launch settings

`src/OA.Supplier.WebApp/Properties/launchSettings.json` is ignored by Git because it can contain local ports and environment variables. Create this file locally if you want to run the backend with `--launch-profile http` or from an IDE.

Use placeholders for any values that are specific to your machine. Only include `MSSQL_SA_ID` and `MSSQL_SA_PASSWORD` when the effective connection string does not use `Integrated Security=true`.

```json
{
  "$schema": "https://json.schemastore.org/launchsettings.json",
  "profiles": {
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:5215",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "MSSQL_SA_ID": "<your SQL Server user>",
        "MSSQL_SA_PASSWORD": "<your SQL Server password>"
      }
    },
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:7177;http://localhost:5215",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "MSSQL_SA_ID": "<your SQL Server user>",
        "MSSQL_SA_PASSWORD": "<your SQL Server password>"
      }
    }
  }
}
```

## Run the Applications

### Run the backend

From the repository root:

```bash
dotnet run --project src/OA.Supplier.WebApp/OA.Supplier.WebApp.csproj --launch-profile http
```

Open the backend at `http://localhost:5215`.

### Run the Vue frontend

In another terminal, start the Vite development server:

```bash
npm run dev --prefix src/oa.supplier.webclient
```

Open the Vite URL shown in the terminal, typically `http://localhost:5173`.

## Run the Tests

Run the .NET component tests from the repository root:

```bash
dotnet test OA.Supplier.slnx
```

Docker must be running because the component tests use Testcontainers to start SQL Server.

Validate the Vue frontend build:

```bash
npm run build --prefix src/oa.supplier.webclient
```

There is currently no dedicated frontend test script.
