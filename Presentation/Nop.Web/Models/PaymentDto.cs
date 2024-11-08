namespace NopCommerce.Models;

public class PaymentDto
{
    public string Email { get; set; }
    
    public string? Discription { get; set; }
    
    /// <summary>
    /// в копейках
    /// </summary>
    public int Anmount { get; set; }
    
    public decimal Price { get; set; }
    
    public List<Item> Items { get; set; }
}