using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HajurKoCarRental.Models
{
    public class RentalRequest
    {
        [Key] public int ReqID { get; set; }
        [ForeignKey("Id")] public string UserID { get; set; }
        [ForeignKey("CarID")] public int? CarID { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? Status { get; set; }
        public bool? Paid { get; set; }
        public DateTime? PaymentDate { get; set; }
        public decimal? TotalAmount { get; set; }
        public string? damage { get; set; }
   
        [ForeignKey("Id")] public string? AuthorizedBy { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual Car Car { get; set; }
        public virtual ApplicationUser AuthorizedByUser { get; set; }

        public virtual ICollection<Damage> Damages { get; set; }
    }
}
