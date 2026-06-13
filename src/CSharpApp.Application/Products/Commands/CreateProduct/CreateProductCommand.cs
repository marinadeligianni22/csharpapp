namespace CSharpApp.Application.Products.Commands.CreateProduct;

public record CreateProductCommand(
    string Title,
    int Price,
    string Description,
    int CategoryId,
    List<string> Images
) : IRequest<Product>;

public class CreateProductCommandHandler :
    IRequestHandler<CreateProductCommand, Product>
{
    private readonly IProductsService _productsService;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(
        IProductsService productsService,
        ILogger<CreateProductCommandHandler> logger)
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