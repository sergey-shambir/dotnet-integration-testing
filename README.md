# Статус

ПРОЕКТ НЕ ЗАКОНЧЕН, СТАТЬЯ НЕ НАПИСАНА. Планирую сделать это до 17 ноября 2024.

# Сборка и запуск сервиса

Для сборки нужен .NET 8 или выше:

```bash 
docker-compose up -d
dotnet build
dotnet run --project src/WebService/
```

Затем откройте в браузере: http://localhost:8120/swagger/index.html
