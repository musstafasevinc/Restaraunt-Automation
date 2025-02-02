using RestoranOtomasyonu.Models;

namespace RestoranOtomasyonu.Extensions
{
    public static class OrderExtensions
    {
        public static string GetDisplayName(this OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Preparing => "Hazırlanıyor",
                OrderStatus.Ready => "Hazır",
                OrderStatus.Completed => "Tamamlandı",
                _ => status.ToString()
            };
        }
    }
} 