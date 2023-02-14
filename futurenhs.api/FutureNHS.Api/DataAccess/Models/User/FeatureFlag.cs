namespace FutureNHS.Api.DataAccess.Models.User;

public record FeatureFlag
{
    public FeatureFlag() {}

    public FeatureFlag(string name, bool enabled)
    {
        Id = name;
        Name = name;
        Enabled = enabled;
    }
    public string Id { get; init; }
    public string Name { get; init; }
    public bool Enabled { get; init; }
}