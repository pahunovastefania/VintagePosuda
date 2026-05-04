namespace VintagePosuda.Tests.Validators;
public class ItemValidatorTests
{
    private static Item ValidItem() => new()
    {
        Name = "Тарелка",
        Year = 1980,
        Price = 1000m,
        ManufacturerId = 1,
        CategoryId = 1,
        MaterialId = 1,
        Details = new ItemDetails { Condition = "Good" },
    };

    [Fact]
    public void Valid_Item_PassesValidation()
    {
        var sut = new ItemValidator();
        var result = sut.Validate(ValidItem());
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("X")]
    public void Name_TooShort_FailsValidation(string name)
    {
        var item = ValidItem();
        item.Name = name;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Item.Name));
    }

    [Fact]
    public void Year_BeforeAllowed_FailsValidation()
    {
        var item = ValidItem();
        item.Year = 1500;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Item.Year));
    }

    [Fact]
    public void Year_InFuture_FailsValidation()
    {
        var item = ValidItem();
        item.Year = DateTime.UtcNow.Year + 5;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Year_Null_IsAllowed()
    {
        var item = ValidItem();
        item.Year = null;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Price_Zero_FailsValidation()
    {
        var item = ValidItem();
        item.Price = 0m;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Item.Price));
    }

    [Fact]
    public void Price_Negative_FailsValidation()
    {
        var item = ValidItem();
        item.Price = -100m;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ManufacturerId_Zero_FailsValidation()
    {
        var item = ValidItem();
        item.ManufacturerId = 0;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Item.ManufacturerId));
    }

    [Fact]
    public void CategoryId_Zero_FailsValidation()
    {
        var item = ValidItem();
        item.CategoryId = 0;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == nameof(Item.CategoryId));
    }

    [Fact]
    public void MaterialId_Zero_FailsValidation()
    {
        var item = ValidItem();
        item.MaterialId = 0;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Details_EmptyCondition_FailsValidation()
    {
        var item = ValidItem();
        item.Details!.Condition = string.Empty;

        var sut = new ItemValidator();
        var result = sut.Validate(item);

        result.IsValid.Should().BeFalse();
    }
}
