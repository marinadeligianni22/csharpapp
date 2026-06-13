using System.Net.Http.Json;

namespace CSharpApp.Application.Products;

public class ProductsService : IProductsService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<ProductsService> _logger;

    public ProductsService(
        HttpClient httpClient,
        IOptions<RestApiSettings> restApiSettings,
        ILogger<ProductsService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> GetProducts()
    {
        try
        {
            _logger.LogInformation("Fetching products from API");

            var response = await _httpClient.GetAsync(_restApiSettings.Products);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var products = JsonSerializer.Deserialize<List<Product>>(content);

            _logger.LogInformation("Successfully fetched {Count} products", products?.Count ?? 0);

            return products?.AsReadOnly() ?? new List<Product>().AsReadOnly();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while fetching products");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize products response");
            throw;
        }
    }

    public async Task<Product?> GetProductById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching product with ID: {ProductId} from API", id);

            var response = await _httpClient.GetAsync($"{_restApiSettings.Products}/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Product with ID {ProductId} not found", id);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var product = JsonSerializer.Deserialize<Product>(content);

            _logger.LogInformation("Successfully fetched product with ID: {ProductId}", id);

            return product;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while fetching product {ProductId}", id);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize product response for ID {ProductId}", id);
            throw;
        }
    }

    public async Task<Product> CreateProduct(string title, int price, string description, int categoryId, List<string> images)
    {
        try
        {
            _logger.LogInformation("Creating new product: {Title}", title);

            var payload = new { title, price, description, categoryId, images };

            var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Products, payload);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdProduct = JsonSerializer.Deserialize<Product>(content);

            _logger.LogInformation("Successfully created product with ID: {ProductId}", createdProduct?.Id);

            return createdProduct!;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while creating product");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize created product response");
            throw;
        }
    }
}
