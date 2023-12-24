using MongoDB.Driver;

namespace Selflink_api.Db
{
    public interface IMongoDbClientSingleton
    {
        public IMongoDatabase GetDatabase();
        
        public MongoClient GetClient();

        public string GetLinkCollectionName();

        public string GetOrderCollectionName();
    }

}