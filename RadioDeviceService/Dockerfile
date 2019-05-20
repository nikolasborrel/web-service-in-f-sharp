FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY . ./
RUN dotnet restore tests/RadioDeviceService.Tests/*.fsproj
RUN dotnet build tests/RadioDeviceService.Tests/*.fsproj
RUN dotnet test tests/RadioDeviceService.Tests/*.fsproj