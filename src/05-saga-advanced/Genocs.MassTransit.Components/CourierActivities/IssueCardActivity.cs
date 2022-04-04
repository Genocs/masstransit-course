using MassTransit;
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
            string cardCurrency = context.Arguments.Currency;
            if (string.IsNullOrEmpty(cardCurrency))
                throw new ArgumentNullException(nameof(cardCurrency));

            await Task.Delay(1000);
            await Task.Delay(_random.Next(10000));

            if (cardCurrency.StartsWith("USD"))
            {
                throw new InvalidOperationException("USD currency is not handled. please use EUR");
            }

            return context.Completed(new { AuthorizationCode = "99999999" });
        }

        public async Task<CompensationResult> Compensate(CompensateContext<IssueCardLog> context)
        {
            await Task.Delay(100);

            return context.Compensated();
        }
    }
}
