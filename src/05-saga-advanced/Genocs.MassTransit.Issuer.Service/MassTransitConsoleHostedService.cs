using MassTransit;

namespace Genocs.MassTransit.Service
{
    public class MassTransitConsoleHostedService :
        IHostedService
    {
        //readonly IBusControl _bus;

        public MassTransitConsoleHostedService(IBusControl bus)
        {
            //_bus = bus;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            //await _bus.StartAsync(cancellationToken).ConfigureAwait(false);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            //return _bus.StopAsync(cancellationToken);
        }
    }
}