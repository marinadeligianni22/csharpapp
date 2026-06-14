using Asp.Versioning;
using CSharpApp.Application.Categories.Commands.CreateCategory;
using CSharpApp.Application.Categories.Commands.UpdateCategory;
using CSharpApp.Application.Categories.Queries.GetCategoryById;
using CSharpApp.Application.Categories.Queries.GetAllCategories;

using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CSharpApp.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public CategoriesController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("GetAll categories endpoint called");

        var query = new GetAllCategoriesQuery();
        var categories = await _mediator.Send(query);

        return Ok(categories);
    }

    [HttpGet("{categoryId:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetCatyegoryById(int categoryId)
    {
        _logger.LogInformation("GetCatyegoryById endpoint called for category ID: {CategoryId}", categoryId);

        var query = new GetCategoryByIdQuery(categoryId);
        var category = await _mediator.Send(query);

        if (category is null)
        {
            _logger.LogWarning("Category with ID {CategoryId} not found", categoryId);
            return NotFound();
        }

        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        _logger.LogInformation("Create endpoint called for category: {Name}", command.Name);

        var createdCategory = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetCatyegoryById),
            new { categoryId = createdCategory.Id },
            createdCategory);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        var updatedCommand = command with { Id = id };

        var result = await _mediator.Send(updatedCommand, cancellationToken);
        return Ok(result);
    }

}
