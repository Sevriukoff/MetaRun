using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

public class CharacterLeveledUpEvent : IEventData
{
    public float Level { get; set; }
}