using MassTransit;
using MongoDB.Driver;
using Shared.Queues;
using StockAPI.Consumers;
using StockAPI.Models.Entites;
using StockAPI.Services;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<OrderCreatedEventConsumer>();
            configurator.UsingRabbitMq((context, _configurator) =>
            {
                _configurator.Host(builder.Configuration["RabbitMQ"]);
                _configurator.ReceiveEndpoint(RabbitMQSettings.Stock_OrderCreatedQueue, e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
            });
        });

        builder.Services.AddSingleton<MongoDBService>();

        var app = builder.Build();

        // Seed data metodunu burada çaðýrýyoruz
        await SeedData(app);

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


        // Seed data ekleme iþlemini ayrý bir async metot içine taþýyoruz
        async Task SeedData(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var mongoDBService = scope.ServiceProvider.GetRequiredService<MongoDBService>();
            var collection = mongoDBService.GetCollection<Stock>();

            if (!collection.FindSync(Builders<Stock>.Filter.Empty).Any())
            {
                // ... Mevcut seed data kodunuz
                await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 2000 });
                await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 1000 });
                await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 3000 });
                await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 800 });
                await collection.InsertOneAsync(new() { ProductId = Guid.NewGuid(), Count = 500 });
            }
        }
    }
}