namespace CSharpApp.Application.Products.Commands;

public sealed record CreateProductCommand(
    string Title,
    int Price,
    string Description,
    int CategoryId,
    List<string> Images
) : IRequest<Product>;
