using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using FluentValidation;

namespace Application.Activities.Validators
{
    public class BaseActivityValidator<T, TDto> : AbstractValidator<T> where TDto : BaseActivityDto
    {
        public BaseActivityValidator(Func<T, TDto> selector)
        {
            RuleFor(x => selector(x).Title)
    .NotEmpty().WithMessage("Title is required")
    .MaximumLength(100).WithMessage("Title does not exceed 100 characters");
            RuleFor(x => selector(x).Description).NotEmpty().WithMessage("Description is required");
            RuleFor(x => selector(x).Date).GreaterThan(DateTime.UtcNow).WithMessage("Date is required");
            RuleFor(x => selector(x).Category).NotEmpty().WithMessage("Category is required");
            RuleFor(x => selector(x).City).NotEmpty().WithMessage("City is required");
            RuleFor(x => selector(x).Venue).NotEmpty().WithMessage("Venue is required");
            RuleFor(x => selector(x).Latitude).NotEmpty().WithMessage("Lattitude is required").InclusiveBetween(-90, 90).WithMessage("Lattitude needs to be between -90 and 90");
            RuleFor(x => selector(x).Longitude).NotEmpty().WithMessage("Longitude is required").InclusiveBetween(-180, 190).WithMessage("Longitude needs to be between -180 and 180");

        }
    }
}