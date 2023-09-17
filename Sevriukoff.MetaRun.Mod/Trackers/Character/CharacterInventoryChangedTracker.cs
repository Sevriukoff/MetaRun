using System;
using RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using CharacterMaster = On.RoR2.CharacterMaster;
using Inventory = On.RoR2.Inventory;
using ItemDef = RoR2.ItemDef;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterInventoryChangedTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        CharacterMaster.OnItemAddedClient += OnItemAdded;
        Inventory.RemoveItem_ItemDef_int += OnRemoveItem;
    }

    public override void StopProcessing()
    {
        CharacterMaster.OnItemAddedClient -= OnItemAdded;
        Inventory.RemoveItem_ItemDef_int -= OnRemoveItem;
    }
    
    private void OnItemAdded(CharacterMaster.orig_OnItemAddedClient orig, RoR2.CharacterMaster self,
        ItemIndex itemIndex)
    {
        orig(self, itemIndex);

        if (!self.GetBody().isPlayerControlled)
            return;

        var currentRun = RoR2.Run.instance;
        var playerId = self.playerCharacterMasterController.networkUser.id.steamId.steamValue;
        var itemDef = ItemCatalog.GetItemDef(itemIndex);

        var eventMetadata = new EventMetaData(EventType.CharacterInventoryItemAdded,
            TimeSpan.FromSeconds(currentRun.GetRunStopwatch()), currentRun.GetUniqueId(), playerId)
        {
            Data = new CharacterInventoryChangedEvent
            {
                ItemId = (int) itemIndex,
                ItemName = itemDef.name,
                ItemRare = (ItemRare) (int) itemDef.tier
            }
        };
        
        OnEventProcessed(eventMetadata);
    }
    
    private void OnRemoveItem(Inventory.orig_RemoveItem_ItemDef_int orig, RoR2.Inventory self, ItemDef itemDef,
        int count)
    {
        orig(self, itemDef, count);

        if (self.playerControllerId < 0)
            return;
        
        var currentRun = RoR2.Run.instance;
        var playerId = self.GetComponent<CharacterBody>().master.playerCharacterMasterController
            .networkUser.id.steamId.steamValue; // TODO: Check

        var eventMetadata = new EventMetaData(EventType.CharacterInventoryItemRemoved,
            TimeSpan.FromSeconds(currentRun.GetRunStopwatch()), currentRun.GetUniqueId(), playerId)
        {
            Data = new CharacterInventoryChangedEvent
            {
                ItemId = (int) itemDef.itemIndex,
                ItemName = itemDef.name,
                ItemRare = (ItemRare) (int) itemDef.tier
            }
        };
        
        OnEventProcessed(eventMetadata);
    }
}