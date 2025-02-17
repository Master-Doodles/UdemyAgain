using Domain;
using MediatR;
using Persistence;
using Application.Activities.DTOs;
using AutoMapper;
using Application.Core;


//When a request to create an activity is made,
//the Command is triggered and makes a request, 
// the handler recieves the request and then handler method processes it, 
// and the new activity is saved to the database. 
// The activity ID is then returned as a result.

namespace Application.Activities.Commands
{
    public class CreateActivity
    {
        public class Command: IRequest<Result<string>>
        {
            public required CreateActivityDto ActivityDto { get; set; }

        }
        public class Handler(AppDbContext context, IMapper mapper): IRequestHandler<Command, Result<string>>
        {
            public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
            {

               var activity = mapper.Map<Activity>(request.ActivityDto);

               context.Activities.Add(activity);

               var result = await context.SaveChangesAsync(cancellationToken) > 0;
                if (!result) return Result<string>.Failure("Failed to create the activity", 400);
                return Result<string>.Success(activity.Id);
            
            }
        }
    }
}