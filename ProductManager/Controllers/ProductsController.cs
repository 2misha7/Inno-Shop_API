using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProductManager.DTO.Requests;
using ProductManager.Services;

namespace ProductManager.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private int GetAuthorizedUserId()
    {
        var userIdClaim = User.FindFirst("IdUser");
        if (userIdClaim == null)
            throw new UnauthorizedAccessException("User ID not found in the token.");

        return int.Parse(userIdClaim.Value);
    }
    
    private readonly IProductService _productService;

    
    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }
    
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] AddUpdateProductDTO productDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var id = GetAuthorizedUserId();
            var created = await _productService.CreateProductAsync(id, productDto, cancellationToken);
            return StatusCode((int)HttpStatusCode.Created);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpGet("/{idProduct}")]
    public async Task<IActionResult> GetProduct([FromRoute] int idProduct, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productService.GetProductAsync(idProduct, cancellationToken);
            return Ok(product);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [HttpGet]
    public async Task<IActionResult> SearchProducts([FromQuery] ProductSearchFilterDTO data, CancellationToken cancellationToken)
    {
        try
        {
            var products = await _productService.SearchProductsAsync(data, cancellationToken);
            return Ok(products);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
    [Authorize]
    [HttpPut("/{idProduct}")]
    public async Task<IActionResult> UpdateProduct(int idProduct, [FromBody] AddUpdateProductDTO productDto,
        CancellationToken cancellationToken)
    {
        try
        {
            var userIdFromToken = GetAuthorizedUserId();
            await _productService.UpdateProductAsync(userIdFromToken ,idProduct, productDto, cancellationToken);
            return Ok("Product has been updated");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        } 
    }

    [Authorize]
    [HttpDelete("/{idProduct}")]
    public async Task<IActionResult> DeleteProduct(int idProduct, CancellationToken cancellationToken)
    {
        try
        {
            var userIdFromToken = GetAuthorizedUserId();
            await _productService.DeleteProductAsync(userIdFromToken, idProduct, cancellationToken);
            return Ok("Product has been deleted");
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    
    
}