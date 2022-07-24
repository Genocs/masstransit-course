using MassTransit;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public class DeliveryOrderActivity :
        IActivity<DeliveryOrderArguments, DeliveryOrderLog>
    {
        static readonly Random _random = new Random();

        public async Task<ExecutionResult> Execute(ExecuteContext<DeliveryOrderArguments> context)
        {
            string address = context.Arguments.Address;
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));

            await Task.Delay(_random.Next(10000));

            if (address.StartsWith("via"))
            {
                throw new InvalidOperationException("via address cannot be used as a valid address.");
            }

            return context.Completed(new { ShippingCode = "99999999" });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<DeliveryOrderLog> context)
        {
            await Task.Delay(100);

            return context.Compensated();
        }
    }
}
