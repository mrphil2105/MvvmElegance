using System.ComponentModel;
using AutoFixture;
using Moq;

namespace MvvmElegance.UnitTests.ValidatingModelBaseTests;

public class ValidatePropertyAsyncTests
{
    private static IEnumerable<object[]> GetErrorCollection()
    {
        var fixture = new Fixture();
        var errorMessages = fixture.Create<List<string>>();

        yield return new object[]
        {
            errorMessages.Take(1)
                .ToList()
        };
        yield return new object[] { errorMessages };
    }

    private static IEnumerable<object?[]> GetNonErrorCollection()
    {
        yield return new object?[] { null };
        yield return new object?[] { new List<string>() };
    }

    //
    // Calls
    //

    [Theory]
    [AutoMoqData]
    public async Task ValidatePropertyAsync_CallsValidator(string propertyName,
        [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        await model.ValidatePropertyAsync(propertyName);

        validatorMock.Verify(v => v.ValidatePropertyAsync(propertyName, default), Times.Once);
    }

    //
    // Errors
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_AddsErrors_WhenValidatorReturnsErrors(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);

        await model.ValidatePropertyAsync(propertyName);

        model.HasErrors.Should()
            .BeTrue();
        model.GetErrors(propertyName)
            ?.Cast<object>()
            .Should()
            .Equal(errors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNonErrorCollection))]
    public async Task ValidatePropertyAsync_AddsNonErrors_WhenValidatorReturnsNonErrors(List<string>? nonErrors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(nonErrors);

        await model.ValidatePropertyAsync(propertyName);

        model.HasErrors.Should()
            .BeFalse();
        model.GetErrors(propertyName)
            .Should()
            .BeEquivalentTo(nonErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_AddsErrors_WhenValidatorReturnsNonErrorsAndThenErrors(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(new List<string>());
        await model.ValidatePropertyAsync(propertyName);
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);

        await model.ValidatePropertyAsync(propertyName);

        model.HasErrors.Should()
            .BeTrue();
        model.GetErrors(propertyName)
            ?.Cast<object>()
            .Should()
            .Equal(errors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_RemovesErrors_WhenValidatorReturnsErrorsAndThenNonErrors(
        List<string> errors, string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);
        await model.ValidatePropertyAsync(propertyName);
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(new List<string>());

        await model.ValidatePropertyAsync(propertyName);

        model.HasErrors.Should()
            .BeFalse();
        model.GetErrors(propertyName)
            ?.Cast<object>()
            .Should()
            .BeEmpty();
    }

    //
    // Raises
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_RaisesPropertyChanged_WhenValidatorReturnsErrors(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);
        using var monitor = model.Monitor();

        await model.ValidatePropertyAsync(propertyName);

        monitor.Should()
            .RaisePropertyChangeFor(m => m.HasErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_RaisesErrorsChanged_WhenValidatorReturnsErrors(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);
        using var monitor = model.Monitor();

        await model.ValidatePropertyAsync(propertyName);

        monitor.Should()
            .Raise(nameof(ValidatingModelBase.ErrorsChanged))
            .WithArgs<DataErrorsChangedEventArgs>(e => e.PropertyName == propertyName);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNonErrorCollection))]
    public async Task ValidatePropertyAsync_RaisesPropertyChanged_WhenValidatorReturnsNonErrors(List<string>? nonErrors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(nonErrors);
        using var monitor = model.Monitor();

        await model.ValidatePropertyAsync(propertyName);

        monitor.Should()
            .RaisePropertyChangeFor(m => m.HasErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNonErrorCollection))]
    public async Task ValidatePropertyAsync_RaisesErrorsChanged_WhenValidatorReturnsNonErrors(List<string>? nonErrors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(nonErrors);
        using var monitor = model.Monitor();

        await model.ValidatePropertyAsync(propertyName);

        monitor.Should()
            .Raise(nameof(ValidatingModelBase.ErrorsChanged))
            .WithArgs<DataErrorsChangedEventArgs>(e => e.PropertyName == propertyName);
    }

    //
    // DoesNotRaise
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_DoesNotRaisePropertyChanged_WhenAlreadyValidated(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);
        await model.ValidatePropertyAsync(propertyName);
        using var monitor = model.Monitor();

        await model.ValidatePropertyAsync(propertyName);

        monitor.Should()
            .NotRaisePropertyChangeFor(m => m.HasErrors);
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_DoesNotRaiseErrorsChanged_WhenAlreadyValidated(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);
        await model.ValidatePropertyAsync(propertyName);
        using var monitor = model.Monitor();

        await model.ValidatePropertyAsync(propertyName);

        monitor.Should()
            .NotRaise(nameof(ValidatingModelBase.ErrorsChanged));
    }

    //
    // Returns
    //

    [Theory]
    [MemberAutoMoqData(nameof(GetErrorCollection))]
    public async Task ValidatePropertyAsync_ReturnsFalse_WhenValidatorReturnsErrors(List<string> errors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(errors);

        var isValid = await model.ValidatePropertyAsync(propertyName);

        isValid.Should()
            .BeFalse();
    }

    [Theory]
    [MemberAutoMoqData(nameof(GetNonErrorCollection))]
    public async Task ValidatePropertyAsync_ReturnsTrue_WhenValidatorReturnsNonErrors(List<string>? nonErrors,
        string propertyName, [Frozen] Mock<IModelValidator> validatorMock, ValidatingModel model)
    {
        validatorMock.Setup(v => v.ValidatePropertyAsync(propertyName, default))
            .ReturnsAsync(nonErrors);

        var isValid = await model.ValidatePropertyAsync(propertyName);

        isValid.Should()
            .BeTrue();
    }

    //
    // Throws
    //

    [Theory]
    [AutoData]
    public async Task ValidatePropertyAsync_ThrowsInvalidOperationException_WhenValidatorIsNull(string propertyName)
    {
        var model = new ValidatingModel(null);

        Func<Task> act = () => model.ValidatePropertyAsync(propertyName);

        await act.Should()
            .ThrowAsync<InvalidOperationException>();
    }
}
