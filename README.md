# Интеграционные тесты для ASP.NET Core

- В ветке `main` есть и тестируемая система, и тесты
- В ветке `baseline` есть только тестируемая система — можно переключиться на неё, чтобы писать тесты по статье

# Сборка и запуск сервиса

Для сборки нужен .NET 8 или выше:

```bash 
docker-compose up -d
dotnet build
dotnet run --project src/WebService/
```

Затем откройте в браузере: http://localhost:8120/swagger/index.html
