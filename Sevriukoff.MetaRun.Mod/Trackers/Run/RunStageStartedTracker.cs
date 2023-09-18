using System;
using System.Collections.Generic;
using On.RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events.Run;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using UnityEngine;
using DirectorSpawnRequest = RoR2.DirectorSpawnRequest;
using EventType = Sevriukoff.MetaRun.Domain.Enum.EventType;
using SpawnCard = RoR2.SpawnCard;
using Stage = RoR2.Stage;
using Vector3 = UnityEngine.Vector3;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunStageStartedTracker : BaseEventTracker
{
    private Dictionary<string, int> _interactables;

    public override void StartProcessing()
    {
        _interactables = new();

        InteractableSpawnCard.Spawn += OnSpawnInteractable;
        CharacterMaster.OnServerStageBegin += OnStageStarted;
    }

    public override void StopProcessing()
    {
        InteractableSpawnCard.Spawn -= OnSpawnInteractable;
        CharacterMaster.OnServerStageBegin -= OnStageStarted;

        _interactables = null;
    }
    
    private void OnSpawnInteractable(InteractableSpawnCard.orig_Spawn orig, RoR2.InteractableSpawnCard self,
        Vector3 position, Quaternion rotation, DirectorSpawnRequest directorSpawnRequest, ref SpawnCard.SpawnResult result)
    {
        if (!_interactables.ContainsKey(self.name))
        {
            _interactables.Add(self.name, 1);
        }
        else
        {
            var value = _interactables[self.name];
            _interactables[self.name] = ++value;
        }
        
        orig(self, position, rotation, directorSpawnRequest, ref result);
    }
    
    private void OnStageStarted(CharacterMaster.orig_OnServerStageBegin orig, RoR2.CharacterMaster self, Stage stage)
    {
        orig(self, stage);

        if (self.playerCharacterMasterController == null || _interactables.Count == 0)
            return;

        var eventMetadata = EventMetaDataUtil.CreateEvent
        (
            EventType.RunStageStarted,
            new RunStageStartedEvent
            {
                StageName = stage.sceneDef.baseSceneName,
                IsFinalStage = stage.sceneDef.isFinalStage,
                StageType = (StageType) (int) stage.sceneDef.sceneType,
                Interactables = _interactables
            }
        );
        
        OnEventProcessed(eventMetadata);
        
        _interactables = new Dictionary<string, int>();
    }
}