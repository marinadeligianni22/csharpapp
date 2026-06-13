namespace CSharpApp.Application.Products.Queries.GetProducts;

public record GetAllProductsQuery : IRequest<IReadOnlyCollection<Product>>;

public class GetAllProductQueriesHandler :
    IRequestHandler<GetAllProductsQuery, IReadOnlyCollection<Product>>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<GetAllProductQueriesHandler> _logger;

    public GetAllProductQueriesHandler(
        IProductsService productsService,
        ILogger<GetAllProductQueriesHandler> logger)
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
}