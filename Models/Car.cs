using System.ComponentModel.DataAnnotations;
using System;

namespace HajurKoCarRental.Models
{
    public class Car
    {
        [Key] public int CarID { get; set; }
        public string? Manufacturer { get; set; }

        public string? Model { get; set; }
        public string? Color { get; set; }
        public decimal RentalRate { get; set; }
        public string? VehicleNo { get; set; }
        public bool IsAvailable { get; set; }
        public string? CarImageUrl { get; set; }
        public ICollection<Offer> Offers { get; set; }
        public ICollection<RentalRequest> RentalRequests { get; set; }
        public ICollection<Damage> Damages { get; set; }
        public List<RentalHistory> RentalHistories { get; set; }
    }
}
