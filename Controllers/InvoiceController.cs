using Invoice.Data;
using Invoice.DTOs;
using Invoice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Invoice.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InvoiceController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult InvoiceDetail(int id)
        {
            var invoice = _context.Invoices
            .Include(x => x.Client)
            .Include(x => x.Items.Where(item => !item.IsDeleted))
            .FirstOrDefault(x => x.Id == id);

            if (invoice == null)
            {
                return NotFound("Fatura bulunamadı.");
            }

            ViewBag.Users = _context.Users.First();
            return View(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice(dtoAddInvoice model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(model);

            }
            var client = await _context.Clients
                    .FirstOrDefaultAsync(c => c.Name == model.ClientName);

            if (client == null)
            {
                client = new Client
                {
                    Name = model.ClientName,
                    Email = model.ClientEmail,
                    Address = model.ClientStreet,
                    City = model.ClientCity,
                    PostCode = model.ClientPostCode,
                    Country = model.ClientCountry
                };
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
            }

            var invoice = new InvoiceModel
            {
                InvoiceName = GenerateInvoiceName(),
                InvoiceDate = model.InvoiceDate,
                PaymentDue = CalculatePaymentDue(model.InvoiceDate, model.PaymentTerm),
                PaymentStatus = Status.Taslak,
                ProjectDescription = model.Description,
                ClientId = client.Id,
                TotalAmount = model.Items.Sum(x => x.Quantity * x.Price),
            };

            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            foreach (var item in model.Items)
            {
                var invoiceItem = new Item
                {
                    Name = item.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    TotalPrice = item.Quantity * item.Price,
                    InvoiceId = invoice.Id
                };
                _context.Items.Add(invoiceItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> EditInvoice(dtoAddInvoice model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("InvoiceDetail", new { id = model.Id });
            }

            var invoice = await _context.Invoices
                .Include(i => i.Client)
                .Include(i => i.Items)
                .FirstOrDefaultAsync(i => i.Id == model.Id);

            if (invoice == null)
            {
                return NotFound();
            }

            // Update invoice properties
            invoice.ProjectDescription = model.Description;
            invoice.InvoiceDate = model.InvoiceDate;
            invoice.PaymentDue = CalculatePaymentDue(model.InvoiceDate, model.PaymentTerm);
            invoice.PaymentStatus = model.PaymentStatus;

            // Update client properties
            invoice.Client.Name = model.ClientName;
            invoice.Client.Email = model.ClientEmail;
            invoice.Client.Address = model.ClientStreet;
            invoice.Client.City = model.ClientCity;
            invoice.Client.PostCode = model.ClientPostCode;
            invoice.Client.Country = model.ClientCountry;

            // Update items
            var itemsToRemove = invoice.Items.Where(i => !model.Items.Any(mi => mi.Id == i.Id)).ToList();
            foreach (var item in itemsToRemove)
            {
                item.IsDeleted = true;
            }

            foreach (var item in model.Items)
            {
                var existingItem = invoice.Items.FirstOrDefault(i => i.Id == item.Id);
                if (existingItem != null)
                {
                    // Mevcut öğeyi güncelle
                    existingItem.Name = item.Name;
                    existingItem.Quantity = item.Quantity;
                    existingItem.Price = item.Price;
                    existingItem.TotalPrice = item.Price * item.Quantity;
                }
                else
                {
                    // Yeni bir öğe ekle
                    invoice.Items.Add(new Item
                    {
                        Name = item.Name,
                        Quantity = item.Quantity,
                        Price = item.Price,
                        TotalPrice = item.Price*item.Quantity
                    });
                }
            }
            invoice.TotalAmount = model.Items.Sum(x => x.Quantity * x.Price);
            // Update user address (assuming it's stored in a separate table)
            var user = await _context.Users.FirstAsync();
            if (user != null)
            {
                user.StreetAddress = model.StreetAddress;
                user.City = model.City;
                user.PostCode = model.PostCode;
                user.Country = model.Country;
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("InvoiceDetail", new { id = invoice.Id });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            item.IsDeleted = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FirstOrDefaultAsync(x => x.Id == id);

            if (invoice == null)
            {
                return NotFound("Fatura bulunamadı.");
            }

            invoice.IsDeleted = true;
            invoice.PaymentStatus = Status.Silindi;
            _context.Invoices.Update(invoice);
            _context.SaveChanges();
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsPaid(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return Json(new { success = false });
            }

            invoice.PaymentStatus = Status.Ödendi;
            _context.Update(invoice);
            await _context.SaveChangesAsync();

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return Json(new { success = true });
            }

            return RedirectToAction(nameof(InvoiceDetail), new { id = invoice.Id });
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

        public DateTime CalculatePaymentDue(DateTime createdTime, PaymentTerm paymentTerm)
        {
            if (paymentTerm == PaymentTerm.Net1Day)
            {
                return createdTime.AddDays(1);
            }
            else if (paymentTerm == PaymentTerm.Net7Day)
            {
                return createdTime.AddDays(7);

            }
            else if (paymentTerm == PaymentTerm.Net14Day)
            {
                return createdTime.AddDays(14);
            }
            else
            {
                return createdTime.AddDays(30);
            }
        }
    }
}
