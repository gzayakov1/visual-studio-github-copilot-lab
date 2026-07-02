using DataEntities;
using Store.Services;
using System.Net.Http.Json;

namespace TinyShop.Tests;

[TestClass]
public class ProductTests
{
    [TestMethod]
    public void Product_NewInstance_HasDefaultValues()
    {
        // Arrange & Act
        var product = new Product();

        // Assert
        Assert.AreEqual(0, product.Id);
        Assert.IsNull(product.Name);
        Assert.IsNull(product.Description);
        Assert.AreEqual(0m, product.Price);
        Assert.IsNull(product.ImageUrl);
    }

    [TestMethod]
    public async Task ProductService_GetProductById_ReturnsProductFromApi()
    {
        var handler = new StubHttpMessageHandler((request, _) =>
        {
            Assert.AreEqual("/api/Product/7", request.RequestUri!.PathAndQuery);
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = JsonContent.Create(new Product
                {
                    Id = 7,
                    Name = "Tent",
                    Description = "Test product",
                    Price = 42.50m,
                    ImageUrl = "tent.png"
                }, ProductSerializerContext.Default.Product)
            };

            return Task.FromResult(response);
        });

        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.com")
        };

        var service = new ProductService(httpClient);

        var product = await service.GetProductById(7);

        Assert.IsNotNull(product);
        Assert.AreEqual(7, product!.Id);
        Assert.AreEqual("Tent", product.Name);
    }

    private sealed class StubHttpMessageHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handler) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => handler(request, cancellationToken);
    }
}
