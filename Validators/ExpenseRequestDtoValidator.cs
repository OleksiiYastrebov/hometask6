using FluentValidation;
using hometask6.Dtos;

namespace hometask6.Validators;

public class ExpenseRequestDtoValidator : AbstractValidator<ExpenseRequestDto>
{
    public ExpenseRequestDtoValidator()
    {
        RuleFor(dto => dto.CategoryId).NotEmpty();
        RuleFor(dto => dto.Price).GreaterThanOrEqualTo(0.01m).LessThanOrEqualTo(99999999.99m); // ! decimal(10,2)
        RuleFor(dto => dto.Comment).MaximumLength(255);
        RuleFor(dto => dto.DateTime);
    }
}