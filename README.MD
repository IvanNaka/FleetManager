# FleetManager

FleetManager is a .NET 8 Web API for managing vehicles and their chassis in a fleet. It provides endpoints to create, retrieve, update, and list vehicles, as well as manage chassis information.

## Features

- Register new vehicles with chassis information
- Retrieve vehicle details by chassis
- Update vehicle color
- List all vehicles
- Built with ASP.NET Core (.NET 8) and Entity Framework Core

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- (Optional) [Visual Studio 2022](https://visualstudio.microsoft.com/vs/)

## Getting Started

### 1. Clone the repository

### 2. Configure the Database

Edit `FleetManager.WebAPI/appsettings.json` and set your SQL Server connection string: 
"ConnectionStrings": { "DefaultConnection": "Server=YOUR_SERVER;Database=FleetManager;Trusted_Connection=True;MultipleActiveResultSets=true" }

### 3. Apply Migrations
dotnet ef database update --project FleetManager.Infrastructure --startup-project FleetManager.WebAPI

### 4. Run the API
dotnet run --project FleetManager.WebAPI

The API will be available at `https://localhost:5001` or `http://localhost:5000`.

## API Endpoints

| Method | Route                                 | Description                       |
|--------|---------------------------------------|-----------------------------------|
| POST   | `/api/vehicle`                        | Create a new vehicle              |
| GET    | `/api/vehicle/{serie}/{number}`       | Get vehicle by chassis            |
| PUT    | `/api/vehicle/{serie}/{number}/color` | Update vehicle color              |
| GET    | `/api/vehicle`                        | List all vehicles                 |

### Example: Create Vehicle
POST /api/vehicle { "chassisSeries": "ABC123", "chassisNumber": 456, "type": "Car", "color": "Red" }

### Example: Update Vehicle Color
PUT /api/vehicle/ABC123/456/color { "newColor": "Blue" }

## Testing

To run unit tests:
dotnet test