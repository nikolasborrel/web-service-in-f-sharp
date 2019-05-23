# A web service in F# #
An example of a web service written in asp.net core using [Giraffe](https://github.com/giraffe-fsharp/Giraffe) and F#

## Overview
Project files
```verbatim
RadioDeviceService/
    Models.fs : DTOs and internal model representations
    RadioRepository.fs : simple in-memory repository for getting/setting radio profiles and locations
    HttpHandlers.fs : for handling incoming requests
    Program.fs : handling routes and configuring application
RadioDeviceService.Tests/
    Tests.fs : endpoint tests
    TestUtils.fs : utilities for tests
```

## Running

### Docker: run the web service
Open a Terminal and go to the `RadioDeviceService/src/RadioDeviceService` folder. To run on `localhost:8000` (port 8000):
```bash
docker build -t radio-device-ws:latest .
docker run --publish 8000:5000 --rm radio-device-ws
```

### Docker: Tests
Go to the root directory `RadioDeviceService/`. To run:
```bash
docker build -t radio-device-ws-tests:latest .
```

### Local machine (non-Docker)
Go to the root directory `RadioDeviceService/` and build:
```bash
 build.bat
```

To run the web service on `localhost:5000` (port 5000):
```bash
dotnet run --project src/RadioDeviceService/
```