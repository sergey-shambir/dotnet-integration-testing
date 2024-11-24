using Reqnroll;
using WebService.Specs.Drivers;
using WebService.Specs.Fixture;

namespace WebService.Specs.Steps;

[Binding]
public class ProductStepDefinitions(TestServerFixture fixture)
{
    private readonly ProductApiTestDriver _driver = new(fixture);
    private readonly Dictionary<string, int> _codeToIdMap = new();

    [Given(@"добавили продукты:")]
    [When(@"добавляем продукты:")]
    public async Task КогдаДобавляемПродукты(Table table)
    {
        List<TestProductData> products = table.CreateSet<TestProductData>().ToList();
        foreach (TestProductData product in products)
        {
            int productId = await _driver.AddProduct(product);
            _codeToIdMap[product.Code] = productId;
        }
    }

    [When(@"обновляем продукты:")]
    public async Task КогдаОбновляемПродукты(Table table)
    {
        List<TestProductData> products = table.CreateSet<TestProductData>().ToList();
        foreach (TestProductData product in products)
        {
            if (!_codeToIdMap.TryGetValue(product.Code, out int productId))
            {
                throw new ArgumentException($"Unexpected product code {product.Code}");
            }

            await _driver.UpdateProduct(productId, product);
        }
    }

    [When(@"удаляем продукт с кодом ""(.*)""")]
    public async Task КогдаУдаляемПродуктСКодом(string productCode)
    {
        if (!_codeToIdMap.TryGetValue(productCode, out int productId))
        {
            throw new ArgumentException($"Unexpected product code {productCode}");
        }

        await _driver.DeleteProduct(productId);
    }

    [Then(@"получим список продуктов:")]
    public async Task ТогдаПолучимСписокПродуктов(Table table)
    {
        List<TestProductData> expected = table.CreateSet<TestProductData>().ToList();
        List<TestProductData> actual = await _driver.ListProducts();
        Assert.Equal(expected.Count, actual.Count);
        Assert.Equivalent(expected, actual);
    }
}