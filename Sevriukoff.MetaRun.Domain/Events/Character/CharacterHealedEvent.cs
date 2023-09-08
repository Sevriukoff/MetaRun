using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

/// <summary>
/// Событие, описывающие восстановление здоровья персонажа.
/// </summary>
public class CharacterHealedEvent : IEventData
{
    public float HealAmount { get; set; }
    public bool IsRegen { get; set; }
}