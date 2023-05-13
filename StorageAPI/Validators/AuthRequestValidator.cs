namespace StorageAPI.Validators;

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(request => request.Login)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("You need to write a correct email");
        RuleFor(request => request.Password)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("Password length should be at least 6 characters");
    }
}