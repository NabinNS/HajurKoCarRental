using System.ComponentModel.DataAnnotations;
using System;

namespace HajurKoCarRental.Models
{
    public class Car
    {
        [Key] public int CarID { get; set; }

        [Required(ErrorMessage = "Manufacturer is required")]
        public string Manufacturer { get; set; }

        [Required(ErrorMessage = "Model is required")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Rental rate is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Rental rate must be greater than 0")]
        public decimal RentalRate { get; set; }

        [Required(ErrorMessage = "Vehicle number is required")]
        public string VehicleNo { get; set; }
        public bool IsAvailable { get; set; }
        public string? CarImageUrl { get; set; }
        public ICollection<Offer>? Offers { get; set; }
        public ICollection<RentalRequest>? RentalRequests { get; set; }
        public ICollection<Damage>? Damages { get; set; }
    }
}
