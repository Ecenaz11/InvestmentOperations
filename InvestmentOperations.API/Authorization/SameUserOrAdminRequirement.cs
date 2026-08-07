using Microsoft.AspNetCore.Authorization;

namespace InvestmentOperations.API.Authorization
{
    public class SameUserOrAdminRequirement : IAuthorizationRequirement
    {
        public SameUserOrAdminRequirement()
        {
        }
    }
}