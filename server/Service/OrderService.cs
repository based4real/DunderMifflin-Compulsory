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
        var newOrder = order.ToOrder();
    
        // Få fat i produkter fra Databasen via. ID fra request
        var products = await context.Papers
            .Where(p => order.OrderEntries.Select(entry => entry.ProductId).Contains(p.Id))
            .ToListAsync();

        // Lav til en liste og tjek om nogle af dem skulle være discontinued
        var discontinuedProducts = products.Where(p => p.Discontinued).ToList();
        if (discontinuedProducts.Count > 0)
        {
            var discontinuedProductNames = string.Join(", ", discontinuedProducts.Select(p => p.Name));
            throw new InvalidOperationException($"The following items are discontinued: {discontinuedProductNames}.");
        }
        
        // Udregn total pris samt tjek om produkt findes og om stock er korrekt
        // Hvis korrekt, stock opdateres samt udregnes pris * mængde
        newOrder.TotalAmount = order.OrderEntries.Sum(entry =>
        {
            var product = products.Find(p => p.Id == entry.ProductId);
            if (product == null)
                throw new NotFoundException($"Product {entry.ProductId} not found.");

            if (entry.Quantity > product.Stock)
                throw new InvalidOperationException($"Insufficient stock for {product.Name}. Requested: {entry.Quantity}, Available: {product.Stock}.");
            
            product.Stock -= entry.Quantity;
            return product.Price * entry.Quantity;
        });
        
        await context.Orders.AddAsync(newOrder);
        try 
        { 
            await context.SaveChangesAsync();
        } 
        catch (DbUpdateException ex)
        {
            throw new DbUpdateException("An error occurred while trying to insert Order into database.", ex);
        }
        return OrderDetailViewModel.FromEntity(newOrder);
    }
}