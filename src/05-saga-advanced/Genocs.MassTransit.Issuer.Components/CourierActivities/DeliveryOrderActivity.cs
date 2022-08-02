using MassTransit;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public class DeliveryOrderActivity :
        IActivity<DeliveryOrderArguments, DeliveryOrderLog>
    {
        //static readonly Random _random = new Random();

        public async Task<ExecutionResult> Execute(ExecuteContext<DeliveryOrderArguments> context)
        {
            string shippingAddress = context.Arguments.ShippingAddress;
            if (string.IsNullOrEmpty(shippingAddress))
                throw new ArgumentNullException(nameof(shippingAddress));

            //await Task.Delay(_random.Next(10000));

            if (shippingAddress.StartsWith("via"))
            {
                throw new InvalidOperationException("'via' cannot be used as a valid shippingAddress.");
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
