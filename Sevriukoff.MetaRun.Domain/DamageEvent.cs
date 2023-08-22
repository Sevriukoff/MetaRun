using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain;

public class EventBase
{
    public EventType EventType { get; set; }
    public string Revision { get; set; }
}

public class DamageEvent : EventBase
{
    public float Damage { get; set; }
    public DamageType DamageType { get; set; }
    public DotIndex DotType { get; set; }
    public bool IsCrit { get; set; }
    public bool IsRejected { get; set; }
    
    public GameObject Attacker { get; set; }
    public string Inflictor { get; set; }
    public GameObject Target { get; set; }
}

public class HealEvent : EventBase
{
    public float Heal { get; set; }
    public bool IsRegen { get; set; }
    public GameObject Target { get; set; }
}

public class GameObject
{
    public uint PlayerId { get; set; } = 0;
    public string Name { get; set; }
    public bool IsPlayer => PlayerId > 0;
    public bool IsElite { get; set; }
    public bool IsBoss { get; set; }
}