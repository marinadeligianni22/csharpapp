using CSharpApp.Application.Products.Queries.GetProducts;

namespace CSharpApp.Application.Products.Queries.GetProductById;

public record GetProductByIdQuery(int Id) : IRequest<Product?>;

public class GetProductByIdQueryHandler :
    IRequestHandler<GetProductByIdQuery, Product?>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<GetProductByIdQueryHandler> _logger;

    public GetProductByIdQueryHandler(
        IProductsService productsService,
        ILogger<GetProductByIdQueryHandler> logger)
    {
        _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
        _logger = logger;
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
