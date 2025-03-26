using Microsoft.AspNetCore.Authorization;

public class EmailDomainRequirement : IAuthorizationRequirement
{
    public string Domain { get; }

    public EmailDomainRequirement(string domain)
    {
        Domain = domain;
    }
}
