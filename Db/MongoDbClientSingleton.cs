using MongoDB.Driver;

namespace Selflink_api.Db
{
    public class MongoDbClientSingleton: IMongoDbClientSingleton
    {

        public IMongoDatabase Database;
        public MongoClient Client;

        public MongoDbClientSingleton()
        {
            string connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING")!;
            string databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME")!;

            Client = new MongoClient(connectionString);
            Database = Client.GetDatabase(databaseName);
        }

        public IMongoDatabase GetDatabase()
        {
            return Database;
        }

        public MongoClient GetClient()
        {
            return Client;
        }

        public string GetLinkCollectionName()
        {
            return "links";
        }

        public string GetOrderCollectionName()
        {
            return "orders";
        }
    }
}