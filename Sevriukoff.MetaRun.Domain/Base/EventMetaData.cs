using System;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Base;

public class EventMetaData
{
    public EventType Type { get; }
    public TimeSpan RunTime { get; }
    public Guid RunId { get; }
    public ulong PlayerId { get; }
    public string Revision { get; }
    public IEventData Data { get; set; }
    
    public EventMetaData(EventType type, TimeSpan runTime,
        Guid runId, ulong playerId, string revision = "0.0.1")
    {
        Type = type;
        RunTime = runTime;
        RunId = runId;
        PlayerId = playerId;

        Revision = revision;
    }
}