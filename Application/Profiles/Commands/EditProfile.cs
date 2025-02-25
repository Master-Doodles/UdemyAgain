using System;
using Application.Core;
using Application.Interfaces;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class EditProfile
{
    public class Command:IRequest<Result<Unit>>{

    public required string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
        
    }
    //we grab the database context and we grab the user accessor as we need those to get access to the user with the 
    public class Handler(AppDbContext context,IUserAccessor userAccessor) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var user = await userAccessor.GetUserAsync(); //grab the user

            user.DisplayName = request.DisplayName; //set its fields
            user.Bio = request.Bio;
            //save the user
            var result = await context.SaveChangesAsync(cancellationToken)>0;
            //return the result
            return result 
                ? Result<Unit>.Success(Unit.Value)
                :Result<Unit>.Failure("Problem updating the displayname & bio",400);
            
            
        }
    }
}
