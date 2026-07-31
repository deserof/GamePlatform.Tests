using Bogus;
using GamePlatform.Tests.Infrastructure.Clients;

namespace GamePlatform.Tests.Infrastructure.Generators;

public sealed class PlayerRequestFaker : Faker<PlayerRequestDTO>
{
    public PlayerRequestFaker()
    {
        RuleFor(x => x.Currency_code, f => f.Finance.Currency().Code);
        RuleFor(x => x.Email, f => f.Internet.Email());
        RuleFor(x => x.Name, f => f.Name.FirstName());
        RuleFor(x => x.Surname, f => f.Name.LastName());
        RuleFor(x => x.Username, f => f.Internet.UserName().PadRight(4, '0'));
        RuleFor(x => x.Password_change, f => f.Internet.Password(8));
        RuleFor(x => x.Password_repeat, (_, p) => p.Password_change);
    }
}
