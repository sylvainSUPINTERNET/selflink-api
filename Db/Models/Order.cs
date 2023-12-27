
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Db.Models
{
    [BsonIgnoreExtraElements]
    public class Order
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("stripeProductId")]
        public string StripeProductId { get; set; }

        [BsonElement("prductName")]
        public string ProductName { get; set;}

        [BsonElement("stripePriceId")]
        public string StripePriceId { get; set; }

        [BsonElement("stripePaymentIntentId")]
        public string StripePaymentIntentId { get; set; }

        [BsonElement("phone")]
        public string Phone { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("shippingLine1")]
        public string ShippingLine1 { get; set; }

        [BsonElement("shippingLine2")]
        public string ShippingLine2 { get; set; }

        [BsonElement("shippingCity")]
        public string ShippingCity { get; set; }

        [BsonElement("shippingPostalCode")]
        public string ShippingPostalCode { get; set; }

        [BsonElement("shippingState")]
        public string ShippingState { get; set; }

        [BsonElement("shippingCountry")]
        public string ShippingCountry { get; set; }

        [BsonElement("quantityToSend")]
        public string QuantityToSend { get; set; }

        [BsonElement("amount")]
        public string Amount { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set;} // eur
 
        [BsonElement("status")]
        public string Status {get;set;} // sent, refunded, pending


        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; private set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; private set; }

        public Order () 
        {
            CreatedAt = DateTime.UtcNow;
        }


    }
}