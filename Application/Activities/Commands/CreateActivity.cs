using Domain;
using MediatR;
using Persistence;
using Application.Activities.DTOs;
using AutoMapper;
using Application.Core;
using Application.Interfaces;


//When a request to create an activity is made,
//the Command is triggered and makes a request, 
// the handler recieves the request and then handler method processes it, 
// and the new activity is saved to the database. 
// The activity ID is then returned as a result.

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command : IRequest<Result<string>>
        {
            public required CreateActivityDto ActivityDto { get; set; }

        }
        public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {
                // Retrieve the current user asynchronously using the userAccessor.
                var user = await userAccessor.GetUserAsync();
                // Retrieve the current user asynchronously using the userAccessor.
                var activity = mapper.Map<Activity>(request.ActivityDto);
                // Add the new activity to the database context.
                context.Activities.Add(activity);   
                // Create a new ActivityAttendee entity to associate the user with the activity as the host.
                var attendee = new ActivityAttendee{
                    ActivityId = activity.Id,
                    UserId = user.Id,
                    IsHost = true
                };
                // Add the attendee to the activity's list of attendees.
                activity.Attendees.Add(attendee);

                // Save changes to the database and check if the operation was successful.
                var result = await context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<string>.Failure("Failed to create the activity", 400);
                // Return a success response with the newly created activity's ID.
                return Result<string>.Success(activity.Id);

            }
        }
    }
}