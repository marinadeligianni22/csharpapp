using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CSharpApp.Application.Products.Queries.GetProducts;
using CSharpApp.Application.Products.Queries.GetProductById;
using CSharpApp.Application.Products.Commands.CreateProduct;

namespace CSharpApp.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/products")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll products endpoint called");

        var query = new GetAllProductsQuery();
        var products = await _mediator.Send(query);

        return Ok(products);
    }

    [HttpGet("{productId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProductById(int productId)
    {
        _logger.LogInformation("GetProductById endpoint called for product ID: {ProductId}", productId);

        var query = new GetProductByIdQuery(productId);
        var product = await _mediator.Send(query);

        if (product is null)
        {
            _logger.LogWarning("Product with ID {ProductId} not found", productId);
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        _logger.LogInformation("Create endpoint called for product: {Title}", command.Title);

        var createdProduct = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetProductById),
            new { productId = createdProduct.Id },
            createdProduct);
    }
}
