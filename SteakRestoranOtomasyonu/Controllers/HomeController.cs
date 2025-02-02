using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoranOtomasyonu.Data;
using RestoranOtomasyonu.Models;

namespace RestoranOtomasyonu.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var tables = _context.Tables
                .Include(t => t.Orders.Where(o => o.Status != OrderStatus.Completed))
                .ToList();

            // Masa durumlarını aktif siparişlere göre güncelle
            foreach (var table in tables)
            {
                table.IsOccupied = table.Orders.Any();
            }
            _context.SaveChanges();

            return View(tables);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
