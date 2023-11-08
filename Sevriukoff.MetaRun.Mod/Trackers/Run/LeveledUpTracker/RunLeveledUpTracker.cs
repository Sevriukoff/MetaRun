using System;
using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunLeveledUpTracker : BaseEventTracker
{
    public RunLeveledUpTracker()
    {
        SupportedEvent = new Dictionary<EventType, bool>
        {
            {EventType.RunLeveledUp, true}
        };
    }
    
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
        CreateEventMetaData(EventType.RunLeveledUp);
        
        orig(self);
    }
}