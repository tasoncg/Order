using System;
using LegacyOrderService.Models;
using LegacyOrderService.Data;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using LegacyOrderService.Data;

namespace LegacyOrderService
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            using IHost host = Host.CreateDefaultBuilder(args)
               .ConfigureAppConfiguration((ctx, cfg) =>
               {
                   cfg.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddEnvironmentVariables();
               })
                              .ConfigureServices((ctx, services) =>
               {
                   services.AddSingleton<IOrderRepository, OrderRepository>();
                   services.AddSingleton<IProductRepository, ProductRepository>();
                   services.AddHostedService<ConsoleInteractionService>();
                   services.AddHostedService<OrderWorker>();
               })
               .ConfigureLogging(logging =>
               {
                   logging.ClearProviders();
                   logging.AddConsole();
               })
               .UseConsoleLifetime()
               .Build();
            //Console.WriteLine("Welcome to Order Processor!");
            //Console.WriteLine("Enter customer name:");
            //string name = Console.ReadLine();

            //Console.WriteLine("Enter product name:");
            //string product = Console.ReadLine();
            //var productRepo = new ProductRepository();
            //double price = productRepo.GetPrice(product);


            //Console.WriteLine("Enter quantity:");
            //int qty = Convert.ToInt32(Console.ReadLine());

            //Console.WriteLine("Processing order...");

            //Order order = new Order();
            //order.CustomerName = name;
            //order.ProductName = product;
            //order.Quantity = qty;
            //order.Price = 10.0;

            //double total = order.Quantity * order.Price;

            //Console.WriteLine("Order complete!");
            //Console.WriteLine("Customer: " + order.CustomerName);
            //Console.WriteLine("Product: " + order.ProductName);
            //Console.WriteLine("Quantity: " + order.Quantity);
            //Console.WriteLine("Total: $" + price);

            //Console.WriteLine("Saving order to database...");
            //var repo = new OrderRepository();
            //repo.Save(order);
            //Console.WriteLine("Done.");
            await host.RunAsync();
        }
    }
}
