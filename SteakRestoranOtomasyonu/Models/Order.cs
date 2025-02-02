using System.ComponentModel.DataAnnotations;

namespace RestoranOtomasyonu.Models
{
    public class Order
    {
        public int Id { get; set; }

        public int TableId { get; set; }
        public Table? Table { get; set; }

        [Display(Name = "Sipariş Tarihi")]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Display(Name = "Toplam Tutar")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Durum")]
        public OrderStatus Status { get; set; }

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }

    public enum OrderStatus
    {
        [Display(Name = "Hazırlanıyor")]
        Preparing,
        
        [Display(Name = "Hazır")]
        Ready,
        
        [Display(Name = "Tamamlandı")]
        Completed
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
} 