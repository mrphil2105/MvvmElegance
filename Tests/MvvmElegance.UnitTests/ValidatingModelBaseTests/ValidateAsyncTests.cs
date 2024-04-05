using System.Collections;
using AutoFixture;
using Moq;

namespace MvvmElegance.UnitTests.ValidatingModelBaseTests;

public class ValidateAsyncTests
{
    private static IEnumerable<object[]> GetErrorDictionaries()
    {
        var fixture = new Fixture();
        var propertyNames = fixture.Create<List<string>>();
        var errorMessages = fixture.Create<List<string>>();

        yield return new object[] { propertyNames.ToDictionary(pn => pn, _ => errorMessages.Take(1)) };
        yield return new object[] { propertyNames.ToDictionary(pn => pn, _ => (IEnumerable<string>)errorMessages) };
    }

    private static IEnumerable<object?[]> GetNonErrorDictionaries()
    {
        var fixture = new Fixture();
        var propertyNames = fixture.Create<List<string>>();

        yield return new object?[] { null };
        yield return new object?[] { new Dictionary<string, IEnumerable<string>>() };
        yield return new object?[] { propertyNames.ToDictionary(pn => pn, _ => (IEnumerable<string>?)null) };
        yield return new object?[] { propertyNames.ToDictionary(pn => pn, _ => Enumerable.Empty<string>()) };
    }

    private static IEnumerable<object?[]> GetNullOrEmptyDictionaries()
    {
        yield return new object?[] { null };
        yield return new object?[] { new Dictionary<string, IEnumerable<string>>() };
    }

    //
    // Calls
    //

    [Theory]
    [AutoMoqData]
    public async Task ValidateAsync_CallsValidator([Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        await model.ValidateAsync();

        validatorMock.Verify(v => v.ValidateAsync(default), Times.Once);
    }

    //
    // Errors
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_AddsErrors_WhenValidatorReturnsErrors(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);

        await model.ValidateAsync();

        model.HasErrors.Should().BeTrue();
        errors.Keys.ToDictionary(pn => pn, model.GetErrors).Should().BeEquivalentTo(errors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNonErrorDictionaries))]
    public async Task ValidateAsync_AddsNonErrors_WhenValidatorReturnsNonErrors(
        Dictionary<string, IEnumerable<string>>? nonErrors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(nonErrors);

        await model.ValidateAsync();

        model.HasErrors.Should().BeFalse();
        nonErrors?.Keys.ToDictionary(pn => pn, model.GetErrors).Should().BeEquivalentTo(nonErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_AddsErrors_WhenValidatorReturnsNonErrorsAndThenErrors(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(new Dictionary<string, IEnumerable<string>>());
        await model.ValidateAsync();
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);

        await model.ValidateAsync();

        model.HasErrors.Should().BeTrue();
        errors.Keys.ToDictionary(pn => pn, model.GetErrors).Should().BeEquivalentTo(errors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_RemovesErrors_WhenValidatorReturnsErrorsAndThenNonErrors(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);
        await model.ValidateAsync();
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(new Dictionary<string, IEnumerable<string>>());

        await model.ValidateAsync();

        model.HasErrors.Should().BeFalse();
        errors
            .Keys.ToDictionary(pn => pn, model.GetErrors)
            .Should()
            .BeEquivalentTo(errors.Keys.ToDictionary(pn => pn, _ => (IEnumerable?)null));
    }

    //
    // Raises
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_RaisesPropertyChanged_WhenValidatorReturnsErrors(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);
        using var monitor = model.Monitor();

        await model.ValidateAsync();

        monitor.Should().RaisePropertyChangeFor(m => m.HasErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_RaisesErrorsChanged_WhenValidatorReturnsErrors(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);
        using var monitor = model.Monitor();

        await model.ValidateAsync();

        monitor.Should().Raise(nameof(ValidatingModelBase.ErrorsChanged));
    }

    //
    // DoesNotRaise
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetNullOrEmptyDictionaries))]
    public async Task ValidateAsync_DoesNotRaisePropertyChanged_WhenValidatorReturnsNullOrEmpty(
        Dictionary<string, IEnumerable<string>>? nonErrors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(nonErrors);
        using var monitor = model.Monitor();

        await model.ValidateAsync();

        monitor.Should().NotRaisePropertyChangeFor(m => m.HasErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNullOrEmptyDictionaries))]
    public async Task ValidateAsync_DoesNotRaiseErrorsChanged_WhenValidatorReturnsNullOrEmpty(
        Dictionary<string, IEnumerable<string>>? nonErrors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(nonErrors);
        using var monitor = model.Monitor();

        await model.ValidateAsync();

        monitor.Should().NotRaise(nameof(ValidatingModelBase.ErrorsChanged));
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_DoesNotRaisePropertyChanged_WhenAlreadyValidated(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);
        await model.ValidateAsync();
        using var monitor = model.Monitor();

        await model.ValidateAsync();

        monitor.Should().NotRaisePropertyChangeFor(m => m.HasErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_DoesNotRaiseErrorsChanged_WhenAlreadyValidated(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);
        await model.ValidateAsync();
        using var monitor = model.Monitor();

        await model.ValidateAsync();

        monitor.Should().NotRaise(nameof(ValidatingModelBase.ErrorsChanged));
    }

    //
    // Returns
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorDictionaries))]
    public async Task ValidateAsync_ReturnsFalse_WhenValidatorReturnsErrors(
        Dictionary<string, IEnumerable<string>> errors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(errors);

        var isValid = await model.ValidateAsync();

        isValid.Should().BeFalse();
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNonErrorDictionaries))]
    public async Task ValidateAsync_ReturnsTrue_WhenValidatorReturnsNonErrors(
        Dictionary<string, IEnumerable<string>>? nonErrors,
        [Frozen] Mock<IModelValidator> validatorMock,
        ValidatingModel model
    )
    {
        validatorMock.Setup(v => v.ValidateAsync(default)).ReturnsAsync(nonErrors);

        var isValid = await model.ValidateAsync();

        isValid.Should().BeTrue();
    }

    //
    // Throws
    //

    [Fact]
    public async Task ValidateAsync_ThrowsInvalidOperationException_WhenValidatorIsNull()
    {
        var model = new ValidatingModel(null);

        Func<Task> act = () => model.ValidateAsync();

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
