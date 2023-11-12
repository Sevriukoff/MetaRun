using System;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events.Character;

namespace Sevriukoff.MetaRun.Domain.Base;

public class EventMetaData : ISummationAbleWith<EventMetaData>
{
    public EventType Type { get; set; }
    public TimeSpan RunTime { get; set; }
    public Guid RunId { get; set; }
    public ulong PlayerId { get; set; }
    public string Revision { get; set; }
    public IEventData Data { get; set; }
    public int CountSummations { get; private set; }
    
    public EventMetaData(EventType type, TimeSpan runTime,
        Guid runId, ulong playerId, string revision = "0.0.1")
    {
        Type = type;
        RunTime = runTime;
        RunId = runId;
        PlayerId = playerId;

        Revision = revision;
    }

    public string GetSummationKey() => PlayerId + "/" + (int)Type + "/" + Data?.GetSummationKey();

    public void Add(EventMetaData other)
    {
        if (GetSummationKey() != other.GetSummationKey())
            return;

        Data.Add(other.Data);
        //Data = Data.Sum(other.Data)

        CountSummations++;
    }
}