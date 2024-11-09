namespace WebService.Model;

public class Product
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Price { get; set; }
    public uint StockQuantity { get; set; }
}