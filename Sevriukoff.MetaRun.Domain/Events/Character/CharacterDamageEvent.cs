using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Entities;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

/// <summary>
/// Событие, описывающие нанесение урона персонажем, или получение урона персонажем от монстров.
/// </summary>
public class CharacterDamageEvent : IEventData
{
    public float Damage { get; set; }
    public DamageType DamageType { get; set; }
    public DotIndex DotType { get; set; }
    public bool IsCrit { get; set; }
    public bool IsRejected { get; set; }
    public string Inflictor { get; set; }
    public Monster Enemy { get; set; }

    public virtual string GetSummationKey() => ((int) DamageType).ToString() + (int) DotType + Enemy.UnityNetId;

    public void Add(IEventData other)
    {
        if (GetSummationKey() != other.GetSummationKey())
            return;

        if (other is CharacterDamageEvent damageEvent)
        {
            Damage = damageEvent.Damage;
        }
    }
}