
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Dto
{
    public class LinksDto
    {

        [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }

        public string GoogleOAuth2Sub { get; set; }

        public string Iban { get; set; }

        public string PaymentUrl { get; set; }

        public string StripeLinkId { get; set; }

        public string StripeProductId { get; set; }

        public string StripePriceId { get; set; }

    }
}