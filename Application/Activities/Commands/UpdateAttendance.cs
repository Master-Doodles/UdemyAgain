using System;
using System.Security.Cryptography.X509Certificates;
using Application.Core;
using Application.Interfaces;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;
//Class to remove a user from an activity or cancel an activity if the user is the host
public class UpdateAttendance
{
    public class Command : IRequest<Result<Unit>>
    {
        public required string Id { get; set; }
    }
    public class Handler(IUserAccessor userAccessor, AppDbContext context) : IRequestHandler<Command, Result<Unit>>
    {
        public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
        {
            var activity = await context.Activities //get the activities from the db
            .Include(x => x.Attendees) //eager load the attendees cause we need to know if they are attending or not attending
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);//find the first occurance of that activity ussing activity 

            if (activity == null) return Result<Unit>.Failure("Activity not found", 404);//no activity not found error

            var user = await userAccessor.GetUserAsync(); //gets the user 
            var attendence = activity.Attendees.FirstOrDefault(x=> x.UserId == user.Id);//gets the first attendee that matches with the id
            var isHost = activity.Attendees.Any(x => x.IsHost && x.UserId == user.Id);//checks if that user is the host

            if(attendence != null){//if there are attendees 
                if(isHost) activity.IsCancelled = !activity.IsCancelled; //cancel the activity
                else activity.Attendees.Remove(attendence); //remove the attendee
            }
            else{//no attendees
                activity.Attendees.Add(new ActivityAttendee{//add the attendee 
                    UserId = user.Id,
                    ActivityId = activity.Id,
                    IsHost = false
                });
            }

            var result = await context.SaveChangesAsync(cancellationToken) > 0; //save changes to the db
            return result 
            ? Result<Unit>.Success(Unit.Value) 
            : Result<Unit>.Failure("Problem Updating the DB",400);


        }
    }
}
