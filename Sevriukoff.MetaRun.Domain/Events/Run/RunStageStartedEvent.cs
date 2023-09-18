using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events.Run;

public class RunStageStartedEvent : IEventData
{
    public string StageName { get; set; }
    public StageType StageType { get; set; }
    public bool IsFinalStage { get; set; }
    public Dictionary<string, int> Interactables { get; set; }
}