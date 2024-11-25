using Microsoft.AspNetCore.Authorization;
using ProductManager.DTO.Requests;
using ProductManager.DTO.Responses;
using ProductManager.Entities;
using ProductManager.Repositories;

namespace ProductManager.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    
    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    [Authorize]
    public async Task<ProductDTO> CreateProductAsync(int userId, AddUpdateProductDTO request, CancellationToken cancellationToken)
    {
        var newProduct = new Product()
        {
            DateOfCreation = DateTime.Now,
            Description = request.Description,
            ItemsAvailable = request.ItemsAvailable,
            Name = request.Name,
            Price = request.Price,
            UserId = userId,
            IsHidden = false
        };
        var toReturn = await _productRepository.AddProductAsync(newProduct, cancellationToken);
        await _productRepository.SaveChangesAsync(cancellationToken);
        return toReturn;
    }

    public async Task UpdateProductAsync(int userIdFromToken, int idProduct, AddUpdateProductDTO productDto, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetProductAsync(idProduct, cancellationToken);
        if (product.UserId != userIdFromToken)
        {
            Console.WriteLine(product.UserId);
            Console.WriteLine(userIdFromToken);
            throw new Exception("Users can perform operations only on their products");
        }
        await _productRepository.UpdateProductAsync(idProduct, productDto, cancellationToken);
    }

    public async Task DeleteProductAsync(int userIdFromToken, int idProduct, CancellationToken cancellationToken)
    {
        var product = await GetProductAsync(idProduct, cancellationToken);
        if (product.UserId != userIdFromToken)
        {
            Console.WriteLine(product.UserId);
            Console.WriteLine(userIdFromToken);
            throw new Exception("Users can perform operations only on their products");
        }
        await _productRepository.DeleteProductAsync(idProduct, cancellationToken);
    }

    public async Task<ProductDTO> GetProductAsync(int idProduct, CancellationToken cancellationToken)
    {
        ProductDTO product = null;
        try
        {
            product = await _productRepository.GetProductAsync(idProduct, cancellationToken);
            return product;
        }
        catch (Exception e)
        {
            if (e.Message == "no")
            {
                throw new Exception("no");
            }
        }
        return product;
    }

    public Task<List<ProductDTO>> GetAllProducrsAsync(CancellationToken cancellationToken)
    {
        var products = _productRepository.GetAllProductsAsync();
        return products;
    }

    public Task<List<ProductDTO>> SearchProductsAsync(ProductSearchFilterDTO data, CancellationToken cancellationToken)
    {
        try
        {
            var toreturn = _productRepository.SearchProductsAsync(data, cancellationToken);
            return toreturn;
        }
        catch (Exception e)
        {
            throw new Exception(e.Message);
        }
    }
}