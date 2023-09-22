using System.Globalization;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Run;

public class RunDifficultyChangedEvent : IEventData
{
    public float AmbientLevel { get; set; }
    public string GetSummationKey() =>
        AmbientLevel <= 1 ? "1" : AmbientLevel.ToString(CultureInfo.InvariantCulture).Replace("", ".");

    public void Add(IEventData other)
    {
        throw new System.NotImplementedException();
    }
}