using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderAPI.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<OrderAPIDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddMassTransit(configurator =>
    configurator.UsingRabbitMq((context, _configurator) =>
    { 
        _configurator.Host(builder.Configuration["RabbitMQ"]);
    }));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // D�ng�leri g�rmezden gelmesini sa�lar.
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
