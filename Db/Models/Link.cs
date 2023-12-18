
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Db.Models
{
    public class Link
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; private set; }


        // represent the user id in the token
        public string Sub { get; set;}

        public string Iban { get; set; }

        public string PaymentUrl { get; set; }

        public string StripeId { get; set; }


        public Link () 
        {
            CreatedAt = DateTime.UtcNow;
        }


    }
}