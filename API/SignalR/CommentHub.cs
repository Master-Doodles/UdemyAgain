using System;
using Application.Activities.Commands;
using Application.Activities.Queries;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;
// Handles incoming messages from clients (e.g., sending comments).
// Sends real-time updates to connected clients.
// Manages SignalR groups, allowing clients to receive activity-specific messages.

public class CommentHub(IMediator mediator) : Hub
{
    public async Task SendComment(AddComment.Command command)
    {
        var comment = await mediator.Send(command);

        await Clients.Group(command.ActivityId).SendAsync("ReceiveComment", comment.Value); //sends the new comment to all clients connected to the same activity group 
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext(); // this code gets the httpContext which is the http request and 
    
        var activityId = httpContext?.Request.Query["activityId"]; // then looks for the the query parameter called activityId if it exists stores it in activityId

        if (string.IsNullOrEmpty(activityId)) throw new HubException("No activity with this id");

        await Groups.AddToGroupAsync(Context.ConnectionId, activityId!); // adds the current connection to a group named activityId 

        var result = await mediator.Send(new GetComments.Query{ActivityId = activityId!});// sends a request to get the comments where the activity id is the same

        await Clients.Caller.SendAsync("LoadComments", result.Value);
    }
}