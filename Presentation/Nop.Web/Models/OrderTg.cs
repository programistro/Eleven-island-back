namespace Nop.Web.Models;

public class OrderTg
{
    public int Number { get; set; }
    
    public DateOnly Date { get; set; }
    
    public string City { get; set; }
    
    public string? AddressPVZ { get; set; }
    
    public bool Courier { get; set; }
    
    /// <summary>
    /// Квартира
    /// </summary>
    public string? Flat { get; set; }
    
    /// <summary>
    /// Этаж
    /// </summary>
    public int? Floor { get; set; }
    
    public string? Domophon { get; set; }
    
    public NopCommerce.Models.CDEK.Order Order { get; set; }
}