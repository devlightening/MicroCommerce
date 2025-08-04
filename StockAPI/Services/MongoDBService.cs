using MongoDB.Driver;

namespace StockAPI.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _mongoDatabase;
        public MongoDBService(IConfiguration configuration)
        {
            MongoClient client = new(configuration.GetConnectionString("MongoDB"));
            _mongoDatabase = client.GetDatabase("StockAPIDb");
        }

    }
}
