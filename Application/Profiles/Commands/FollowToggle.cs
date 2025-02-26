
using System;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Persistence;

namespace Application.Profiles.Commands;

/// <summary>
/// Handles toggling a follow/unfollow action between users.
/// </summary>
/// <param name="TargetUserId">The ID of the user to follow or unfollow.</param>
/// <returns>
/// A <see cref="Result{Unit}"/> indicating success or failure.  
/// - Returns 200 if the operation succeeds.  
/// - Returns 400 if the target user is not found or if the update fails.
/// </returns>
/// <remarks>
/// - Uses the current authenticated user as the observer.  
/// - Adds a follow record if none exists; removes it otherwise.  
/// </remarks>
/// 
public class FollowToggle
{
    public class Command : IRequest<Result<Unit>> //returns Result<Unit>
    {
        public required string TargetUserId { get; set; } // targetId is the Id of the user to follow/unfollow 
    }

    public class Handler(AppDbContext context, IUserAccessor userAccessor) 
        : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var observer = await userAccessor.GetUserAsync(); //gets current logged in user
            var target = await context.Users.FindAsync([request.TargetUserId], 
                cancellationToken); //looks for the user that was passed through
            
            if (target == null) return Result<Unit>.Failure("Target user not found", 400);
            
            var following = await context.UserFollowings
                .FindAsync([observer.Id, target.Id], cancellationToken); //checks to see in the userFOllowings table wether they are already following each other

            if (following == null) context.UserFollowings.Add(new UserFollowing //if not following set the ids of the userFollowing table 
            {
                ObserverId = observer.Id,
                TargetId = target.Id
            });
            else context.UserFollowings.Remove(following); //if you are following just remove it 

            return await context.SaveChangesAsync(cancellationToken) > 0// save changes to the db and then return a result
                ? Result<Unit>.Success(Unit.Value)
                : Result<Unit>.Failure("Problem updating following", 400);
        }
    }
}
