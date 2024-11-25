using ProductManager.DTO.Requests;
using ProductManager.DTO.Responses;
using ProductManager.Entities;

namespace ProductManager.Repositories;

public interface IProductRepository
{
    Task<ProductDTO> AddProductAsync(Product newProduct, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task<ProductDTO> GetProductAsync(int idProduct, CancellationToken cancellationToken);
    Task DeleteProductAsync(int idProduct, CancellationToken cancellationToken);
    Task UpdateProductAsync(int idProduct, AddUpdateProductDTO productDto, CancellationToken cancellationToken);
    Task<List<ProductDTO>> GetAllProductsAsync();
    Task<List<ProductDTO>> SearchProductsAsync(ProductSearchFilterDTO data, CancellationToken cancellationToken);
    Task SoftDeleteProductsAsync(int userId, CancellationToken cancellationToken);
    Task RestoreProductsAsync(int userId, CancellationToken cancellationToken);
}