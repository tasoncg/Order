using LegacyOrderService.Models;
using Microsoft.Data.Sqlite;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using LegacyOrderService.Models;

namespace LegacyOrderService.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly string _connectionString;

        public OrderRepository()
        {
            _connectionString = $"Data Source={Path.Combine(AppContext.BaseDirectory, "orders.db")}";
        }

        public async Task<IEnumerable<Order>> LoadPendingAsync(CancellationToken ct = default)
        {
            var result = new List<Order>();

            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Orders (CustomerName TEXT, ProductName TEXT, Quantity INTEGER, Price REAL);";
            await cmd.ExecuteNonQueryAsync(ct);

            cmd.CommandText = "SELECT CustomerName, ProductName, Quantity, Price FROM Orders;";
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                var o = new Order
                {
                    CustomerName = reader.GetString(0),
                    ProductName = reader.GetString(1),
                    Quantity = reader.GetInt32(2),
                    Price = reader.IsDBNull(3) ? 0.0 : reader.GetDouble(3)
                };
                result.Add(o);
            }

            return result;
        }

        public async Task SaveAsync(Order order, CancellationToken ct = default)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);

            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Orders (CustomerName TEXT, ProductName TEXT, Quantity INTEGER, Price REAL);";
            await cmd.ExecuteNonQueryAsync(ct);

            cmd.CommandText = "INSERT INTO Orders (CustomerName, ProductName, Quantity, Price) VALUES (@cn, @pn, @q, @p);";
            cmd.Parameters.AddWithValue("@cn", order.CustomerName ?? string.Empty);
            cmd.Parameters.AddWithValue("@pn", order.ProductName ?? string.Empty);
            cmd.Parameters.AddWithValue("@q", order.Quantity);
            cmd.Parameters.AddWithValue("@p", order.Price);
            await cmd.ExecuteNonQueryAsync(ct);
        }

        public async Task SeedDataAsync(CancellationToken ct = default)
        {
            await using var connection = new SqliteConnection(_connectionString);
            await connection.OpenAsync(ct);
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = "CREATE TABLE IF NOT EXISTS Orders (CustomerName TEXT, ProductName TEXT, Quantity INTEGER, Price REAL);";
            await cmd.ExecuteNonQueryAsync(ct);

            cmd.CommandText = "INSERT INTO Orders (CustomerName, ProductName, Quantity, Price) VALUES (@cn, @pn, @q, @p);";
            cmd.Parameters.AddWithValue("@cn", "John");
            cmd.Parameters.AddWithValue("@pn", "Widget");
            cmd.Parameters.AddWithValue("@q", 1);
            cmd.Parameters.AddWithValue("@p", 9.99);
            await cmd.ExecuteNonQueryAsync(ct);
        }
    }
}