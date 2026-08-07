using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace InvestmentOperations.API.Authorization
{
    public class SameUserOrAdminHandler : AuthorizationHandler<SameUserOrAdminRequirement, int>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SameUserOrAdminRequirement requirement, int resourceUserId)
        {
           if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            var callerIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier);
            if(callerIdClaim != null && int.Parse(callerIdClaim.Value) == resourceUserId)
            {
                context.Succeed(requirement);
            } 

            return Task.CompletedTask;
        }
    }
}

