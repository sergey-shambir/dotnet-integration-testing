using System.Net.Http.Json;
using Newtonsoft.Json;
using WebService.Specs.Fixture;

namespace WebService.Specs.Drivers;

public class ProductApiTestDriver(ITestServerFixture fixture)
{
    private HttpClient HttpClient => fixture.HttpClient;

    public async Task<List<TestProductData>> ListProducts()
    {
        var response = await HttpClient.GetAsync("/api/products");
        await EnsureSuccessStatusCode(response);

        string content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<TestProductData>>(content)
               ?? throw new ArgumentException($"Unexpected JSON response: {content}");
    }

    public async Task<int> AddProduct(TestProductData product)
    {
        var response = await HttpClient.PostAsJsonAsync("/api/products", product);
        await EnsureSuccessStatusCode(response);

        string content = await response.Content.ReadAsStringAsync();
        AddProductResult result = JsonConvert.DeserializeObject<AddProductResult>(content)
                                  ?? throw new FormatException($"Unexpected response: {content}");

        return result.Id;
    }

    public async Task UpdateProduct(int productId, TestProductData product)
    {
        var response = await HttpClient.PutAsJsonAsync($"/api/products/{productId}", product);
        await EnsureSuccessStatusCode(response);
    }

    public async Task DeleteProduct(int productId)
    {
        var response = await HttpClient.DeleteAsync($"/api/products/{productId}");
        await EnsureSuccessStatusCode(response);
    }

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            throw new ApiClientException(response.StatusCode, content);
        }
    }

    private record AddProductResult(int Id);
}