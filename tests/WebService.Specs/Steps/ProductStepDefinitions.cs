using System.Net;
using Reqnroll;
using WebService.Specs.Drivers;
using WebService.Specs.Fixture;

namespace WebService.Specs.Steps;

[Binding]
public class ProductStepDefinitions(TestServerFixture fixture, ScenarioContext scenarioContext)
{
    private readonly ProductApiTestDriver _driver = new(fixture);
    private readonly Dictionary<string, int> _codeToIdMap = new();

    private Exception? _lastException;

    [Given(@"добавили продукты:")]
    [When(@"добавляем продукты:")]
    public async Task КогдаДобавляемПродукты(Table table)
    {
        try
        {
            List<TestProductData> products = table.CreateSet<TestProductData>().ToList();
            foreach (TestProductData product in products)
            {
                int productId = await _driver.AddProduct(product);
                _codeToIdMap[product.Code] = productId;
            }
        }
        catch (Exception e) when (IsNegativeScenario())
        {
            _lastException = e;
        }
    }

    [When(@"обновляем продукты:")]
    public async Task КогдаОбновляемПродукты(Table table)
    {
        try
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
        catch (Exception e) when (IsNegativeScenario())
        {
            _lastException = e;
        }
    }

    [When(@"удаляем продукт с кодом ""(.*)""")]
    public async Task КогдаУдаляемПродуктСКодом(string productCode)
    {
        try
        {
            if (!_codeToIdMap.TryGetValue(productCode, out int productId))
            {
                throw new ArgumentException($"Unexpected product code {productCode}");
            }

            await _driver.DeleteProduct(productId);
        }
        catch (Exception e) when (IsNegativeScenario())
        {
            _lastException = e;
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

    [Then(@"получим ошибку валидации")]
    public void ТогдаПолучимОшибкуВалидации()
    {
        Assert.IsType<ApiClientException>(_lastException);
        if (_lastException is ApiClientException e)
        {
            Assert.Equal(HttpStatusCode.BadRequest, e.HttpStatusCode);
        }
    }

    [AfterScenario]
    private void AfterScenario()
    {
        if (IsNegativeScenario())
        {
            Assert.NotNull(_lastException);
        }
        else if (_lastException != null)
        {
            throw _lastException;
        }
    }

    private bool IsNegativeScenario()
    {
        return scenarioContext.ScenarioInfo.Tags.Contains("negative");
    }
}