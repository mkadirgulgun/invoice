using Invoice.Data;
using Invoice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet("/Client/{FullName}")]
        public List<Client> GetClient(string FullName)
        {
            var clients = _context.Clients
                         .Where(x => x.Name.Contains(FullName))
                         .ToList();

            return clients;
        }

    }
}
