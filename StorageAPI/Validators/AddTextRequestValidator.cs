namespace StorageAPI.Validators;

public class AddTextRequestValidator : AbstractValidator<AddTextRequest>
{
    public AddTextRequestValidator()
    {
        RuleFor(request => request.Deletable).NotEmpty().WithMessage("Field 'Deletable' cannot be null");
        RuleFor(request => request.Description).NotEmpty().WithMessage("Field 'Description' cannot be null or empty");
    }
}