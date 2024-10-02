using DataAccess;
using Microsoft.EntityFrameworkCore;
using Service.Exceptions;
using Service.Interfaces;
using Service.Models.Requests;
using Service.Models.Responses;
using SharedDependencies.Enums;

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

    public async Task UpdateOrderStatus(List<int> ids, OrderStatus orderStatus)
    {
        var validIds = ValidateIds(ids.Distinct().ToList());

        var orders = await context.Orders.Where(order => validIds.Contains(order.Id)).ToListAsync();
        var foundIds = orders.Select(o => o.Id).ToList();
        var invalidIds = validIds.Except(foundIds).ToList();
        
        ValidateFoundOrders(foundIds, invalidIds);
        
        var status = ValidateOrderStatus(orderStatus);

        foreach (var order in orders) order.Status = status;
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            string idsMessage = foundIds.Count == 1
                ? $"the order with ID {foundIds[0]}"
                : $"the orders with IDs {string.Join(", ", foundIds)}";

            throw new DbUpdateException($"An error occurred while updating the status for {idsMessage}.", ex);
        }
    }

    private static List<int> ValidateIds(List<int> ids)
    {
        var validIds = ids.Where(id => id > 0).ToList();

        if (validIds.Count != 0) return validIds;
        
        string errorMessage = ids.Count == 1
            ? $"The provided ID {ids[0]} is invalid. All IDs must be positive numbers greater than 0."
            : $"All provided IDs are invalid. The following IDs are not valid: {string.Join(", ", ids)}. All IDs must be positive numbers greater than 0.";

        throw new BadRequestException(errorMessage);
    }
    
    private static void ValidateFoundOrders(List<int> foundIds, List<int> invalidIds)
    {
        if (foundIds.Count != 0) return;
        
        string errorMessage = invalidIds.Count == 1
            ? $"The provided ID {invalidIds[0]} is invalid."
            : $"No valid order IDs were provided. The following IDs are invalid: {string.Join(", ", invalidIds)}.";

        throw new NotFoundException(errorMessage);
    }
    
    private static string ValidateOrderStatus(OrderStatus orderStatus)
    {
        string status = orderStatus.ToString().ToLower();
        
        if (Enum.TryParse<OrderStatus>(status, true, out _)) return status;
        
        var validStatuses = string.Join(", ", Enum.GetNames(typeof(OrderStatus)));
        throw new BadRequestException($"Invalid order status value: {status}. Valid values are: {validStatuses}");
    }
}