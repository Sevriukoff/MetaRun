using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

/// <summary>
/// Событие, описывающие пройдённую дистанцию персонажем.
/// </summary>
public class CharacterMovedEvent : IEventData
{
    public float Distance { get; set; }

    public Vector3 PrevPos { get; set; }
    public Vector3 CurrentPos { get; set; }
}