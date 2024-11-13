using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Invoice.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int Quantity { get; set; }
        public Double Price { get; set; }
        [JsonIgnore]
        public Double TotalPrice { get; set; }
        public int InvoiceId { get; set; }
        [ForeignKey("InvoiceId")]
        public InvoiceModel Invoice { get; set; }

    }
}
