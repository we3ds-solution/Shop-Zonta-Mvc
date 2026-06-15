using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using market_mvc.Data;
using market_mvc.Domain.entity;

namespace market_mvc.Controllers
{
    public class ItemsController : Controller
    {
        private readonly AppDbContext _context;

        public ItemsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Details(Guid id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            return View(item); 
        }
        public IActionResult Index()
        {
            IEnumerable<Item> list = _context.Items.ToList();
            return View(list);
        }
        [HttpGet]
        public IActionResult Create()
        {
            
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Item item)
        {
            if (!ModelState.IsValid)
            {
                return View(item);
            }
            else
            {
                _context.Items.Add(item);
                _context.SaveChanges();
            return RedirectToAction("Index");
            }
        }
        public IActionResult Edit()
        {
            IEnumerable<Item> list = _context.Items.ToList();
            return View(list);
        }
        public IActionResult Delete()
        {
            IEnumerable<Item> list = _context.Items.ToList();
            return View(list);
        }
    }
}

