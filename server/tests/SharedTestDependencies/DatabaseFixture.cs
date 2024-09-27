using DataAccess;
using PgCtx;
using Service;

namespace SharedTestDependencies;

public class DatabaseFixture : IDisposable
{ 
    private readonly PgCtxSetup<AppDbContext> _pgCtxSetup = new();

    public DatabaseFixture()
    {
        Environment.SetEnvironmentVariable($"{nameof(AppOptions)}:{nameof(AppOptions.LocalDbConn)}", _pgCtxSetup._postgres.GetConnectionString());
        SeedDatabase();
    }
    
    private void SeedDatabase()
    {
        var customers = TestObjects.Customers(4);
        _pgCtxSetup.DbContextInstance.Customers.AddRange(customers);
        
        var properties = TestObjects.Properties(4); 
        _pgCtxSetup.DbContextInstance.Properties.AddRange(properties);
        
        var papers = TestObjects.Papers(4, properties); 
        _pgCtxSetup.DbContextInstance.Papers.AddRange(papers);
        
        var orders = TestObjects.Orders(customers, papers);
        _pgCtxSetup.DbContextInstance.Orders.AddRange(orders);

        _pgCtxSetup.DbContextInstance.SaveChanges();
    }
    
    public void Dispose()
    {
        _pgCtxSetup.TearDown();
    }

    public AppDbContext AppDbContext()
    {
        return _pgCtxSetup.DbContextInstance;
    }
}