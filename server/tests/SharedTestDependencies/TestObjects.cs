using Bogus;
using DataAccess.Models;

namespace SharedTestDependencies;

public static class TestObjects
{
    public static Customer Customer()
    {
        return new Faker<Customer>()
            .RuleFor(c => c.Name, f => f.Name.FullName())
            .RuleFor(c => c.Address, f => f.Address.FullAddress())
            .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
            .RuleFor(c => c.Email, f => f.Internet.Email());
    }

    public static Paper Paper()
    {
        return new Faker<Paper>()
            .RuleFor(p => p.Name, f => f.Commerce.ProductName())
            .RuleFor(p => p.Discontinued, f => false)
            .RuleFor(p => p.Stock, f => f.Random.Int(1, 100))
            .RuleFor(p => p.Price, f => (double)f.Finance.Amount(1, 100));
    }

    public static Order Order(Customer customer, List<OrderEntry> orderEntries)
    {
        return new Faker<Order>()
            .RuleFor(o => o.OrderDate, f => DateTime.UtcNow)
            .RuleFor(o => o.Status, f => "Pending")
            .RuleFor(o => o.Customer, customer)
            .RuleFor(o => o.OrderEntries, orderEntries);
    }

    public static OrderEntry OrderEntry(Paper product)
    {
        return new Faker<OrderEntry>()
            .RuleFor(oe => oe.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(oe => oe.Product, product);
    }
}