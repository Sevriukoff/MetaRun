using System.Collections.Generic;
using RiskOfOptions.OptionConfigs;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterInventoryChangedTrackerOptions : BaseTrackerOptions, IOptionsFor<CharacterInventoryChangedTracker>
{
    public new CharacterInventoryChangedTracker ConfiguredObject { get; set; }
    public BindableOption<bool> IsActiveItemAdded { get; }
    public BindableOption<bool> IsActiveItemRemoved { get; }
    public CharacterInventoryChangedTrackerOptions() : base(typeof(CharacterInventoryChangedTracker))
    {
        IsActiveItemAdded = new BindableOption<bool>(Section, nameof(IsActiveItemAdded)){Description = "description"};
        IsActiveItemRemoved = new BindableOption<bool>(Section, nameof(IsActiveItemRemoved)){Description = "description"};
    }

    public override IEnumerable<(OptionBase, BaseOptionConfig)> GetOption()
    {
        yield return (IsActiveItemAdded, null);
        yield return (IsActiveItemRemoved, null);

        foreach (var opt in base.GetOption())
            yield return opt;
    }
}