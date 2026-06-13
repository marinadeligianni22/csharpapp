namespace CSharpApp.Application.Products.Queries;

public sealed record GetAllProductsQuery : IRequest<IReadOnlyCollection<Product>>;
public sealed record GetProductByIdQuery(int Id) : IRequest<Product?>;

