namespace ProductManager.DTO.Requests;

public class ProductSearchFilterDTO
{
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public DateTime? CreatedBefore { get; set; }
    public DateTime? CreatedAfter { get; set; }
    
}