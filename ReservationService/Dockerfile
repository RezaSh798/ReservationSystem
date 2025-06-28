FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY ../ReservationService/ ./ReservationService/
COPY ../Reservation.Contracts/ ./Reservation.Contracts/
WORKDIR /app/ReservationService
RUN dotnet restore

RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/publish ./

EXPOSE 3000
ENTRYPOINT ["dotnet", "ReservationService.dll"]
