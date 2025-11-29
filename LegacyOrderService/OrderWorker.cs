using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LegacyOrderService.Data;

public class OrderWorker : BackgroundService
{
    private readonly ILogger<OrderWorker> _logger;
    private readonly IOrderRepository _repo;
    private readonly IProductRepository _productRepo;

    public OrderWorker(ILogger<OrderWorker> logger, IOrderRepository repo, IProductRepository productRepo)
    {
        _logger = logger;
        _repo = repo;
        _productRepo = productRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OrderWorker started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var pending = await _repo.LoadPendingAsync(stoppingToken);
                foreach (var order in pending)
                {
                    try
                    {
                        var price = await _productRepo.GetPriceAsync(order.ProductName, stoppingToken);
                        order.Price = price * order.Quantity;
                        order.IsProceed = 1;

                        await _repo.UpdateAsync(order, stoppingToken);
                        _logger.LogInformation("Processed order {OrderCustomer} {Product} qty {Qty} total {Total}", order.CustomerName, order.ProductName, order.Quantity, order.Price);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to process order for {Customer}", order.CustomerName);
                    }
                }

                // Poll interval - small sleep, cancellation-aware
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker cancellation requested");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled worker loop exception");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        _logger.LogInformation("OrderWorker stopping");
    }
}
