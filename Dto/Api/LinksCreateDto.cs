using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Selflink_api.Dto
{
    public class LinksCreateDto
    {

        [Required, MinLength(1), MaxLength(100), NotNull]
        public string Name { get; set; }

        [Required, MinLength(1), MaxLength(100), NotNull]
        public string ProductName {get; set; }

        [Required, NotNull]
        public List<string> ShippingCountries {get; set;}

        [Required, NotNull]
        public string PriceUnit {get; set;}

        [Required, NotNull]
        public string QuantityStock {get; set;}

        [Required, NotNull]
        public string Iban {get; set;}

        [Required, MaxLength(150), NotNull]
        public string Description {get; set;}
        
        [Required, NotNull]
        public List<string> ProductImage {get; set;}

        [Required, NotNull]
        public string Currency {get; set;}

    }
}