using Magazine.Core.Services;
using Magazine.WebApi;

var builder = WebApplication.CreateBuilder(args);

// Добавляем сервисы MVC и Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Получаем строку подключения из конфигурации
var configuration = builder.Configuration;
builder.Services.AddSingleton<IConfiguration>(configuration);
builder.Services.AddScoped<IProductService, ProductService>();

var app = builder.Build();

// Включаем Swagger в режиме разработки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Подключаем маршрутизацию контроллеров
app.UseAuthorization();
app.MapControllers();

app.Run();
