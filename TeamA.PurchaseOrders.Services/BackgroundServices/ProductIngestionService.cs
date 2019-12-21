using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamA.PurchaseOrders.Services.Interfaces;
using TeamA.PurchaseOrders.Services.Services;

namespace TeamA.PurchaseOrders.Services.BackgroundServices
{
    // Code here influenced by lee conlin's blog on recurring tasks in .net core
    public class ProductIngestionService : IHostedService, IDisposable
    {
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();
        public IServiceScopeFactory _scopeFactory { get; }

        public ProductIngestionService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This will cause the loop to stop if the service is stopped
            while (!stoppingToken.IsCancellationRequested)
            {
                // Due to this being IHostedService, it has no scope meaning we must create our own
                // Use the injected scope factory to create our scope
                using (var scope = _scopeFactory.CreateScope())
                {
                    // consume our products service through the provided scope
                    var productsService = scope.ServiceProvider.GetRequiredService<IProductsService>();
                    await productsService.GetAndSaveProducts();
                }

                // Wait 5 minutes before running again.
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                    cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }
    }
}
