using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp1.Contracts.Common;
using TestApp1.Contracts.Products;
using TestApp1.Database.Models;
using TestApp1.Services.Interfaces;

namespace TestApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResponse<ProductResponse>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        (page, pageSize) = PagingRequest.Normalize(page, pageSize);
        return Ok(await productService.GetAllAsync(page, pageSize));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = UserRole.Manager + "," + UserRole.Customer)]
    public async Task<ActionResult<ProductResponse>> GetById(Guid id)
    {
        var product = await productService.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<ActionResult<ProductResponse>> Create([FromBody] CreateProductRequest request)
    {
        var product = await productService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductRequest request)
    {
        var updated = await productService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await productService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
