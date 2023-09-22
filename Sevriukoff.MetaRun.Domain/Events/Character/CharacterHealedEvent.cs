using System.Globalization;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

/// <summary>
/// Событие, описывающие восстановление здоровья персонажа.
/// </summary>
public class CharacterHealedEvent : IEventData
{
    public float HealAmount { get; set; }
    public bool IsRegen { get; set; }
    
    public string GetSummationKey()
        => (HealAmount * 100).ToString(CultureInfo.InvariantCulture) + (IsRegen ? "1" : "0");

    public void Add(IEventData other)
    {
        if (GetSummationKey() != other.GetSummationKey())
            return;

        if (other is CharacterHealedEvent healedEvent)
        {
            HealAmount += healedEvent.HealAmount;
        }
    }
    
    public IEventData Sum(IEventData other)
    {
        if (GetSummationKey() != other.GetSummationKey())
            return null;

        if (other is CharacterHealedEvent healedEvent)
        {
            return new CharacterHealedEvent
            {
                HealAmount = HealAmount + healedEvent.HealAmount,
                IsRegen = healedEvent.IsRegen
            };
        }

        return null;
    }
}