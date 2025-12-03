# Dublin Bikes API — README

This repository is a versioned API for Dublin bike stations (v1: file-backed, v2: Cosmos-backed). It implements searching, filtering, sorting, paging, caching, a background updater that mutates station availability, and basic tests + a Postman collection.

Requirements
- .NET 8 SDK
- Visual Studio 2022 (optional) or CLI
- (Optional) Node.js + Newman to run the Postman collection from CLI

Quick start — run locally
1. Open the solution in Visual Studio 2022 and start the project:
   - Select the API project and run via __F5__ or use __Debug > Start Debugging__.
   - Or build and run from CLI:
     - `dotnet build`
     - `dotnet run --project fs-2025-assessment-1-74918.csproj`
2. By default the app runs with HTTPS. Check the exact URL/port in `Properties/launchSettings.json` and use that as the base URL for requests (example below uses `https://localhost:5001`).

Swagger UI (development only)
- When running in Development, Swagger UI is available at:
  - `https://localhost:5001/swagger`
- You can use it to explore v1 and v2 docs.

Postman collection
- File: `postman/Stations.postman_collection.json` (included in repo)
- Example run with Newman (if installed):
  - `newman run postman/Stations.postman_collection.json -e postman/Stations.postman_environment.json`
- Or import the collection into the Postman app and run with the Collection Runner.

Example curl requests
- List stations (paged):
  - curl "https://localhost:5001/api/v1/stations?page=1&pageSize=20"
- List with search/filter/sort:
  - curl "https://localhost:5001/api/v1/stations?q=smith&status=OPEN&minBikes=1&sort=availableBikes&dir=desc&page=1&pageSize=10"
- Get single station:
  - curl "https://localhost:5001/api/v1/stations/42"
- Summary:
  - curl "https://localhost:5001/api/v1/stations/summary"
- Create (POST) a station (v1 supports in-memory add; v2 may be NotSupported):
  - curl -X POST "https://localhost:5001/api/v1/stations" -H "Content-Type: application/json" -d @newstation.json
- Update (PUT) station:
  - curl -X PUT "https://localhost:5001/api/v1/stations/123" -H "Content-Type: application/json" -d @updatestation.json

Returned fields (station object)
- `number` (int) — station identifier
- `name` (string)
- `address` (string)
- `position` { `lat`, `lng` } — GPS coordinates
- `bike_stands` (int) — total stands
- `available_bikes` (int)
- `available_bike_stands` (int)
- `status` (string) — e.g., `OPEN` / `CLOSED`
- `last_update` (long) — epoch milliseconds (original JSON)
- `last_update_utc` (ISO 8601) — computed DateTimeOffset UTC
- `last_update_local` (ISO 8601) — computed Europe/Dublin local time (falls back to UTC)
- `occupancy` (double) — computed: available_bikes / bike_stands (safe when bike_stands == 0)

Paged response shape (GET /api/v{version}/stations)
- Returns a JSON object:
  - `items` — array of station objects
  - `total` — total matched items
  - `page` — current page
  - `pageSize` — items per page
  - `totalPages` — computed total pages

Caching
- GET queries and summary are cached in-memory for 5 minutes by default. Cache keys include query parameters and page info.

Assumptions & notes
- v1 uses `Data/dublinbike.json` (file-backed) and supports POST/PUT in-memory.
- v2 uses the `CosmosBikeRepository` when configured. Write operations (POST/PUT) in v2 are NotSupported unless the repository is extended.
- Background updater (`BackgroundServices/BikeStationUpdater`) updates in-memory station availability periodically to simulate a live feed.
- Timezone conversion uses `TimeZoneInfo.FindSystemTimeZoneById("Europe/Dublin")`. On hosts without tzdata, local time falls back to UTC.
- The Postman collection uses `{{baseUrl}}` variable — update it to match your running app.

Running tests
- Unit + integration tests are available in the test project (if present). Run:
  - `dotnet test`
- You can run the Postman collection manually (Postman) or via Newman.

If you want, I can:
- Add example JSON files for POST/PUT (`postman/sample_new_station.json`).
- Add CI workflow to run `dotnet test` and Newman on each push.