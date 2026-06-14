using System.Net.Http.Json;
using static System.Net.Mime.MediaTypeNames;

namespace CSharpApp.Application.Categories;

public class CategoriesService : ICategoriesService
{
    private readonly HttpClient _httpClient;
    private readonly RestApiSettings _restApiSettings;
    private readonly ILogger<CategoriesService> _logger;

    public CategoriesService(
        HttpClient httpClient,
        IOptions<RestApiSettings> restApiSettings,
        ILogger<CategoriesService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _restApiSettings = restApiSettings.Value;
        _logger = logger;
    }

    public async Task<Category> CreateCategory(string name, string image)
    {
        try
        {
            _logger.LogInformation("Creating new category: {Name}", name);

            var payload = new { name, image };

            var response = await _httpClient.PostAsJsonAsync(_restApiSettings.Categories, payload);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var createdCategory = JsonSerializer.Deserialize<Category>(content);

            _logger.LogInformation("Successfully created product with ID: {ProductId}", createdCategory?.Id);

            return createdCategory!;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while creating category");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize created category response");
            throw;
        }
    }

    public async Task<IReadOnlyCollection<Category>> GetCategories()
    {
        throw new NotImplementedException();
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        try
        {
            _logger.LogInformation("Fetching category with ID: {CategoryId} from API", id);

            var response = await _httpClient.GetAsync($"{_restApiSettings.Categories}/{id}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found", id);
                return null;
            }

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var category = JsonSerializer.Deserialize<Category>(content);

            _logger.LogInformation("Successfully fetched category with ID: {CategoryId}", id);

            return category;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "HTTP request failed while fetching category {CategoryId}", id);
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize category response for ID {CategoryId}", id);
            throw;
        }
    }
}
