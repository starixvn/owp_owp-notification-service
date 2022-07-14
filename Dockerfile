FROM hub.nexdev.net/base/dotnet/core/sdk:6.0 AS builder

# Install the tools
RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-dump
RUN dotnet tool install --tool-path /tools dotnet-counters
#
WORKDIR /src

COPY ./src/OWP.MessageConsumer/. ./src/OWP.MessageConsumer
COPY ./src/OWP.Core/. src/OWP.Core
COPY ./src/OWP.TicketNotification/. ./src/OWP.TicketNotification
COPY ./src/OWP.PreMatchNotification/. ./src/OWP.PreMatchNotification
COPY ./src/OWP.OtherSportNotification/. ./src/OWP.OtherSportNotification
COPY ./OWP.NotificationService/OWP.NotificationService.csproj ./OWP.NotificationService/OWP.NotificationService.csproj

WORKDIR /src/OWP.NotificationService
RUN dotnet nuget add source http://nuget.nexdev.net/api/v2
RUN dotnet restore -v diag

COPY ./OWP.NotificationService .

RUN dotnet publish -r linux-x64 --self-contained false -c Release -o /app 

FROM hub.nexdev.net/base/dotnet/core/aspnet:6.0 AS base

RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /etc/ssl/openssl.cnf
RUN sed -i 's/DEFAULT@SECLEVEL=2/DEFAULT@SECLEVEL=1/g' /usr/lib/ssl/openssl.cnf
RUN sed -i 's/MinProtocol = TLSv1.2/MinProtocol = TLSv1/g' /usr/lib/ssl/openssl.cnf

WORKDIR /app
COPY --from=builder /app .
COPY --from=builder /tools /tools

ENV TZ="America/La_Paz" 

ENTRYPOINT ["dotnet", "OWP.NotificationService.dll"]