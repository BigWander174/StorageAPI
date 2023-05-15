namespace StorageAPI.Validators;

public class AddTextRequestValidator : AbstractValidator<AddTextRequest>
{
    public AddTextRequestValidator()
    {
        RuleFor(request => request.Description).NotEmpty().WithMessage("Field 'Description' cannot be null or empty");
    }
}