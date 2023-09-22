using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

public class CharacterCoinsChangesEvent : IEventData
{
    public bool IsLunarCoin { get; set; }
    public int Value { get; set; }
    public string GetSummationKey() => IsLunarCoin ? "1" : "0";

    public void Add(IEventData other)
    {
        throw new System.NotImplementedException();
    }
}