using System;
using On.RoR2;
using Sevriukoff.MetaRun.Mod.Base;
using ProcChainMask = RoR2.ProcChainMask;

namespace Sevriukoff.MetaRun.Mod.Trackers;

public class MobSpawnEventTracker : BaseEventTracker
{
    public override void StartProcessing()
    {
        HealthComponent.Heal += HealthComponentOnHeal;
    }

    public override void StopProcessing()
    {
        HealthComponent.Heal -= HealthComponentOnHeal;
    }
    
    private float HealthComponentOnHeal(HealthComponent.orig_Heal orig, RoR2.HealthComponent self,
        float amount, ProcChainMask procChainMask, bool nonRegen)
    {
        if (Math.Abs(self.fullHealth - amount) < 0.001 && !nonRegen && !self.body.isPlayerControlled)
        {
            // Моб заспавнился в self вся про него инфа
        }

        return orig(self, amount, procChainMask, nonRegen);
    }
}