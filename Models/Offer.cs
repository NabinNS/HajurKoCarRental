using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HajurKoCarRental.Models
{
    public class Offer
    {

        [Key] public int OfferID { get; set; }
        [ForeignKey("CarID")] public int CarID { get; set; }
        public decimal DiscountRate { get; set; }
        public string? OfferDescription { get; set; }

        //Relationship
        public virtual Car Car { get; set; }

    }
   
}
