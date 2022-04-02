using MassTransit.Courier;
using System;
using System.Threading.Tasks;

namespace Genocs.MassTransit.Components.CourierActivities
{
    public class IssueCardActivity :
        IActivity<IssueCardArguments, IssueCardLog>
    {
        static readonly Random _random = new Random();

        public async Task<ExecutionResult> Execute(ExecuteContext<IssueCardArguments> context)
        {
            string cardNumber = context.Arguments.CardNumber;
            if (string.IsNullOrEmpty(cardNumber))
                throw new ArgumentNullException(nameof(cardNumber));

            await Task.Delay(1000);
            await Task.Delay(_random.Next(10000));

            if (cardNumber.StartsWith("5999"))
            {
                throw new InvalidOperationException("The card number was invalid");
            }

            return context.Completed(new { AuthorizationCode = "77777" });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<IssueCardLog> context)
        {
            await Task.Delay(100);

            return context.Compensated();
        }
    }
}
