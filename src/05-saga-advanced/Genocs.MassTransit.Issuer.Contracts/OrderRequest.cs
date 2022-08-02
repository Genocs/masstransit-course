using System;

namespace Genocs.MassTransit.Contracts
{
    /// <summary>
    /// The Entry point order request used to push the message to the ESB
    /// </summary>
    public interface OrderRequest
    {
        /// <summary>
        /// The OrderId unique identier
        /// </summary>
        Guid OrderId { get; }

        /// <summary>
        /// The Unique identifier about the Customer
        /// </summary>
        string CustomerNumber { get; }

        /// <summary>
        /// The payment card.
        /// For PCI DSS requirement this should be the public token 
        /// used to retrieve the Actual credit card secrets stored in 
        /// safe place
        /// </summary>
        string PaymentCardNumber { get; }

        /// <summary>
        /// Where the order will delivered 
        /// </summary>
        string ShippingAddress { get; }

        DateTime Timestamp { get; }
    }
}
