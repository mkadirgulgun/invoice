using Invoice.Models;

namespace Invoice.DTOs
{
   
    public class dtoAddInvoice
    {
        public  int? Id { get; set; }
        public DateTime InvoiceDate { get; set; }
        public PaymentTerm PaymentTerm { get; set; }
        public double TotalAmount { get; set; }
        public string Description { get; set; }
        public Status PaymentStatus { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }

        public string ClientName { get; set; }
        public string ClientEmail { get; set; }
        public string ClientStreet { get; set; }
        public string ClientCity { get; set; }
        public string ClientPostCode { get; set; }
        public string ClientCountry { get; set; }

        public List<InvoiceItem>? Items { get; set; }
    }

    public class InvoiceItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public Double Price { get; set; }
        public bool IsDeleted { get; set; }
    }

}
