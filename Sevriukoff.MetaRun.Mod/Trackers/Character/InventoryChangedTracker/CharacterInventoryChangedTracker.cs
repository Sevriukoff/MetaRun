using System;
using System.Collections.Generic;
using RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using CharacterMaster = On.RoR2.CharacterMaster;
using Inventory = On.RoR2.Inventory;
using ItemDef = RoR2.ItemDef;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterInventoryChangedTracker : BaseEventTracker
{
    public CharacterInventoryChangedTracker()
    {
        SupportedEvent = new Dictionary<EventType, bool>
        {
            {EventType.CharacterInventoryItemAdded, true},
            {EventType.CharacterInventoryItemRemoved, true}
        };
    }
    
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
        
        var playerId = self.playerCharacterMasterController.networkUser.id.steamId.steamValue;
        var itemDef = ItemCatalog.GetItemDef(itemIndex);
        
       CreateEventMetaData
       (
           EventType.CharacterInventoryItemAdded,
           new CharacterInventoryChangedEvent
           {
               ItemId = (int) itemIndex,
               ItemName = itemDef.name,
               ItemRare = (ItemRare) (int) itemDef.tier
           },
           playerId
       );
    }
    
    private void OnRemoveItem(Inventory.orig_RemoveItem_ItemDef_int orig, RoR2.Inventory self, ItemDef itemDef,
        int count)
    {
        orig(self, itemDef, count);

        if (self.playerControllerId < 0)
            return;
        
        var playerId = self.GetComponent<CharacterBody>().master.playerCharacterMasterController
            .networkUser.id.steamId.steamValue; // TODO: Check
        
        CreateEventMetaData
        (
            EventType.CharacterInventoryItemRemoved,
            new CharacterInventoryChangedEvent
            {
                ItemId = (int) itemDef.itemIndex,
                ItemName = itemDef.name,
                ItemRare = (ItemRare) (int) itemDef.tier
            },
            playerId
        );
    }

    public override void ChangeSupportedEvent(EventType eventType, bool value)
    {
        base.ChangeSupportedEvent(eventType, value);

        switch (eventType)
        {
            case EventType.CharacterInventoryItemAdded when value:
                CharacterMaster.OnItemAddedClient += OnItemAdded;
                break;
            case EventType.CharacterInventoryItemAdded:
                CharacterMaster.OnItemAddedClient -= OnItemAdded;
                break;
            case EventType.CharacterInventoryItemRemoved when value:
                Inventory.RemoveItem_ItemDef_int += OnRemoveItem;
                break;
            case EventType.CharacterInventoryItemRemoved:
                Inventory.RemoveItem_ItemDef_int -= OnRemoveItem;
                break;
        }
    }
}