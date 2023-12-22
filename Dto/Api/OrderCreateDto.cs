namespace Selflink_api.Dto;

public class OrderCreateDto 
{
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

    public string Currency { get; set;}

    public string Status {get;set;} // sent, refund, pending
}