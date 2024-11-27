using Reqnroll.Assist.Attributes;

namespace WebService.Specs.Drivers;

public class TestProductData(
    string code,
    string description,
    decimal price,
    uint stockQuantity
)
{
    [TableAliases("Код")]
    public string Code { get; init; } = code;

    [TableAliases("Описание")]
    public string Description { get; init; } = description;

    [TableAliases("Цена")]
    public decimal Price { get; init; } = price;

    [TableAliases("Количество")]
    public uint StockQuantity { get; init; } = stockQuantity;
}