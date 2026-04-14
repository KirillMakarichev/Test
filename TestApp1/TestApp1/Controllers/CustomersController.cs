using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp1.Contracts.Common;
using TestApp1.Contracts.Customers;
using TestApp1.Database.Models;
using TestApp1.Services.Interfaces;

namespace TestApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CustomersController(ICustomerService customerService, ICurrentUserService currentUserService)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<ActionResult<PagedResponse<CustomerResponse>>> GetAll([FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        (page, pageSize) = PagingRequest.Normalize(page, pageSize);
        return Ok(await customerService.GetAllAsync(page, pageSize));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = UserRole.Manager + "," + UserRole.Customer)]
    public async Task<ActionResult<CustomerResponse>> GetById(Guid id)
    {
        if (id == Guid.Empty)
        {
            return BadRequest();
        }

        var role = currentUserService.GetRole();
        if (string.Equals(role, UserRole.Customer, StringComparison.OrdinalIgnoreCase)
            && currentUserService.GetUserId() != id)
        {
            return Forbid();
        }

        var customer = await customerService.GetByIdAsync(id);
        if (customer is null)
        {
            return NotFound();
        }

        return Ok(customer);
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<ActionResult<CustomerResponse>> Create([FromBody] CreateCustomerRequest request)
    {
        var customer = await customerService.CreateAsync(request);

        if (customer == null)
        {
            return BadRequest();
        }
        
        return CreatedAtAction(nameof(GetById), new { id = customer.Id }, customer);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = UserRole.Manager + "," + UserRole.Customer)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request)
    {
        var role = currentUserService.GetRole();
        if (string.Equals(role, UserRole.Customer, StringComparison.OrdinalIgnoreCase)
            && currentUserService.GetUserId() != id)
        {
            return Forbid();
        }

        var updated = await customerService.UpdateAsync(id, request);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await customerService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}