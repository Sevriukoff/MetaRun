using System;
using System.Collections.Generic;
using RiskOfOptions.OptionConfigs;
using Sevriukoff.MetaRun.Domain.Enum;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod.Trackers.Character;

public class CharacterDamageTrackerOptions : BaseTrackerOptions, IOptionsFor<CharacterDamageTracker>
{
    public new CharacterDamageTracker ConfiguredObject { get; set; }
    
    public BindableOption<bool> IsActiveDamageDealt { get; }
    public BindableOption<bool> IsActiveDamageTook { get;  }

    public CharacterDamageTrackerOptions() : base(typeof(CharacterDamageTracker))
    {
        IsActiveDamageDealt = new BindableOption<bool>(Section, nameof(IsActiveDamageDealt))
        {
            Description = "Вкл/Выкл отслеживание событий нанесения урона персонажем"
        };
        IsActiveDamageTook = new BindableOption<bool>(Section, nameof(IsActiveDamageTook))
        {
            Description = "Вкл/Выкл отслеживание событий получения урона персонажем"
        };

        IsActive.OptionChange += value =>
        {
            if (!value)
            {
                IsActiveDamageDealt.Value = false;
                IsActiveDamageTook.Value = false;
            }
        };
        
        IsActiveDamageDealt.OptionChange += value =>
        {
            ConfiguredObject?.ChangeSupportedEvent(EventType.CharacterDealtDamage, value);
        };
        
        IsActiveDamageTook.OptionChange += value =>
        {
            ConfiguredObject?.ChangeSupportedEvent(EventType.CharacterTookDamage, value);
        };
    }
    
    public override IEnumerable<(OptionBase, BaseOptionConfig)> GetOption()
    {
        yield return (IsActiveDamageDealt, null);
        yield return (IsActiveDamageTook, null);
        
        foreach (var optionBase in base.GetOption())
            yield return optionBase;
    }

    public event Action AnyOptionChanged;
}