using System;
using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Commands;

public class AddComment // handler for dealing with the backend side of a comments
{
    public class Command : IRequest<Result<CommentDto>> // the request that will be sent to the handler,
    {
        public required string Body { get; set; }
        public required string ActivityId { get; set; }
    } // expoects a Body and a activityID to be given and returns a Result of type CommentDTo

    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) // business logic for adding a comments
        : IRequestHandler<Command, Result<CommentDto>>                                      // it requires context, mapper,and userAccesor
                                                                                            //Imapper for converting one adt to another
                                                                                            //AppDbContext represents an instance of the database, allows us to make changes to the db
                                                                                            //IUserAccessor allows us to access the current users data
    {
        public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken) //processes the request made by the handler , takes in a request and a cancellationtoken
        {
            var activity = await context.Activities //queries the database to find the activity with the given ActivityId
                .Include(x => x.Comments)   //include the comments
                .ThenInclude(x => x.User)   //include the user for the comment
                .FirstOrDefaultAsync(x => x.Id == request.ActivityId, cancellationToken); //grab the first 

            if (activity == null) return Result<CommentDto>.Failure("Could not find activity", 404);

            var user = await userAccessor.GetUserAsync(); //gets the logged in user

            var comment = new Comment //creates a new comment ussing the 
            {
                UserId = user.Id,
                ActivityId = activity.Id,
                Body = request.Body
            };

            activity.Comments.Add(comment); //add the comments to the activity field 

            var result = await context.SaveChangesAsync(cancellationToken) > 0; //save changes to the database context

            return result //save the result and send it back 
                ? Result<CommentDto>.Success(mapper.Map<CommentDto>(comment)) //map the individual comment to the dto for the 
                : Result<CommentDto>.Failure("Failed to add comment", 400);
        }
    }
}