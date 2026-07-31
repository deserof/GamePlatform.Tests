namespace GamePlatform.Tests.Configuration.Models;

public class TestSettings
{
    public const string SectionName = "TestSettings";

    public required string PlayersApi { get; init; }
}
