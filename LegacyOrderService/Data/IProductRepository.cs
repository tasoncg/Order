using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyOrderService.Data
{
    internal interface IProductRepository
    {
        public interface IProductRepository
        {
            Task<double> GetPriceAsync(string productName, CancellationToken ct = default);
        }
    }
}
