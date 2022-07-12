FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

RUN echo "$Mode"

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ConvertX.To.API.csproj", "./"]
RUN dotnet restore "ConvertX.To.API.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "ConvertX.To.API.csproj" -c $Mode -o /app/build

FROM build AS publish
RUN dotnet publish "ConvertX.To.API.csproj" -c $Mode -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ConvertX.To.API.dll"]
