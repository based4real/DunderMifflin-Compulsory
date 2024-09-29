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
    
    public static List<Customer> Customers(int count)
    {
        var customers = new List<Customer>();
        for (int i = 0; i < count; i++)
            customers.Add(Customer());
            
        return customers;
    }

    public static Paper Paper(bool discontinued = false)
    {
        return new Faker<Paper>()
            .RuleFor(p => p.Name, f => $"{f.Commerce.ProductName()}_{Guid.NewGuid()}")
            .RuleFor(p => p.Discontinued, discontinued)
            .RuleFor(p => p.Stock, f => f.Random.Int(0, 100))
            .RuleFor(p => p.Price, f => (double)f.Finance.Amount(1, 500))
            .RuleFor(p => p.Properties, new List<Property>());
    }

    public static List<Paper> Papers(int count, List<Property> allProperties)
    {
        var faker = new Faker();
        var papers = new List<Paper>();
        for (int i = 0; i < count; i++)
        {
            var paper = Paper();
            
            int propertyCount = faker.Random.Int(0, allProperties.Count - 1);
            paper.Properties = allProperties.OrderBy(x => Guid.NewGuid()).Take(propertyCount).ToList();
            
            foreach (var property in paper.Properties)
                property.Papers.Add(paper);
                
            papers.Add(paper);
        }
        
        return papers;
    }

    public static Property Property()
    {
        return new Faker<Property>()
            .RuleFor(p => p.PropertyName, f => f.Commerce.ProductAdjective())
            .RuleFor(p => p.Papers, f => new List<Paper>());
    }
    
    public static List<Property> Properties(int count)
    {
        var properties = new List<Property>();
        var propertyNames = new HashSet<string>();

        while (properties.Count < count)
        {
            var property = Property();
            if (propertyNames.Add(property.PropertyName))
                properties.Add(property);
        }
        
        return properties;
    }

    public static Order Order(Customer customer, List<OrderEntry> orderEntries)
    {
        return new Faker<Order>()
            .RuleFor(o => o.OrderDate, DateTime.UtcNow)
            .RuleFor(o => o.DeliveryDate, DateOnly.FromDateTime(DateTime.Today.AddDays(3)))
            .RuleFor(o => o.Status, "Pending")
            .RuleFor(o => o.Customer, customer)
            .RuleFor(o => o.OrderEntries, orderEntries);
    }

    public static OrderEntry OrderEntry(Paper product)
    {
        return new Faker<OrderEntry>()
            .RuleFor(oe => oe.Quantity, f => f.Random.Int(1, 10))
            .RuleFor(oe => oe.Product, product);
    }
    
    public static List<Order> Orders(List<Customer> customers, List<Paper> papers)
    {
        var orders = new List<Order>();
        var faker = new Faker();

        foreach (var customer in customers)
        {
            int orderCount = faker.Random.Int(1, 5); // Hver kunde har 1-5 ordre
            for (int i = 0; i < orderCount; i++)
            {
                // Random OrderEntries for hver order
                int entryCount = faker.Random.Int(1, 5);
                var orderEntries = new List<OrderEntry>();
                for (int j = 0; j < entryCount; j++)
                {
                    // Vælg random paper
                    var product = faker.PickRandom(papers);
                    var orderEntry = OrderEntry(product);
                    orderEntries.Add(orderEntry);
                }
                var order = Order(customer, orderEntries);
                orders.Add(order);
            }
        }
        return orders;
    }
}