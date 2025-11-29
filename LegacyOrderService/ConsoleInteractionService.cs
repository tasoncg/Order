using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using LegacyOrderService.Data;
using LegacyOrderService.Models;

public class ConsoleInteractionService : IHostedService
{
    private readonly ILogger<ConsoleInteractionService> _logger;
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private Task? _consoleTask;
    private CancellationTokenSource? _cts;

    public ConsoleInteractionService(
        ILogger<ConsoleInteractionService> logger,
        IOrderRepository orderRepo,
        IProductRepository productRepo)
    {
        _logger = logger;
        _orderRepo = orderRepo;
        _productRepo = productRepo;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Console UI ready…");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _consoleTask = Task.Run(() => RunConsoleUI(_cts.Token));

        return Task.CompletedTask;
    }

    private async Task RunConsoleUI(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            Console.WriteLine("Welcome to Order Processor!");
            Console.WriteLine("Enter customer name (or empty to exit):");
            string? name = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            Console.WriteLine("Enter product name:");
            string product = Console.ReadLine() ?? "";

            Console.WriteLine("Enter quantity:");
            int qty = int.TryParse(Console.ReadLine(), out int q) ? q : 1;

            try
            {
                double price = await _productRepo.GetPriceAsync(product, ct);

                var order = new Order
                {
                    Id = Guid.NewGuid(),
                    CustomerName = name,
                    ProductName = product,
                    Quantity = qty,
                    Price = qty * price,
                    IsProceed = 0
                };

                await _orderRepo.SaveAsync(order, ct);

                Console.WriteLine($"Order saved! Total = {order.Price}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                _logger.LogError(ex, "Failed to create order");
            }

            Console.WriteLine("---------------------------------------");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_cts != null)
            _cts.Cancel();

        if (_consoleTask != null)
            await _consoleTask;
    }
}
