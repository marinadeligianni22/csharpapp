namespace CSharpApp.Application.Products.Commands;

public sealed class ProductCommandsHandler :
    IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<ProductCommandsHandler> _logger;

    public ProductCommandsHandler(
        IProductsService productsService,
        ILogger<ProductCommandsHandler> logger)
    {
        _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
        _logger = logger;
    }

    public async Task<Product> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateProductCommand for product: {Title}", request.Title);

        var product = await _productsService.CreateProduct(
            request.Title,
            request.Price,
            request.Description,
            request.CategoryId,
            request.Images);

        _logger.LogInformation("Successfully created product with ID: {ProductId}", product.Id);

        return product;
    }
}
