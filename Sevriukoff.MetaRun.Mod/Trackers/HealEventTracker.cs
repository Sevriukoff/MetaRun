using On.RoR2;
using Sevriukoff.MetaRun.Mod.Base;
using ProcChainMask = RoR2.ProcChainMask;

namespace Sevriukoff.MetaRun.Mod.Trackers;

public class HealEventTracker : BaseEventTracker
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
        

        return orig(self, amount, procChainMask, nonRegen);
    }
}