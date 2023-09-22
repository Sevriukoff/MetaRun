using System;
using System.Collections.Generic;
using System.Linq;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Domain.Events;
using Sevriukoff.MetaRun.Domain.Events.Run;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Utils;
using HealthComponent = On.RoR2.HealthComponent;
using ProcChainMask = RoR2.ProcChainMask;

namespace Sevriukoff.MetaRun.Mod.Trackers.Run;

public class RunStartedTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        HealthComponent.Heal += InitializationCharacters; 
    }

    public override void StopProcessing()
    {
        HealthComponent.Heal -= InitializationCharacters;
    }
    
    private float InitializationCharacters(HealthComponent.orig_Heal orig, RoR2.HealthComponent self,
        float amount, ProcChainMask procChainMask, bool nonRegen)
    {
        var run = RoR2.Run.instance;

        if (run.userMasters.Any(x => !x.Value.hasBody))
            return orig(self, amount, procChainMask, nonRegen);
        
        var playerCharacters =
            run.userMasters.ToDictionary(x => x.Key.steamId.steamValue,
                y => y.Value.GetBody().name);
        
        var serverName = RoR2.NetworkSession.instance.NetworkserverName;
        var artifacts = new List<string>(4);
        var dlcs = new List<string>(1);
        
        foreach (var choice in run.ruleBook.choices)
        {
            var str = choice.globalName.ToLower().Split('.');
        
            if (str.Length != 3)
                continue;
        
            if (str[0] == "artifacts" && str[2] == "on")
                artifacts.Add(str[1]);
        
            if (str[0] == "expansions" && str[2] == "on")
                dlcs.Add(str[1]);
        }

        CreateEventMetaData
        ( 
            EventType.RunStarted,
            new RunStartedEvent
            {
                Seed = run.seed,
                ServerName = serverName,
                Difficulty = (RunDifficulty) (int) run.selectedDifficulty,
                GameMode = (GameMode) (int) run.gameModeIndex,
                Artifacts = artifacts.ToArray(),
                Dlcs = dlcs.ToArray(),
                PlayerCharacters = playerCharacters
            }
        );

        HealthComponent.Heal -= InitializationCharacters;

        return orig(self, amount, procChainMask, nonRegen);
    }
}