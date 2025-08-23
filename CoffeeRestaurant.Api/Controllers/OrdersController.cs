using CoffeeRestaurant.Application.Orders.Commands.CreateOrder;
using CoffeeRestaurant.Application.Orders.Queries.GetOrders;
using CoffeeRestaurant.Shared.Common;
using CoffeeRestaurant.Shared.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeRestaurant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<OrderDto>>>> GetOrders()
    {
        var query = new GetOrdersQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<OrderDto>>.SuccessResult(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<OrderDto>>> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<OrderDto>.SuccessResult(result, "Order created successfully."));
    }
}
