FROM hub.nexdev.net/base/dotnet/core/sdk:8.0-alpine AS builder

WORKDIR /src

COPY ./src/OWP.Core/. src/OWP.Core
COPY ./src/OWP.LiveDetailNotification/. ./src/OWP.LiveDetailNotification
COPY ./src/OWP.TicketNotification/. ./src/OWP.TicketNotification
COPY ./src/OWP.PreMatchNotification/. ./src/OWP.PreMatchNotification
COPY ./src/OWP.OtherSportNotification/. ./src/OWP.OtherSportNotification
COPY ./src/OWP.AssignmentUploadNotification/. ./src/OWP.AssignmentUploadNotification
COPY ./src/OWP.RabbitMQ/. ./src/OWP.RabbitMQ
COPY ./OWP.NotificationService/OWP.NotificationService.csproj ./OWP.NotificationService/OWP.NotificationService.csproj

WORKDIR /src/OWP.NotificationService
RUN dotnet nuget add source https://nuget.nexdev.net/api/v2
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet restore -v n

COPY ./OWP.NotificationService .
RUN --mount=type=cache,target=/root/.nuget/packages \
    dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:8.0-alpine AS base

# Don’t run as root
RUN adduser --disabled-password \
    --home /app \
    --gecos '' dotnetuser && chown -R dotnetuser /app && chown -R dotnetuser /tmp
USER dotnetuser
#

WORKDIR /app
COPY --from=builder /app .

ENV TZ="America/La_Paz" 
ENV COMPlus_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "OWP.NotificationService.dll"]