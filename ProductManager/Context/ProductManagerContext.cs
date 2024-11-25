using Microsoft.EntityFrameworkCore;
using ProductManager.Entities;

namespace ProductManager.Context;

public class ProductManagerContext : DbContext
{
    public ProductManagerContext()
    {
    }
    
    public ProductManagerContext(DbContextOptions<ProductManagerContext> options)
        : base(options)
    {
    } 
    public DbSet<Product> Product { get; set; }
}