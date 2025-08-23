using CoffeeRestaurant.Application.CoffeeItems.Commands.CreateCoffeeItem;
using CoffeeRestaurant.Application.CoffeeItems.Queries.GetCoffeeItems;
using CoffeeRestaurant.Shared.Common;
using CoffeeRestaurant.Shared.DTOs;
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
    public async Task<ActionResult<ApiResponse<List<CoffeeItemDto>>>> GetCoffeeItems()
    {
        var query = new GetCoffeeItemsQuery();
        var result = await _mediator.Send(query);
        return Ok(ApiResponse<List<CoffeeItemDto>>.SuccessResult(result));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<CoffeeItemDto>>> CreateCoffeeItem(CreateCoffeeItemCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(ApiResponse<CoffeeItemDto>.SuccessResult(result, "Coffee item created successfully."));
    }
}
