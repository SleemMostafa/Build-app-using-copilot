using CoffeeRestaurant.Application.CoffeeItems.Commands.CreateCoffeeItem;
using CoffeeRestaurant.Application.CoffeeItems.Queries.GetCoffeeItems;
using CoffeeRestaurant.Shared.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoffeeRestaurant.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoffeeItemsController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoffeeItemsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<GetCoffeeItemsResponse>>>> GetCoffeeItems()
    {
        var query = new GetCoffeeItemsQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<GetCoffeeItemsResponse>>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CreateCoffeeItemResponse>>> CreateCoffeeItem(CreateCoffeeItemCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<CreateCoffeeItemResponse>.SuccessResult(result, "Coffee item created successfully."));
    }
}
