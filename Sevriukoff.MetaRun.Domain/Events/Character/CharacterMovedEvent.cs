using System.Globalization;
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
    public string GetSummationKey() =>
        Distance <= 1 ? "1" : Distance.ToString(CultureInfo.InvariantCulture) + PrevPos + CurrentPos;

    public void Add(IEventData other)
    {
        if (GetSummationKey() != other.GetSummationKey())
            return;

        if (other is CharacterMovedEvent movedEvent)
        {
            Distance += movedEvent.Distance;
            CurrentPos = movedEvent.CurrentPos > CurrentPos ? movedEvent.CurrentPos : CurrentPos;
        }
    }
}