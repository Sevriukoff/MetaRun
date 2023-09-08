using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Run;

public class RunDifficultyChangedEvent : IEventData
{
    public float AmbientLevel { get; set; }
}