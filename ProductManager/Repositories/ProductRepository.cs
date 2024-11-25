using Microsoft.EntityFrameworkCore;
using ProductManager.Context;
using ProductManager.DTO.Requests;
using ProductManager.DTO.Responses;
using ProductManager.Entities;

namespace ProductManager.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ProductManagerContext _dbContext;

    public ProductRepository(ProductManagerContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ProductDTO> AddProductAsync(Product newProduct, CancellationToken cancellationToken)
    {
        await _dbContext.Product.AddAsync(newProduct, cancellationToken);
        var toReturn = new ProductDTO
        {
            DateOfCreation = newProduct.DateOfCreation,
            Description = newProduct.Description,
            IdProduct = newProduct.IdProduct,
            ItemsAvailable = newProduct.ItemsAvailable,
            Name = newProduct.Name,
            Price = newProduct.Price
        };
        return toReturn;
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ProductDTO> GetProductAsync(int idProduct, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Product.FirstOrDefaultAsync(x => x.IdProduct == idProduct, cancellationToken);
        if (product == null)
        {
            throw new Exception("no");
        }

        if (product.IsHidden)
        {
            throw new Exception("This product does not exist");
        }

        return new ProductDTO
        {
            DateOfCreation = product.DateOfCreation,
            Description = product.Description,
            IdProduct = product.IdProduct,
            ItemsAvailable = product.ItemsAvailable,
            Name = product.Name,
            Price = product.Price,
            UserId = product.UserId
        };
    }

    public async Task DeleteProductAsync(int idProduct, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Product.FirstOrDefaultAsync(x => x.IdProduct == idProduct, cancellationToken);
        if (product is { IsHidden: true })
        {
            throw new Exception("This product does not exist");
        }
        _dbContext.Product.Remove(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateProductAsync(int idProduct, AddUpdateProductDTO productDto, CancellationToken cancellationToken)
    {
        var product = await _dbContext.Product.FirstOrDefaultAsync(x => x.IdProduct == idProduct, cancellationToken);
        if (product.IsHidden)
        {
            throw new Exception("This product does not exist");
        }
        product.Description = productDto.Description;
        product.ItemsAvailable = productDto.ItemsAvailable;
        product.Price = productDto.Price;
        product.Name = productDto.Name;
        
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<ProductDTO>> GetAllProductsAsync()
    {
        var toreturn = new List<ProductDTO>();
        var p = _dbContext.Product.ToList();
        foreach (var v in p)
        {
            if (!v.IsHidden)
            {
                toreturn.Add(new ProductDTO
                {
                    IdProduct = v.IdProduct
                });   
            }
        }

        return toreturn;
    }

    public async Task<List<ProductDTO>> SearchProductsAsync(ProductSearchFilterDTO data, CancellationToken cancellationToken)
    {
        var result = _dbContext.Product.Where(p => !p.IsHidden).AsQueryable();
        
        if (!string.IsNullOrEmpty(data.Name))
        {
           result =  result.Where(p => p.Name.Contains(data.Name));
        }
        if (data.MinPrice.HasValue)
        {
            result =  result.Where(p => p.Price >= data.MinPrice.Value);
        }
        if (data.MaxPrice.HasValue)
        {
            result =  result.Where(p => p.Price <= data.MaxPrice.Value);
        }

        if (data.CreatedAfter.HasValue)
        {
            result = result.Where(p => p.DateOfCreation >= data.CreatedAfter.Value);
        }
        if (data.CreatedBefore.HasValue)
        {
            result = result.Where(p => p.DateOfCreation <= data.CreatedBefore.Value);
        }
        
        var products = await result
            .Select(p => new ProductDTO 
            {
                IdProduct = p.IdProduct,
                Name = p.Name,
                Price = p.Price,
                Description = p.Description,
                ItemsAvailable = p.ItemsAvailable,
                DateOfCreation = p.DateOfCreation,
                UserId = p.UserId
            })
            .ToListAsync(cancellationToken);
      
        
        return products;
    }

    public async Task SoftDeleteProductsAsync(int userId, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Product.Where(p => p.UserId == userId && !p.IsHidden).ToListAsync(cancellationToken);
        foreach (var product in products)
        {
            product.IsHidden = true;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
    
    public async Task RestoreProductsAsync(int userId, CancellationToken cancellationToken)
    {
        var products = await _dbContext.Product.Where(p => p.UserId == userId && p.IsHidden).ToListAsync(cancellationToken);
        foreach (var product in products)
        {
            product.IsHidden = false;
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}