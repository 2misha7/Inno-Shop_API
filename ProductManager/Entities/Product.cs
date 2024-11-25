using System.ComponentModel.DataAnnotations;

namespace ProductManager.Entities;

public class Product
{
    [Key]
    public int IdProduct { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int ItemsAvailable { get; set; }
    [Required]
    public DateTime DateOfCreation { get; set; }
    [Required]
    public int UserId { get; set; }
    
    public bool IsHidden { get; set; }
}