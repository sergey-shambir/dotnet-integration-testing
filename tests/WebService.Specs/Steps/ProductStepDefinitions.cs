using Reqnroll;
using WebService.Specs.Drivers;
using WebService.Specs.Fixture;

namespace WebService.Specs.Steps;

[Binding]
public class ProductStepDefinitions(TestServerFixture fixture)
{
    private readonly ProductApiTestDriver _driver = new(fixture);

    [When(@"добавляем продукты:")]
    public async Task КогдаДобавляемПродукты(Table table)
    {
        List<TestProductData> products = table.CreateSet<TestProductData>().ToList();
        foreach (TestProductData product in products)
        {
            await _driver.AddProduct(product);
        }
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