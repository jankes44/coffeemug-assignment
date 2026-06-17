# syntax=docker/dockerfile:1.7
FROM mcr.microsoft.com/dotnet/sdk:10.0-preview AS build
WORKDIR /src

# Copy csproj files first for better layer caching
COPY CoffeeMugAssignment.sln ./
COPY CoffeeMugAssignment/CoffeeMugAssignment.csproj                                 CoffeeMugAssignment/
COPY CoffeeMugAssignment.Application/CoffeeMugAssignment.Application.csproj         CoffeeMugAssignment.Application/
COPY CoffeeMugAssignment.Domain/CoffeeMugAssignment.Domain.csproj                   CoffeeMugAssignment.Domain/
COPY CoffeeMugAssignment.Infrastructure/CoffeeMugAssignment.Infrastructure.csproj   CoffeeMugAssignment.Infrastructure/

RUN dotnet restore CoffeeMugAssignment/CoffeeMugAssignment.csproj

# Copy the rest and publish
COPY . .
RUN dotnet publish CoffeeMugAssignment/CoffeeMugAssignment.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:10.0-preview AS runtime
WORKDIR /app
COPY --from=build /app/publish ./
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "CoffeeMugAssignment.dll"]
