namespace Genocs.MassTransit.Customers.Contracts
{
    public class Customer
    {
        public string CustomerNumber { get; set; }

        public bool Active { get; set; }

        public DateTime Date { get; set; }
    }
}