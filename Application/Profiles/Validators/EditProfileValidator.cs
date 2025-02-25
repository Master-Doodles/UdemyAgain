using System;
using Application.Activities.Commands;
using FluentValidation;

namespace Application.Profiles.Validators;

public class EditProfileValidator : AbstractValidator<EditProfile.Command>
{
    public EditProfileValidator()
    {
        RuleFor(x=>x.DisplayName).NotEmpty().WithMessage("DisplayName is not present");
    }
}
