
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
        public string GoogleOAuth2Sub { get; set;} // when user is logged in with OAuth2 google, this value is filled

        public string Iban { get; set; }

        public string PaymentUrl { get; set; }

        public string StripeProductId { get; set; }

        public string StripeLinkId { get; set; }

        public string StripePriceId { get; set; }

        public Link () 
        {
            CreatedAt = DateTime.UtcNow;
        }


    }
}