using System;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events;

/// <summary>
/// Событие, описывающие восстановление здоровья персонажа.
/// </summary>
public class HealEvent : IEventData
{
    public float HealAmount { get; set; }
    public bool IsRegen { get; set; }
}