using ProductManager.DTO.Requests;
using ProductManager.DTO.Responses;

namespace ProductManager.Services;

public interface IProductService
{
    Task<ProductDTO> CreateProductAsync(int userId, AddUpdateProductDTO updateProductDto, CancellationToken cancellationToken);
    Task UpdateProductAsync(int userIdFromTOken, int idProduct, AddUpdateProductDTO productDto, CancellationToken cancellationToken);
    Task DeleteProductAsync(int userIdFromToken, int idProduct, CancellationToken cancellationToken);
    Task<ProductDTO> GetProductAsync(int idProduct, CancellationToken cancellationToken);
    Task<List<ProductDTO>> GetAllProducrsAsync(CancellationToken cancellationToken);
    Task<List<ProductDTO>> SearchProductsAsync(ProductSearchFilterDTO data, CancellationToken cancellationToken);
   
}