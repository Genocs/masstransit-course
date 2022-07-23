using System;

namespace Genocs.MassTransit.Components.HttpClients
{
    public class Customer
    {
        public string CustomerNumber { get; set; }

        public bool Active { get; set; }

        public DateTime Date { get; set; }
    }
}