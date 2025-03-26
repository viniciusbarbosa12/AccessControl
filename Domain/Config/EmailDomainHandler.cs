using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Text.RegularExpressions;

public class EmailDomainHandler : AuthorizationHandler<EmailDomainRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EmailDomainRequirement requirement)
    {
        var email = context.User.FindFirst(ClaimTypes.Email)?.Value;

        if (email != null && Regex.IsMatch(email, @$".*@{Regex.Escape(requirement.Domain.TrimStart('@'))}$", RegexOptions.IgnoreCase))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
