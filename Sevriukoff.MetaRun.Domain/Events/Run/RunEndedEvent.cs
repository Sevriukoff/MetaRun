using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Run;

public class RunEndedEvent : IEventData
{
    public string Name { get; set; }
    public bool IsWin { get; set; }
    public uint LunarCoinReward { get; set; }
    public string GetSummationKey() => "-1";

    public void Add(IEventData other)
    {
        return;
    }
}