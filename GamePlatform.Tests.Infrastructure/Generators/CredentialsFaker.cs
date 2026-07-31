using Bogus;
using GamePlatform.Tests.Infrastructure.Clients;

namespace GamePlatform.Tests.Infrastructure.Generators;

public sealed class CredentialsFaker : Faker<CredentialsDTO>
{
    public CredentialsFaker()
    {
        RuleFor(x => x.Email, f => f.Internet.Email());
        RuleFor(x => x.Password, f => f.Internet.Password(8));
    }

    public CredentialsFaker(string email, string password) : this()
    {
        RuleFor(x => x.Email, _ => email);
        RuleFor(x => x.Password, _ => password);
    }
}
