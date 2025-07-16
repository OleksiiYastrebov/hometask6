using FluentValidation;
using hometask6.Dtos;

namespace hometask6.Validators;

public class CategoryRequestDtoValidator : AbstractValidator<CategoryRequestDto>
{
    public CategoryRequestDtoValidator()
    {
        RuleFor(dto => dto.Name).NotEmpty().MinimumLength(1).MaximumLength(100)
            .WithMessage("Name length must be in range (1,100)");
    }
}