
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Dto
{
    public class LinksDto
    {

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}