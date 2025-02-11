# Runtime stage: Use a lightweight Linux runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0-alpine AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8585

# Build stage: Use a Windows-based SDK image
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the source code
COPY src/UICrafter/ UICrafter/
COPY src/UICrafter.Client/ UICrafter.Client/
COPY src/UICrafter.Core/ UICrafter.Core/

# Restore dependencies
WORKDIR /src/UICrafter
RUN dotnet restore

# Build and publish the application for Linux
RUN dotnet publish -c Release -r linux-musl-x64 -o /app/publish

# Final stage: Copy the output to the Linux runtime image
FROM runtime AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "UICrafter.dll"]
