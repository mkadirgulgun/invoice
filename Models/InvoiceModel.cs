﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Invoice.Models
{
    public class InvoiceModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string InvoiceName { get; set; }
        public string? ProjectDescription { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime PaymentDue { get; set; }
        public Status PaymentStatus { get; set; } = Status.Taslak;
        public Double TotalAmount { get; set; }
        [NotMapped]
        public PaymentTerm PaymentTerm { get; set; }
        public int ClientId { get; set; }

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }
        public ICollection<Item> Items { get; set; } = new List<Item>();
        public bool IsDeleted { get; set; } = false;
        [NotMapped]
        [JsonIgnore]
        public User? User { get; set; }
    }
    public enum PaymentTerm
    {
        Net1Day,
        Net7Day,
        Net14Day,
        Net30Day
    }

    public enum Status
    {
        Beklemede,
        Ödendi,
        Taslak,
        Silindi
    }
}  