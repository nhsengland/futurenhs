using MimeDetective.Storage.Xml.v2;

namespace FutureNHS.Api.DataAccess.Models.User;

public record FeatureFlag
{
    public FeatureFlag() {}

    public FeatureFlag(FeatureFlag featureFlag)
    {
        Id = featureFlag.Id;
        Name = featureFlag.Name;
        Enabled = featureFlag.Enabled;
    }
    public string Id { get; init; }
    public string Name { get; init; }
    public bool Enabled { get; init; }
}