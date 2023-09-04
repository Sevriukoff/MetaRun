using System;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events;

/// <summary>
/// Событие, описывающие нанесение урона персонажем, или получение урона персонажем от монстров.
/// </summary>
public class DamageEvent : IEventData
{
    public float Damage { get; set; }
    public DamageType DamageType { get; set; }
    public DotIndex DotType { get; set; }
    public bool IsCrit { get; set; }
    public bool IsRejected { get; set; }
    public string Inflictor { get; set; }
    public Monster Enemy { get; set; }
}