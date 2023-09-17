using System;
using On.RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events.Character;
using Sevriukoff.MetaRun.Mod.Base;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterLeveledUpTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        CharacterBody.OnCalculatedLevelChanged += OnLevelUp;
    }

    public override void StopProcessing()
    {
        CharacterBody.OnCalculatedLevelChanged -= OnLevelUp;
    }
    
    private void OnLevelUp(CharacterBody.orig_OnCalculatedLevelChanged orig, RoR2.CharacterBody self,
        float oldLevel, float newLevel)
    {
        orig(self, oldLevel, newLevel);
        
        if (!self.isLocalPlayer)
            return;

        var currentRun = RoR2.Run.instance;
        var playerId = self.master.playerCharacterMasterController.networkUser.id.steamId.steamValue;

        var eventMetadata = new EventMetaData(EventType.CharacterLeveledUp, TimeSpan.FromSeconds(currentRun.GetRunStopwatch()),
            currentRun.GetUniqueId(), playerId)
        {
            Data = new CharacterLeveledUpEvent
            {
                Level = newLevel - oldLevel
            }
        };
        
        OnEventProcessed(eventMetadata);
    }
}