using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LegacyOrderService.Data
{
    public class ProductRepository : IProductRepository
    {
        private readonly Dictionary<string, double> _productPrices = new()
        {
            ["Widget"] = 12.99,
            ["Gadget"] = 15.49,
            ["Doohickey"] = 8.75
        };

        public async Task<double> GetPriceAsync(string productName, CancellationToken ct = default)
        {
            // Simulate an expensive lookup (async, cancellation aware)
            await Task.Delay(200, ct);

            if (_productPrices.TryGetValue(productName, out var price))
                return price;

            throw new KeyNotFoundException($"Product '{productName}' not found");
        }
    }
}