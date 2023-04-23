
using HajurKoCarRental.Models.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HajurKoCarRental.Models
{
    public class RentalHistory
    {
        [Key] public int RentalID { get; set; }
        [ForeignKey("Id")] public string UserID { get; set; }
        [ForeignKey("CarID")] public int CarID { get; set; }
        public DateTime RentalDate { get; set; }
        [ForeignKey("Id")] public string? AuthorizedByID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual ApplicationUser AuthorizedBy { get; set; }
        public virtual RentalRequest RentalRequest { get; set; }
    }
}
