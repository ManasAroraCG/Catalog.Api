using Catalog.Api.Dtos;
using Catalog.Api.Models;
using Catalog.Api.Repositories;
using Microsoft.AspNetCore.Mvc;
using Catalog.Api.Options;
using Microsoft.Extensions.Options;
namespace Catalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{   
    private readonly IProductRepository _repository;
    private readonly CatalogOptions _options;

    public ProductsController(
            IProductRepository repository,
            IOptions<CatalogOptions> options)
    {
        _repository = repository;
        _options = options.Value;
    }


    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> GetAll()
    {
        var products = await _repository.GetAllAsync();
        var pageSize = _options.DefaultPageSize;
        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Product>> GetById(int id)
    {
        var product = await _repository.GetByIdAsync(id);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }


    [HttpGet("health")]
    public IActionResult GetHealth()
    {
        var healthResponse = new
        {
            status = "Healthy",
            timestamp = DateTime.UtcNow
        };

        return Ok(healthResponse);
    }

    [HttpPost]
    public async Task<ActionResult<Product>> Create(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            return BadRequest();
        }

        var product = await _repository.CreateAsync(
            new Product(
                0,
                dto.Name,
                dto.Price,
                dto.Category));

        return CreatedAtAction(
            nameof(GetById),
            new { id = product.Id },
            product);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, CreateProductDto dto)
    {
        var success = await _repository.UpdateAsync(
            new Product(
                id,
                dto.Name,
                dto.Price,
                dto.Category));

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _repository.DeleteAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}