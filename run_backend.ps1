$composeStatus = docker-compose ps --services --filter "status=running"

if (-not $composeStatus) {
    Write-Output "Docker services are not running. Starting Docker Compose..."
    docker-compose up -d
}

dotnet run --project ./server/api