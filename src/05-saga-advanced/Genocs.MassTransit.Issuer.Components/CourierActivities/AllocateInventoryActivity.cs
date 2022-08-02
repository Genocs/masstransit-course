using Genocs.MassTransit.Warehouse.Contracts;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public class AllocateInventoryActivity :
            IActivity<AllocateInventoryArguments, AllocateInventoryLog>
    {
        readonly IRequestClient<AllocateInventory> _client;

        public AllocateInventoryActivity(IRequestClient<AllocateInventory> client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<ExecutionResult> Execute(ExecuteContext<AllocateInventoryArguments> context)
        {
            var orderId = context.Arguments.OrderId;
            // Some business logic checks
            var itemNumber = context.Arguments.ItemNumber;
            if (string.IsNullOrEmpty(itemNumber))
                throw new ArgumentNullException(nameof(itemNumber));

            var quantity = context.Arguments.Quantity;
            if (quantity <= 0.0m)
                throw new ArgumentNullException(nameof(quantity));

            // Create the allocation inventory Id
            var allocationId = NewId.NextGuid();

            // Allocate the inventory
            // The inventory remain allocated for 8 seconds
            var response = await _client.GetResponse<InventoryAllocated>(new
            {
                AllocationId = allocationId,
                ItemNumber = itemNumber,
                Quantity = quantity
            });
            //throw new System.InvalidOperationException("Simulate error");

            // Delay above the 8 sec will create a hold Expiration
            await Task.Delay(7000);

            // Complete the allocation on the inventory
            await context.Publish<AllocationConfirmed>(new
            {
                AllocationId = allocationId
            });

            // Everything went fine so can complete the request
            return context.Completed(new { AllocationId = allocationId });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<AllocateInventoryLog> context)
        {
            await context.Publish<AllocationReleaseRequested>(new
            {
                context.Log.AllocationId,
                Reason = "Order Faulted"
            });

            return context.Compensated();
        }
    }
}
