using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries
{
    public class GetActivityDetails
    {
        public class Query : IRequest<Result<ActivityDto>>
        {
            public required string Id { get; set; }
        }
        public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, Result<ActivityDto>>
        {
            public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                // Retrieve the activity from the database, including its attendees and their associated users
                var activity = await context.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider, 
                    new {currentUserId = userAccessor.GetUserId()})// Optimized mapping using projection  
                                                                     // Projection transforms the database query directly into an ActivityDto at the database level,  
                                                                     // reducing the amount of SQL data fetched by selecting only necessary fields and avoiding unnecessary entity loading.  
                .FirstOrDefaultAsync(x => request.Id == x.Id, cancellationToken); // Find the activity by ID


                if (activity == null) return Result<ActivityDto>.Failure("Activity not found", 404);
                // Dont need to map can just return the dto
                return Result<ActivityDto>.Success(activity);
            }
        }
    }
}