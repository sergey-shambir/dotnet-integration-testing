using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebService.Database;
using WebService.Model;

namespace WebService.Controllers;

[ApiController]
[Route("/api/products")]
public class ProductController(WarehouseDbContext dbContext) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Product>>> ListProducts()
    {
        return await dbContext.Products.ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult> AddProduct(
        [FromBody] ProductParams productParams
    )
    {
        Product product = new()
        {
            Code = productParams.Code,
            Description = productParams.Description,
            Price = productParams.Price,
            StockQuantity = productParams.StockQuantity
        };
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{productId:int}")]
    public async Task<ActionResult<int>> AddProduct(
        [FromRoute] int productId,
        [FromBody] ProductParams productParams
    )
    {
        Product? product = await dbContext.Products.FindAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        product.Code = productParams.Code;
        product.Description = productParams.Description;
        product.Price = productParams.Price;
        product.StockQuantity = productParams.StockQuantity;
        dbContext.Products.Update(product);
        await dbContext.SaveChangesAsync();

        return product.Id;
    }

    [HttpDelete("{productId:int}")]
    public async Task<ActionResult> DeleteProduct([FromRoute] int productId)
    {
        Product? product = await dbContext.Products.FindAsync(productId);
        if (product != null)
        {
            dbContext.Products.Remove(product);
            await dbContext.SaveChangesAsync();
        }

        return NoContent();
    }

    public record ProductParams(
        string Code,
        string Description,
        decimal Price,
        uint StockQuantity
    );
}