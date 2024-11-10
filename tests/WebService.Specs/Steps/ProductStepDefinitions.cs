using Reqnroll;

namespace WebService.Specs.Steps;

[Binding]
public class ProductStepDefinitions
{
    private List<ProductData> _actual = [];

    [When(@"добавляем продукты:")]
    public void КогдаДобавляемПродукты(Table table)
    {
        _actual = table.CreateSet<ProductData>().ToList();
    }

    [Then(@"получим список продуктов:")]
    public void ТогдаПолучимСписокПродуктов(Table table)
    {
        List<ProductData> expected = table.CreateSet<ProductData>().ToList();
        Assert.Equivalent(_actual, expected);
    }

    private record ProductData(
        string Code,
        string Description,
        decimal Price,
        uint StockQuantity
    );
}