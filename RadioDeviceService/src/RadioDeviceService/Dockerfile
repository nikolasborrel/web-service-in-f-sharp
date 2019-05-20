FROM microsoft/dotnet:2.2-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.2-sdk AS build
WORKDIR /src
COPY RadioDeviceService.fsproj /RadioDeviceService/
RUN dotnet restore /RadioDeviceService/
COPY . /RadioDeviceService/
WORKDIR /src/RadioDeviceService
RUN dotnet build /RadioDeviceService/ -c Release -o /app

FROM build AS publish
RUN dotnet publish /RadioDeviceService/ -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "RadioDeviceService.dll"]
