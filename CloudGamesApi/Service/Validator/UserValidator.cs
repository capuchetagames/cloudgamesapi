using Core.Dtos;
using FluentValidation;

namespace CloudGamesApi.Service.Validator;

public class UserValidator : AbstractValidator<UserDto>
{
    public UserValidator()
    {
        RuleFor(user => user.Name)
            .NotEmpty().WithMessage("O Nome é Obrigatório.")
            .Length(2, 50).WithMessage("O Nome deve ter entre 2 e 50 caracteres.");

        RuleFor(user => user.Email)
            .NotEmpty().WithMessage("O Email é Obrigatório.")
            .EmailAddress().WithMessage("O formato do Email não é válido.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("A Senha é obrigatória.")
            .MinimumLength(8).WithMessage("A Senha deve ter pelo menos 8 caracteres.");
        
        RuleFor(user => user.ConfirmPassword)
            .Equal(user => user.Password).WithMessage("A confirmação de Senha não confere.");
        
        RuleFor(user => user.Permission)
            .NotEmpty().WithMessage("O Tipo de usuário é Obrigatório.")
            .IsInEnum().WithMessage("Defina o tipo de usuário.");
    }
    
}