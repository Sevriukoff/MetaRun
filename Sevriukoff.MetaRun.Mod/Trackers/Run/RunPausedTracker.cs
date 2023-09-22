using System;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunPausedTracker : BaseEventTracker
{
    private bool _isRunPaused;
    
    public override void StartProcessing()
    {
        On.RoR2.Run.SetRunStopwatchPaused += RunStop;
    }

    public override void StopProcessing()
    {
        On.RoR2.Run.SetRunStopwatchPaused -= RunStop;
    }
    
    private void RunStop(On.RoR2.Run.orig_SetRunStopwatchPaused orig, RoR2.Run self, bool isPaused)
    {
        orig(self, isPaused);
        
        if (_isRunPaused != isPaused)
        {
            _isRunPaused = isPaused;
            
            if (!isPaused)
                return;
        
            CreateEventMetaData(EventType.RunPaused);
        }
    }
}