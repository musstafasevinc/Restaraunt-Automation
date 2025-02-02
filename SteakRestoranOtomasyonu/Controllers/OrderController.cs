using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoranOtomasyonu.Data;
using RestoranOtomasyonu.Models;

namespace RestoranOtomasyonu.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.Table)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Status != OrderStatus.Completed)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
            return View(orders);
        }

        public async Task<IActionResult> Details(int? tableId)
        {
            if (tableId == null)
            {
                return NotFound();
            }

            var table = await _context.Tables
                .Include(t => t.Orders.Where(o => o.Status != OrderStatus.Completed))
                    .ThenInclude(o => o.OrderItems)
                        .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(t => t.Id == tableId);

            if (table == null)
            {
                return NotFound();
            }

            var activeOrder = table.Orders.FirstOrDefault();

            // Eğer masada aktif sipariş yoksa ve yeni sipariş butonu tıklanmışsa
            if (activeOrder == null && Request.Query.ContainsKey("newOrder"))
            {
                activeOrder = new Order
                {
                    TableId = table.Id,
                    OrderDate = DateTime.Now,
                    Status = OrderStatus.Preparing
                };
                _context.Orders.Add(activeOrder);
                table.IsOccupied = true;
                await _context.SaveChangesAsync();
            }

            ViewBag.Products = await _context.Products
                .Include(p => p.Category)
                .OrderBy(p => p.Category.Name)
                .ToListAsync();

            return View(activeOrder);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem(int orderId, int productId, int quantity)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            var existingItem = order.OrderItems
                .FirstOrDefault(oi => oi.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                order.OrderItems.Add(new OrderItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    UnitPrice = product.Price
                });
            }

            order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
            order.Table.IsOccupied = true;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Details), new { tableId = order.TableId });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveItem(int orderId, int orderItemId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            var orderItem = order.OrderItems
                .FirstOrDefault(oi => oi.Id == orderItemId);

            if (orderItem != null)
            {
                order.OrderItems.Remove(orderItem);
                order.TotalAmount = order.OrderItems.Sum(oi => oi.Quantity * oi.UnitPrice);
                
                // Eğer son ürün de silindiyse ve sipariş hazırlanıyor durumundaysa
                if (!order.OrderItems.Any() && order.Status == OrderStatus.Preparing)
                {
                    _context.Orders.Remove(order);
                    order.Table.IsOccupied = false;
                }
                
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { tableId = order.TableId });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int orderId, OrderStatus status)
        {
            var order = await _context.Orders
                .Include(o => o.Table)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                return NotFound();
            }

            order.Status = status;

            // Sipariş tamamlandıysa
            if (status == OrderStatus.Completed)
            {
                // Masanın başka aktif siparişi var mı kontrol et
                var hasActiveOrders = await _context.Orders
                    .Where(o => o.TableId == order.TableId && 
                               o.Id != order.Id && 
                               o.Status != OrderStatus.Completed)
                    .AnyAsync();
                
                // Başka aktif sipariş yoksa masayı boşalt
                order.Table.IsOccupied = hasActiveOrders;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
    }
} 