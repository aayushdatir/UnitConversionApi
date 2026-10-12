# Unit Conversion API

A maintainable ASP.NET Core Web API for converting numeric values between units of length, temperature, and weight/mass.

## Requirements

- .NET 9 SDK
- Git (for repository setup)

## Run locally

From the repository root:

```bash
dotnet restore
dotnet build
dotnet run --project src/UnitConversionApi
```

The launch profile uses:

- Swagger UI: `https://localhost:7081/swagger`
- HTTP base URL: `http://localhost:5081`

If your local SDK does not include .NET 10, install the .NET 9 SDK or change both project files to a supported target framework available on your machine.

## API endpoints

### Convert a value

`POST /api/conversions`

Request:

```json
{
  "category": "length",
  "value": 2,
  "fromUnit": "km",
  "toUnit": "m"
}
```

Example response:

```json
{
  "category": "length",
  "inputValue": 2,
  "fromUnit": "km",
  "result": 2000,
  "toUnit": "m"
}
```

Temperature example:

```json
{
  "category": "temperature",
  "value": 100,
  "fromUnit": "c",
  "toUnit": "f"
}
```

### List categories

`GET /api/conversions/categories`

### List units for a category

`GET /api/conversions/units/length`

Valid categories: `length`, `temperature`, `weight`, `mass`.

## Supported units

- **Length:** `m`, `km`, `cm`, `mm`, `mi`, `yd`, `ft`, `in`
- **Temperature:** `c`, `f`, `k`
- **Weight/mass:** `kg`, `g`, `mg`, `lb`, `oz`, `t`

Unit symbols and categories are case-insensitive. Temperature values below absolute zero are rejected.

## Error behavior

- `400 Bad Request`: invalid numeric values or physically invalid temperatures.
- `404 Not Found`: unsupported category or unit.
- Model validation errors are automatically returned by `[ApiController]` as `400 Bad Request`.

## Run tests

```bash
dotnet test
```

Tests cover representative length, mass, and temperature conversions, unknown units, invalid numeric values, and absolute zero validation.

## Design decisions and trade-offs

- **Controller-based API:** keeps HTTP concerns separate from conversion logic and is familiar to teams maintaining ASP.NET Core services.
- **Conversion service:** contains conversion rules and can be unit-tested independently from HTTP.
- **Unit registry:** centralizes categories, unit symbols, names, and factors. Adding a linear unit normally requires only a registry entry; temperature remains a special conversion because it uses offsets as well as scaling.
- **Dependency injection:** the registry is registered as a singleton because it is read-only after construction; the conversion service is scoped.
- **Linear conversion strategy:** each linear unit has a factor relative to a base unit. Convert to the base unit, then to the destination unit. This avoids writing a separate formula for every pair.
- **Temperature strategy:** convert to Celsius first, then to the destination scale.
- **Precision:** results are rounded to eight decimal places to keep responses readable. For scientific or financial use, precision and rounding should be defined as explicit domain requirements.
- **Extensibility:** a larger catalogue can be loaded from configuration or a database, and categories can later use separate strategy implementations if conversion rules become more complex.
- **Scope:** this sample does not include authentication, rate limiting, persistence, or telemetry because they are not required for the core conversion challenge. Those can be added for a production deployment.

## Push to GitHub

Create an empty repository on GitHub, then run these commands from this directory:

```bash
git init
git add .
git commit -m "Build unit conversion API"
git branch -M main
git remote add origin https://github.com/YOUR_USERNAME/UnitConversionApi.git
git push -u origin main
```

Replace `YOUR_USERNAME` with your GitHub username and use the URL of your own repository. Do not commit secrets or access tokens.

## Suggested repository structure

```text
UnitConversionApi/
├── src/
│   └── UnitConversionApi/
│       ├── Controllers/
│       ├── Models/
│       ├── Services/
│       ├── Properties/
│       ├── Program.cs
│       └── UnitConversionApi.csproj
├── tests/
│   └── UnitConversionApi.Tests/
├── .gitignore
├── README.md
└── UnitConversionApi.sln
```
