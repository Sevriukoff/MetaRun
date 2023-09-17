using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

public class CharacterCoinsChanges : IEventData
{
    public string CoinType { get; set; }
    public int Value { get; set; }
}