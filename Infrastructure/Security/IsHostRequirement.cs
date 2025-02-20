
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;

public class IsHostRequirement : IAuthorizationRequirement
{

}

public class IsHostRequirementHandler(AppDbContext dbContext, IHttpContextAccessor httpContextAccessor) : AuthorizationHandler<IsHostRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
        //wether or not the user attempting to edit an activity is actiually the same user that is hosting the activity
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return;

        var httpContext = httpContextAccessor.HttpContext;
        //checking if we have the http context
        //checking if we have the route Id and that it is a string
        //then its going to be assigned to activityId
        //then  if any faile we are going to return 403 forbidden
        if (httpContext?.GetRouteValue("id") is not string activityId) return;
        //gives us the attendee if the user attempting to edit the activity is an attentee
        var attendee = await dbContext.ActivityAttendees
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId && x.ActivityId == activityId);

        if (attendee == null) return;
        if (attendee.IsHost) context.Succeed(requirement);


    }
}
