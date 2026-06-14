namespace CSharpApp.Application.Categories.Queries.GetCategoryById;
public record GetCategoryByIdQuery(int Id) : IRequest<Category?>;

public class GetCategoryByIdQueryHandler :
    IRequestHandler<GetCategoryByIdQuery, Category?>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<GetCategoryByIdQueryHandler> _logger;

    public GetCategoryByIdQueryHandler(
        ICategoriesService categoriesService,
        ILogger<GetCategoryByIdQueryHandler> logger)
    {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        _logger = logger;
    }

    public async Task<Category?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetCategoryByIdQuery for category ID: {CategoryId}", request.Id);

        var category = await _categoriesService.GetCategoryById(request.Id);

        if (category is null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", request.Id);
        }
        else
        {
            _logger.LogInformation("Successfully retrieved category with ID: {CategoryId}", request.Id);
        }

        return category;
    }
}

