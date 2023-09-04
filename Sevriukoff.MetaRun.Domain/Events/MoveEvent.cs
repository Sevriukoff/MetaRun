using System;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events;

/// <summary>
/// Событие, описывающие пройдённую дистанцию персонажем.
/// </summary>
public class MoveEvent : IEventData
{
    public float Distance { get; set; }

    public Vector3 PrevPos { get; set; }
    public Vector3 CurrentPos { get; set; }
}