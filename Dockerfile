FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS publish
WORKDIR /src

COPY . .
RUN dotnet restore "./DDNS/DDNS.csproj"

RUN dotnet publish "./DDNS/DDNS.csproj" -c Release -o /app/publish --no-restore --self-contained -r linux-musl-x64

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-alpine AS final
WORKDIR /app

COPY --from=publish /app/publish .

RUN adduser -u 5678 --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["./DDNS"]
