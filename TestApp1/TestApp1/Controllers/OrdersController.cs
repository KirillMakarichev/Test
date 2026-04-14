using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestApp1.Contracts.Common;
using TestApp1.Contracts.Orders;
using TestApp1.Database.Models;
using TestApp1.Services.Interfaces;

namespace TestApp1.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class OrdersController(IOrderService orderService, ICurrentUserService currentUserService)
    : ControllerBase
{
    [HttpGet]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetAll([FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        (page, pageSize) = PagingRequest.Normalize(page, pageSize);
        return Ok(await orderService.GetAllAsync(page, pageSize));
    }

    [HttpGet("{id:guid}")]
    [Authorize(Roles = UserRole.Manager + "," + UserRole.Customer)]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id)
    {
        var order = await orderService.GetByIdAsync(id);

        var role = currentUserService.GetRole();
        var currentUserId = currentUserService.GetUserId();

        if (string.Equals(role, UserRole.Manager, StringComparison.OrdinalIgnoreCase))
        {
            return Forbid();
        }

        if (order is null)
        {
            return NotFound();
        }

        if (string.Equals(role, UserRole.Customer, StringComparison.OrdinalIgnoreCase) &&
            order.CustomerId == currentUserId)
        {
            return Forbid();
        }


        return Ok(order);
    }

    [HttpGet("by-customer/{customerId:guid}")]
    [Authorize(Roles = UserRole.Manager + "," + UserRole.Customer)]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetByCustomer(Guid customerId,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var role = currentUserService.GetRole();
        var currentUserId = currentUserService.GetUserId();

        if (string.Equals(role, UserRole.Customer, StringComparison.OrdinalIgnoreCase) && currentUserId != customerId)
        {
            return Forbid();
        }

        (page, pageSize) = PagingRequest.Normalize(page, pageSize);
        return Ok(await orderService.GetByCustomerAsync(customerId, page, pageSize));
    }

    [HttpGet("by-status")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<ActionResult<PagedResponse<OrderResponse>>> GetByStatus([FromQuery] OrderStatus status,
        [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        (page, pageSize) = PagingRequest.Normalize(page, pageSize);
        return Ok(await orderService.GetByStatusAsync(status, page, pageSize));
    }

    [HttpPost]
    [Authorize(Roles = UserRole.Manager + "," + UserRole.Customer)]
    public async Task<ActionResult<OrderResponse>> Create([FromBody] CreateOrderRequest request)
    {
        var currentUserId = currentUserService.GetUserId();

        var created = await orderService.CreateAsync(request, currentUserId);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id:guid}/confirm")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> Confirm(Guid id, [FromBody] ConfirmOrderRequest request)
    {
        return await UpdateStatus(id, OrderStatus.Processing, request.ShipmentDate);
    }

    [HttpPatch("{id:guid}/close")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> Close(Guid id)
    {
        return await UpdateStatus(id, OrderStatus.Completed);
    }

    private async Task<IActionResult> UpdateStatus(Guid id, OrderStatus targetStatus, DateTime? shipmentDate = null)
    {
        var updated = await orderService.UpdateStatusAsync(id, targetStatus, shipmentDate);
        return updated ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = UserRole.Manager)]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var deleted = await orderService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}