using System;
using System.Globalization;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

/// <summary>
/// Событие, описывающие восстановление здоровья персонажа.
/// </summary>
public class CharacterHealedEvent : IEventData
{
    public float HealAmount { get; private set; }
    public float InitialHealAmount { get; }
    public bool IsRegen { get; }
    
    private readonly string _summationKey;
    
    public CharacterHealedEvent(float healAmount, bool isRegen)
    {
        HealAmount = healAmount;
        InitialHealAmount = healAmount;
        IsRegen = isRegen;
        
        _summationKey = IsRegen ? "1" : $"0{(int)InitialHealAmount}{DateTime.Now.Millisecond}";
    }

    public string GetSummationKey()
        => _summationKey;

    public void Add(IEventData other)
    {
        if (GetSummationKey() != other.GetSummationKey())
            return;

        if (other is CharacterHealedEvent healedEvent)
        {
            HealAmount += healedEvent.HealAmount;
        }
    }
}