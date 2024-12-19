ARG CI_REGISTRY_IMAGE
FROM $CI_REGISTRY_IMAGE:builder AS deps
RUN mkdir -p /src/packages

FROM hub.nexdev.net/base/dotnet/core/sdk:9.0-alpine AS builder

ENV NUGET_PACKAGES=/src/packages
COPY --from=deps $NUGET_PACKAGES/. $NUGET_PACKAGES/

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
RUN dotnet restore -v n -r linux-x64

COPY ./OWP.NotificationService .
RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app

FROM hub.nexdev.net/base/dotnet/core/aspnet:9.0-alpine AS base

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