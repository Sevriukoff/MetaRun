using System.Globalization;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

public class CharacterLeveledUpEvent : IEventData
{
    public float Level { get; set; }
    public string GetSummationKey() => Level.ToString(CultureInfo.InvariantCulture).Replace(".", "");

    public void Add(IEventData other)
    {
        throw new System.NotImplementedException();
    }
}