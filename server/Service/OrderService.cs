using DataAccess;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Service.Interfaces;
using Service.Models.Order;

namespace Service;

public class OrderService(AppDbContext context) : IOrderService
{
    public async Task<OrderDetailViewModel> Create(OrderCreateModel order)
    {
        try
        {
            var newOrder = order.ToOrder();
        
            var products = await context.Papers
                .Where(p => order.OrderEntries.Select(entry => entry.ProductId).Contains(p.Id))
                .ToListAsync();

            if (products.Any(p => p.Discontinued))
                throw new InvalidOperationException("One or more items are discontinued.");

            newOrder.TotalAmount = order.OrderEntries.Sum(entry =>
            {
                var product = products.Find(p => p.Id == entry.ProductId);
                if (product == null)
                    throw new InvalidOperationException("Product not found.");

                return product.Price * entry.Quantity;
            });
            
            context.Orders.Add(newOrder);

            var orderEntries = order.OrderEntries.Select(entry => entry.ToOrderEntry(newOrder)).ToList();
            newOrder.OrderEntries = orderEntries;
        
            await context.SaveChangesAsync();
            return OrderDetailViewModel.FromEntity(newOrder);
        }
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException("An error occurred while trying to insert Order into database.", ex);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException("An error occurred while processing the order.", ex);
        }
    }
}