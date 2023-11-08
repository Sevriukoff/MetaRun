using System.Collections.Generic;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Base;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunPlayerSessionTracker : BaseEventTracker
{
    public RunPlayerSessionTracker()
    {
        SupportedEvent = new Dictionary<EventType, bool>
        {
            {EventType.RunPlayerConnected, false},
            {EventType.RunPlayerDisconnected, false}
        };
    }
    
    public override void StartProcessing()
    {
        
    }

    public override void StopProcessing()
    {
        
    }
}