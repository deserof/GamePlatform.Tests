using Bogus;
using GamePlatform.Tests.Infrastructure.Clients;

namespace GamePlatform.Tests.Infrastructure.Generators;

public sealed class PlayerRequestOneFaker : Faker<PlayerRequestOneDTO>
{
    public PlayerRequestOneFaker()
    {
        RuleFor(x => x.Email, f => f.Internet.Email());
    }

    public PlayerRequestOneFaker(string email) : this()
    {
        RuleFor(x => x.Email, _ => email);
    }
}
