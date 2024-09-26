using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;

namespace Service;

public class OrderService(AppDbContext context) : IOrderService
{
    // Kan blive delt ud i flere metoder senere
    public async Task<OrderDetailViewModel> Create(OrderCreateModel order)
    {
        try
        {
            var newOrder = order.ToOrder();
        
            var products = await context.Papers
                .Where(p => order.OrderEntries.Select(entry => entry.ProductId).Contains(p.Id))
                .ToListAsync();

            var discontinuedProducts = products.Where(p => p.Discontinued).ToList();
            if (discontinuedProducts.Count > 0)
            {
                var discontinuedProductNames = string.Join(", ", discontinuedProducts.Select(p => p.Name));
                throw new InvalidOperationException($"The following items are discontinued: {discontinuedProductNames}.");
            }
            
            newOrder.TotalAmount = order.OrderEntries.Sum(entry =>
            {
                var product = products.Find(p => p.Id == entry.ProductId);
                if (product == null)
                    throw new NotFoundException($"Product {entry.ProductId} not found.");

                if (entry.Quantity > product.Stock)
                    throw new InvalidOperationException($"Insufficient stock for {product.Name}. Requested: {entry.Quantity}, Available: {product.Stock}.");

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
    }
}