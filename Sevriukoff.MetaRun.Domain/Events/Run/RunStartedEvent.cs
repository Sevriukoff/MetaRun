using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events.Run;

/// <summary>
/// Событие, описывающие создание нового забега.
/// </summary>
public class RunStartedEvent : IEventData
{
    public ulong Seed { get; set; }
    public string ServerName { get; set; }
    public RunDifficulty Difficulty { get; set; }
    public GameMode GameMode { get; set; }
    public Dictionary<ulong, string> PlayerCharacters { get; set; }
    public string[] Artifacts { get; set; }
    public string[] Dlcs { get; set; }
    public string GetSummationKey() => "-1";

    public void Add(IEventData other)
    {
        return;
    }
}