namespace ProductManager.DTO.Requests;

public class AddUpdateProductDTO
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int ItemsAvailable { get; set; }
    
}