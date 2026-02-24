using TaskManager.Api.Extensions;
using TaskManager.Api.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 🎯 DI регистрация
builder.Services.AddSingleton<ITaskStore, InMemoryTaskStore>();

// Services
builder.Services.AddSingleton<InMemoryTaskStore>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(opt => opt.AddPolicy("Vue", p => p
    .WithOrigins("http://localhost:5173")
    .AllowAnyMethod()
    .AllowAnyHeader()));

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("Vue");
app.UseHttpsRedirection();

// 🎯 Endpoints через extension
app.MapTaskEndpoints();

app.Run();