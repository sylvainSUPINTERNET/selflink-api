
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Db.Models
{
    [BsonIgnoreExtraElements]
    public class Link
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; private set; }


        // represent the user id in the token
        [BsonElement("sub")]
        public string Sub { get; set;} // when user is logged in with OAuth2 google, this value is filled

        [BsonElement("iban")]
        public string Iban { get; set; }


        [BsonElement("paymentUrl")]
        public string PaymentUrl { get; set; }

        [BsonElement("stripeProductId")]
        public string StripeProductId { get; set; }

        [BsonElement("stripeLinkId")]
        public string StripeLinkId { get; set; }

        [BsonElement("stripePriceId")]
        public string StripePriceId { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("providerIssuer")]
        public string ProviderIssuer { get; set; }


        [BsonElement("linkUrl")]
        public string LinkUrl { get; set; }

        [BsonElement("stripeStatusActive")]
        public bool StripeStatusActive { get; set; }

        public Link () 
        {
            CreatedAt = DateTime.UtcNow;
        }


    }
}