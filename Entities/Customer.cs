namespace WebApplication1.Entities
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string Name { get; set; } = null!;

        public string? Address1 { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; } = null!;

        public string? ProvinceOrState { get; set; } = null!;

        public string? ZipOrPostalCode { get; set; } = null!;

        public string? Phone { get; set; }

        public string? ContactLastName { get; set; }

        public string? ContactFirstName { get; set; }

        public string? ContactEmail { get; set; }

        public bool IsDeleted { get; set; } = false;

        // Navigation property because A customer has many invoices
        public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    }
}
