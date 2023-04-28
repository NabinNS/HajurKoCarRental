using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurKoCarRental.Models
{
    public class Damage
    {
        [Key] public int DamageID { get; set; }
        [ForeignKey("Id")] public string UserID { get; set; }
        [ForeignKey("CarID")] public int CarID { get; set; }
        [ForeignKey("RentalId")] public int RentalID { get; set; }
        public string? DamageDescription { get; set; }
        public string? DamageType { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual RentalRequest RentalRequest { get; set; }
    }
}
