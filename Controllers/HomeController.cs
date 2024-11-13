using Invoice.Data;
using Invoice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Invoice.Controllers
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
            var invoices = _context.Invoices
             .Include(x => x.Client)
             .Include(x => x.Items)
             .Where(x => x.IsDeleted == false)
             .OrderBy(x => x.Id)
             .ToList();
            ViewBag.Users = _context.Users.First();
            return View(invoices);
        }
        public IActionResult Details()
        {
            return View();
        }

        public IActionResult Edit() 
        { 
            return View(); 
        }
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Add(InvoiceModel model)
        {

            model.InvoiceName = GenerateInvoiceName();
            
            _context.Invoices.Add(model);
            _context.SaveChanges();
            return View();
        }
        public string GenerateInvoiceName()
        {
            Random random = new Random();

            char firstLetter = (char)random.Next('A', 'Z' + 1);
            char secondLetter = (char)random.Next('A', 'Z' + 1);
            int number = random.Next(1000, 9999);
            var invoiceName = _context.Invoices.FirstOrDefault(x => x.InvoiceName == $"#{firstLetter}{secondLetter}{number}");
            if (invoiceName != null)
            {
                char firstLetterAgain = (char)random.Next('A', 'Z' + 1);
                char secondLetterAgain = (char)random.Next('A', 'Z' + 1);
                int numberAgain = random.Next(1000, 9999);

                return $"#{firstLetter}{secondLetter}{number}";
            }
            else
            {
                return $"#{firstLetter}{secondLetter}{number}";
            }
        }

    }
}
