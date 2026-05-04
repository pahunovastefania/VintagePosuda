using FluentValidation;
using VintagePosuda.Web.Models;

namespace VintagePosuda.Web.Validators;
public class ItemValidator : AbstractValidator<Item>
{
    public ItemValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Введите название предмета.")
            .Length(2, 200).WithMessage("Название должно быть от 2 до 200 символов.");

        RuleFor(x => x.Year)
            .InclusiveBetween(1700, DateTime.UtcNow.Year)
            .When(x => x.Year.HasValue)
            .WithMessage($"Год выпуска должен быть в диапазоне 1700–{DateTime.UtcNow.Year}.");

        RuleFor(x => x.Price)
            .GreaterThan(0m).WithMessage("Цена должна быть положительной.")
            .LessThan(10_000_000m).WithMessage("Слишком большая цена. Проверьте ввод.");

        RuleFor(x => x.ManufacturerId)
            .GreaterThan(0).WithMessage("Выберите завод-изготовитель.");

        RuleFor(x => x.CategoryId)
            .GreaterThan(0).WithMessage("Выберите категорию.");

        RuleFor(x => x.MaterialId)
            .GreaterThan(0).WithMessage("Выберите материал.");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Описание не должно быть длиннее 2000 символов.");

        When(x => x.Details is not null, () =>
        {
            RuleFor(x => x.Details!.Condition)
                .NotEmpty().WithMessage("Укажите состояние предмета.")
                .MaximumLength(50);

            RuleFor(x => x.Details!.Origin)
                .MaximumLength(200);

            RuleFor(x => x.Details!.Provenance)
                .MaximumLength(2000);

            RuleFor(x => x.Details!.Defects)
                .MaximumLength(1000);
        });
    }
}
