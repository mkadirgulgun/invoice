﻿namespace Invoice.Models;

    public class Client
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
    public ICollection<InvoiceModel>? Invoices { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string StreetAddress { get; set; }
    public string City { get; set; }
    public string PostCode { get; set; }
    public string Country { get; set; }
}

