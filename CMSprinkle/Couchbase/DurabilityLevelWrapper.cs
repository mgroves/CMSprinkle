using Couchbase.KeyValue;

namespace CMSprinkle.Couchbase;

public class DurabilityLevelWrapper
{
    public DurabilityLevel DurabilityLevel { get; }

    public DurabilityLevelWrapper(DurabilityLevel durabilityLevel)
    {
        DurabilityLevel = durabilityLevel;
    }
}