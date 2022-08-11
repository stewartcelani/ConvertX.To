FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["src/Presentation/ConvertX.To.API/ConvertX.To.API.csproj", "src/Presentation/ConvertX.To.API/"]
COPY ["src/Presentation/ConvertX.To.API.Contracts/ConvertX.To.API.Contracts.csproj", "src/Presentation/ConvertX.To.API.Contracts/"]
COPY ["src/Core/ConvertX.To.Domain/ConvertX.To.Domain.csproj", "src/Core/ConvertX.To.Domain/"]
COPY ["src/Core/ConvertX.To.Application/ConvertX.To.Application.csproj", "src/Core/ConvertX.To.Application/"]
COPY ["src/Infrastructure/ConvertX.To.Infrastructure.Shared/ConvertX.To.Infrastructure.Shared.csproj", "src/Infrastructure/ConvertX.To.Infrastructure.Shared/"]
COPY ["src/Infrastructure/ConvertX.To.Infrastructure.Persistence/ConvertX.To.Infrastructure.Persistence.csproj", "src/Infrastructure/ConvertX.To.Infrastructure.Persistence/"]
RUN dotnet restore "src/Presentation/ConvertX.To.API/ConvertX.To.API.csproj"
COPY . .
WORKDIR "src/Presentation/ConvertX.To.API"
RUN dotnet build "ConvertX.To.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ConvertX.To.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConvertX.To.API.dll"]
