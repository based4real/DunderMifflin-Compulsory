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
        // Filter og tjek ids
        var validIds = ValidationHelper.FilterValidIds(ids.Distinct().ToList(), "order", out var invalidIds);
        if (validIds.Count == 0)
            throw new BadRequestException("All provided IDs are invalid. Please provide valid order IDs greater than 0.");

        // Hent ordre med gyldige ids
        var orders = await context.Orders.Where(order => validIds.Contains(order.Id)).ToListAsync();
        var validItemsFound = ValidationHelper.ValidateItemsExistence(validIds, orders, o => o.Id, out var notFoundIds);

        if (!validItemsFound)
            throw new NotFoundException("No valid order IDs were provided for updating status.");

        // Tjek order status
        string status = orderStatus.ToString().ToLower();
        if (!Enum.TryParse<OrderStatus>(status, true, out _))
        {
            var validStatuses = string.Join(", ", Enum.GetNames(typeof(OrderStatus)));
            throw new BadRequestException($"Invalid order status value: {status}. Valid values are: {validStatuses}");
        }

        // Opdater status for gyldige ordre
        var ordersToUpdate = orders.Where(o => !invalidIds.Contains(o.Id) && !notFoundIds.Contains(o.Id)).ToList();
        ordersToUpdate.ForEach(order => order.Status = status);
        
        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            string idsMessage = validIds.Count == 1
                ? $"the order with ID {validIds[0]}"
                : $"the orders with IDs {string.Join(", ", validIds)}";

            throw new DbUpdateException($"An error occurred while updating the status for {idsMessage}.", ex);
        }
    }
}