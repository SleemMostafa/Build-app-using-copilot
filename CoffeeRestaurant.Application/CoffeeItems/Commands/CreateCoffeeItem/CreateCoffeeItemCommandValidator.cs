using FluentValidation;

namespace CoffeeRestaurant.Application.CoffeeItems.Commands.CreateCoffeeItem;

public class CreateCoffeeItemCommandValidator : AbstractValidator<CreateCoffeeItemCommand>
{
    public CreateCoffeeItemCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

        RuleFor(v => v.Description)
            .NotEmpty().WithMessage("Description is required.")
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.");

        RuleFor(v => v.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0.");

        RuleFor(v => v.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}
