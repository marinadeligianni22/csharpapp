namespace CSharpApp.Application.Categories.Commands.CreateCategory;

public record CreateCategoryCommand(string Name, string Image) : IRequest<Category>;

public class CreateCategoryCommandHandler :
    IRequestHandler<CreateCategoryCommand, Category>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<CreateCategoryCommandHandler> _logger;

    public CreateCategoryCommandHandler(
        ICategoriesService categoriesService,
        ILogger<CreateCategoryCommandHandler> logger)
    {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        _logger = logger;
    }

    public async Task<Category> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling CreateCategoryCommand for product: {Name}", request.Name);

        var category = await _categoriesService.CreateCategory(request.Name, request.Image);

        _logger.LogInformation("Successfully created category with ID: {CategoryId}", category.Id);

        return category;
    }
}