using Microsoft.EntityFrameworkCore;
using WebService.Database;

var builder = WebApplication.CreateBuilder(args);

string? connectionString = builder.Configuration.GetConnectionString("MainConnection");

builder.Services.AddControllers();
builder.Services.AddDbContext<WarehouseDbContext>(
    options => options.UseNpgsql(connectionString)
);

// Добавляем экспорт описания API в формате Swagger на странице /swagger
// Подробнее о Swagger/OpenAPI см. https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Выполняем миграции
using (IServiceScope scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<WarehouseDbContext>();
    await dbContext.Database.MigrateAsync();
}

// Конфигурируем конвейер обработки HTTP запросов.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.MapControllers();

app.Run();

public partial class Program { }