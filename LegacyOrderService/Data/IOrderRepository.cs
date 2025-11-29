using LegacyOrderService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegacyOrderService.Data
{
    internal interface IOrderRepository
    {
        Task<IEnumerable<Order>> LoadPendingAsync(CancellationToken ct = default);
        Task SaveAsync(Order order, CancellationToken ct = default);
        Task SeedDataAsync(CancellationToken ct = default);
    }
}
