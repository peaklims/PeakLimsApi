FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ["PeakLims/src/PeakLims/PeakLims.csproj", "./PeakLims/src/PeakLims/"]
COPY ["SharedKernel/SharedKernel.csproj", "./SharedKernel/"]
RUN dotnet restore "./PeakLims/src/PeakLims/PeakLims.csproj"

# Copy everything else and build
COPY . ./
RUN dotnet build "PeakLims/src/PeakLims/PeakLims.csproj" -c Release -o /app/build

FROM build-env AS publish
RUN dotnet publish "PeakLims/src/PeakLims/PeakLims.csproj" -c Release -o /app/out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /app/out .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "/app/PeakLims.dll"]
