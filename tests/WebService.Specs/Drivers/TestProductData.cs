namespace WebService.Specs.Drivers;

public record TestProductData(
    string Code,
    string Description,
    decimal Price,
    uint StockQuantity
);