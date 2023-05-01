using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HajurKoCarRental.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public bool IsRegular { get; set; }
        public int Discount { get; set; }
        public bool IsActive { get; set; }
        public string? DrivingLicenseURL { get; set; }
        public string? CitizenshipURL { get; set; }
        public DateTime? LastActive { get; set; }
        public bool? PaymentDue { get; set; }
        public bool? Verified { get; set; }
        public ICollection<RentalRequest> RentalRequests { get; set; }
        public ICollection<Damage> Damages { get; set; }
    }
}
