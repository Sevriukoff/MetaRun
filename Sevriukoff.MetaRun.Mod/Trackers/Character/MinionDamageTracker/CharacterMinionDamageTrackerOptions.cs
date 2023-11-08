using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterMinionDamageTrackerOptions : BaseTrackerOptions, IOptionsFor<CharacterMinionDamageTracker>
{
    public new CharacterMinionDamageTracker ConfiguredObject { get; set; }
    
    public CharacterMinionDamageTrackerOptions() : base(typeof(CharacterMinionDamageTracker))
    {
        
    }
}