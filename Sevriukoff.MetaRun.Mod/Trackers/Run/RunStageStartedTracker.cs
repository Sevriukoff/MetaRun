using System;
using On.RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Run;
using Sevriukoff.MetaRun.Mod.Base;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunStageStartedTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        On.RoR2.Run.OnServerSceneChanged += StartStage;
    }

    public override void StopProcessing()
    {
        On.RoR2.Run.OnServerSceneChanged -= StartStage;
    }
    
    private void StartStage(On.RoR2.Run.orig_OnServerSceneChanged orig, RoR2.Run self, string sceneName)
    {
        var eventMetadata = new EventMetaData(EventType.RunStageStarted, TimeSpan.FromSeconds(self.GetRunStopwatch()),
            self.GetUniqueId(), 123456789)
        {
            Data = new RunStageStartedEvent
            {
                StageName = sceneName,
                IsFinalStage = self.nextStageScene.isFinalStage,
                StageType = (StageType)(int)self.nextStageScene.sceneType
            }
        };
        
        OnEventProcessed(eventMetadata);
        
        orig(self, sceneName);
    }
}