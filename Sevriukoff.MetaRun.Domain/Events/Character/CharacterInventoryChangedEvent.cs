using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;

namespace Sevriukoff.MetaRun.Domain.Events.Character;

public class CharacterInventoryChangedEvent : IEventData
{
    public int ItemId { get; set; }
    public string ItemName { get; set; }
    public ItemRare ItemRare { get; set; }
    public string GetSummationKey() => ItemId.ToString();

    public void Add(IEventData other)
    {
        throw new System.NotImplementedException();
    }
}