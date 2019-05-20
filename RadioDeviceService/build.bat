dotnet restore src/RadioDeviceService
dotnet build src/RadioDeviceService

dotnet restore tests/RadioDeviceService.Tests
dotnet build tests/RadioDeviceService.Tests
dotnet test tests/RadioDeviceService.Tests
