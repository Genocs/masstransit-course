using Genocs.MassTransit.Warehouse.Contracts;
using MassTransit;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Warehouse.Components.Consumers
{
    /// <summary>
    /// This consumer handle the event coming from the request to allocate the Inventory 
    /// </summary>
    public class AllocateInventoryConsumer : IConsumer<AllocateInventory>
    {
        public async Task Consume(ConsumeContext<AllocateInventory> context)
        {
            // Publish the Event handled by the AllocationStateMachine
            // The state machine will keep the allocation 
            await context.Publish<AllocationCreated>(new
            {
                context.Message.AllocationId,
                HoldDuration = 8000, // 8 seconds
            });  

            // Respond to the client that the allocation went well 
            await context.RespondAsync<InventoryAllocated>(new
            {
                context.Message.AllocationId,
                context.Message.ItemNumber,
                context.Message.Quantity
            });
        }
    }
}
