using System;
using Application.Core;
using Application.Interfaces;
using Application.Profiles.DTOs;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Profiles.Queries;

public class GetUserActivities
{
    public class Query : IRequest<Result<List<UserActivityDto>>>
    {
        public required string UserId { get; set; }
        public required string Filter { get; set; }//past future and hosting
    }

    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<List<UserActivityDto>>>
    {
        public async Task<Result<List<UserActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
        {

            //get the activities that the user is attending holistically 
            var query = context.ActivityAttendees
                .Where(a => a.UserId == request.UserId)
                .OrderBy(a => a.Activity.Date)
                .Select(b => b.Activity)
                .AsQueryable();

            var filter = string.IsNullOrWhiteSpace(request.Filter) ? "past" : request.Filter.ToLower();
            switch (filter)
            {
                case "future":
                    query = query.Where(a => a.Date >= DateTime.UtcNow);
                    break;
                case "past":
                    query = query.Where(a => a.Date < DateTime.UtcNow);
                    break;
                case "hosting":
                    // we need to check if the activity attendee top id is the same as the given user id 
                    //potential bug is that they are not ordered properly
                    query = query.Where(a => a.Attendees.First().UserId == request.UserId);
                    break;
            }
            var activities = await query
                .ProjectTo<UserActivityDto>(mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);


            return Result<List<UserActivityDto>>.Success(activities); //returns the users activities
        }
    }
}
