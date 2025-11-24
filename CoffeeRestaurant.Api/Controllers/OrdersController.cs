using CoffeeRestaurant.Application.Orders.Commands.CreateOrder;
using CoffeeRestaurant.Application.Orders.Queries.GetOrders;
using CoffeeRestaurant.Shared.Common;
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
    public async Task<ActionResult<ApiResponse<List<GetOrdersResponse>>>> GetOrders()
    {
        var query = new GetOrdersQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<GetOrdersResponse>>.SuccessResult(result));
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<CreateOrderResponse>>> CreateOrder(CreateOrderCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<CreateOrderResponse>.SuccessResult(result, "Order created successfully."));
    }
}
