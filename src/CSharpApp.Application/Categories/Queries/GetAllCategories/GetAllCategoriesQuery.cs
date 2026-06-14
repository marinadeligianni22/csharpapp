using CSharpApp.Application.Products.Queries.GetProducts;

namespace CSharpApp.Application.Categories.Queries.GetAllCategories;

public record GetAllCategoriesQuery : IRequest<IReadOnlyCollection<Category>>;

public class GetAllCategoriesQueryHandler :
    IRequestHandler<GetAllCategoriesQuery, IReadOnlyCollection<Category>>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<GetAllCategoriesQueryHandler> _logger;

    public GetAllCategoriesQueryHandler(
        ICategoriesService categoriesService,
        ILogger<GetAllCategoriesQueryHandler> logger)
    {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        _logger = logger;
    }

    public async Task<IReadOnlyCollection<Category>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllCategoriesQuery");

        var categories = await _categoriesService.GetCategories();

        _logger.LogInformation("Successfully retrieved {Count} categories", categories.Count);

        return categories;
    }
}