using System;
using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

/// <summary>
/// Retrieves a list of users who follow or are followed by a given user.
/// </summary>
/// <param name="UserId">The ID of the user whose follow relationships are being queried.</param>
/// <param name="Predicate">
/// Determines whether to return "followers" (users following the target)  
/// or "followings" (users the target is following). Default is "followers".
/// </param>
/// <returns>
/// A <see cref="Result{List{UserProfile}}"/> containing the list of users.  
/// - Returns 200 with the user profiles on success.  
/// - Returns an empty list if no follow relationships exist.
/// </returns>
/// <remarks>
/// - Uses AutoMapper for mapping user entities to DTOs.  
/// - Resolves relationships using the `UserFollowings` table.  
/// </remarks>
/// 
public class GetFollowings
{
    public class Query : IRequest<Result<List<UserProfile>>>
    {   

        //two inputs if there is no second input we defualt as followers
        public string Predicate { get; set; } = "followers";
        public required string UserId { get; set; } 
    }

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) 
        : IRequestHandler<Query, Result<List<UserProfile>>>
    {
        public async Task<Result<List<UserProfile>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var profiles = new List<UserProfile>(); //A list for the profiles

            switch (request.Predicate)
            {
                case "followers": //fetches the users following the given user (Who is following me?)
                    profiles = await context.UserFollowings.Where(x => x.TargetId == request.UserId)
                        .Select(x => x.Observer)
                        .ProjectTo<UserProfile>(mapper.ConfigurationProvider, //maps the user directly to UserProfile
                            new {currentUserId = userAccessor.GetUserId()})
                        .ToListAsync(cancellationToken);
                    break;
                case "followings"://fetches users the given user is following (Who am I following?)
                    profiles = await context.UserFollowings.Where(x => x.ObserverId == request.UserId)
                        .Select(x => x.Target)
                        .ProjectTo<UserProfile>(mapper.ConfigurationProvider, //maps the user directly to UserProfile
                            new {currentUserId = userAccessor.GetUserId()}) //uses there ids as keys
                        .ToListAsync(cancellationToken);
                    break;
            }

            return Result<List<UserProfile>>.Success(profiles);
        }
    }
}