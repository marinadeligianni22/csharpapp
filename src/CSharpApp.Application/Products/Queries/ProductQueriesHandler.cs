namespace CSharpApp.Application.Products.Queries;

/// <summary>
/// Handles all product-related queries
/// </summary>
public sealed class ProductQueriesHandler :
    IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product>>,
    IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<ProductQueriesHandler> _logger;

    public ProductQueriesHandler(
        IProductsService productsService,
        ILogger<ProductQueriesHandler> logger)
    {
        _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllProductsQuery");

        var products = await _productsService.GetProducts();

        _logger.LogInformation("Successfully retrieved {Count} products", products.Count);

        return products;
    }

    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetProductByIdQuery for product ID: {ProductId}", request.Id);

        var product = await _productsService.GetProductById(request.Id);

        if (product is null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", request.Id);
        }
        else
        {
            _logger.LogInformation("Successfully retrieved product with ID: {ProductId}", request.Id);
        }

        return product;
    }
}
