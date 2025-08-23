using FluentValidation;

namespace CoffeeRestaurant.Application.Orders.Commands.CreateOrder;

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(v => v.CustomerId)
            .NotEmpty().WithMessage("Customer is required.");

        RuleFor(v => v.OrderItems)
            .NotEmpty().WithMessage("Order must contain at least one item.");

        RuleForEach(v => v.OrderItems).SetValidator(new CreateOrderItemRequestValidator());
    }
}

public class CreateOrderItemRequestValidator : AbstractValidator<CreateOrderItemRequest>
{
    public CreateOrderItemRequestValidator()
    {
        RuleFor(v => v.CoffeeItemId)
            .NotEmpty().WithMessage("Coffee item is required.");

        RuleFor(v => v.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than 0.")
            .LessThanOrEqualTo(10).WithMessage("Quantity cannot exceed 10.");

        RuleFor(v => v.SpecialInstructions)
            .MaximumLength(200).WithMessage("Special instructions must not exceed 200 characters.");
    }
}
