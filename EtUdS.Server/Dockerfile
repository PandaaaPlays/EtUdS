FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

COPY ./publish .

EXPOSE 5000 

RUN mkdir devoirs

ENTRYPOINT ["dotnet", "EtUdS.Server.dll"]