
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Selflink_api.Db.Models
{
    public class Order
    {
        public ObjectId Id { get; set; }

        public string StripeProductId { get; set; }

        public string StripePriceId { get; set; }

        public string StripePaymentIntentId { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string ShippingLine1 { get; set; }

        public string ShippingLine2 { get; set; }

        public string ShippingCity { get; set; }

        public string ShippingPostalCode { get; set; }

        public string ShippingState { get; set; }

        public string ShippingCountry { get; set; }

        public string QuantityToSend { get; set; }

        public string Amount { get; set; }

        public string Currency { get; set;} // eur
 
        public string Status {get;set;} // sent, refund, pending


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