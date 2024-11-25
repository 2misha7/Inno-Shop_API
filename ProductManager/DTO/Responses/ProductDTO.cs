namespace ProductManager.DTO.Responses;

public class ProductDTO
{
    public int IdProduct { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int ItemsAvailable { get; set; }
    public DateTime DateOfCreation { get; set; }
    public int UserId { get; set; }
}