using CSharpApp.Application.Categories.Commands.CreateCategory;

namespace CSharpApp.Application.Categories.Commands.UpdateCategory;

public record UpdateCategoryCommand(int Id, string Name) : IRequest<Category>;

public class UpdateCategoryCommandHandler :
    IRequestHandler<UpdateCategoryCommand, Category>
{
    private readonly ICategoriesService _categoriesService;
    private readonly ILogger<UpdateCategoryCommandHandler> _logger;

    public UpdateCategoryCommandHandler(
        ICategoriesService categoriesService,
        ILogger<UpdateCategoryCommandHandler> logger)
    {
        _categoriesService = categoriesService ?? throw new ArgumentNullException(nameof(categoriesService));
        _logger = logger;
    }

    public async Task<Category> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UpdateCategoryCommand for category: {Name}", request.Name);

        var category = await _categoriesService.UpdateCategory(request.Id, request.Name);

        _logger.LogInformation("Successfully created category with ID: {CategoryId}", category.Id);

        return category;
    }
}