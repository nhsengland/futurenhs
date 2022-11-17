namespace FutureNHS.Api.DataAccess.Models.FeatureFlags
{
    public sealed record FeatureFlags
    {
        public FeatureFlags() {}

        public FeatureFlags(FeatureFlags featureFlags)
        {
            SelfRegister = featureFlags.SelfRegister;
        }

            
        public bool SelfRegister { get; set; }
    }
}
