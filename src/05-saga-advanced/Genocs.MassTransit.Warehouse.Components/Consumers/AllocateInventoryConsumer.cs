using Genocs.MassTransit.Warehouse.Contracts;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Warehouse.Components.Consumers
{
    public class AllocateInventoryConsumer :
        IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
            await context.Publish<AllocationCreated>(new
            {
                context.Message.AllocationId,
                HoldDuration = 15000,
            });

            //throw new InvalidOperationException("Non Va bene");

            await context.RespondAsync<InventoryAllocated>(new
            {
                context.Message.AllocationId,
                context.Message.ItemNumber,
                context.Message.Quantity
            });
        }
    }
}
