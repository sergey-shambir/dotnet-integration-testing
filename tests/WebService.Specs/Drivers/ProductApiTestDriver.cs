using System.Net.Http.Json;
using Newtonsoft.Json;

namespace WebService.Specs.Drivers;

public class ProductApiTestDriver(HttpClient httpClient)
{
    public async Task<List<TestProductData>> ListProducts()
    {
        var response = await httpClient.GetAsync("/api/products");
        await EnsureSuccessStatusCode(response);

        string content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<TestProductData>>(content)
               ?? throw new ArgumentException($"Unexpected JSON response: {content}");
    }

    public async Task AddProduct(TestProductData product)
    {
        var response = await httpClient.PostAsJsonAsync("/api/products", product);
        await EnsureSuccessStatusCode(response);
    }

    private static async Task EnsureSuccessStatusCode(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            string content = await response.Content.ReadAsStringAsync();
            Assert.Fail($"HTTP status code {response.StatusCode}: {content}");
        }
    }
}