using System;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Base;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunLeveledUpTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        On.RoR2.Run.OnAmbientLevelUp += AmbientLevelUp;
    }
    
    public override void StopProcessing()
    {
        On.RoR2.Run.OnAmbientLevelUp -= AmbientLevelUp;
    }
    
    private void AmbientLevelUp(On.RoR2.Run.orig_OnAmbientLevelUp orig, RoR2.Run self)
    {
        var eventMetadata = new EventMetaData(EventType.RunLeveledUp, TimeSpan.FromSeconds(self.GetRunStopwatch()),
            self.GetUniqueId(), 123456789);
        
        OnEventProcessed(eventMetadata);
        
        orig(self);
    }
}