
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Db.Models
{
    public class Link
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }
    }
}