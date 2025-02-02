using System.ComponentModel.DataAnnotations;

namespace RestoranOtomasyonu.Models
{
    public class Table
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Masa numarası zorunludur")]
        [Display(Name = "Masa Numarası")]
        public int Number { get; set; }

        [Display(Name = "Durum")]
        public bool IsOccupied { get; set; }

        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
} 