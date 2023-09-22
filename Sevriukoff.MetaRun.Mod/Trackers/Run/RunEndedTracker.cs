using RoR2;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events.Run;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunEndedTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        On.RoR2.Run.OnClientGameOver += OnRunEnd;
    }

    public override void StopProcessing()
    {
        On.RoR2.Run.OnClientGameOver -= OnRunEnd;
    }
    
    private void OnRunEnd(On.RoR2.Run.orig_OnClientGameOver orig, RoR2.Run self, RunReport runReport)
    {
        orig(self, runReport);
        
        CreateEventMetaData
        (
            EventType.RunEnded,
            new RunEndedEvent
            {
                Name = runReport.gameEnding.endingTextToken,
                IsWin = runReport.gameEnding.isWin,
                LunarCoinReward = runReport.gameEnding.lunarCoinReward
            }
        );
    }
}