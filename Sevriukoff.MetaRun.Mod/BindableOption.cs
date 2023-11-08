using System;
using BepInEx.Configuration;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod;

public class BindableOption<T>(string section, string key, T defaultValue = default) : 
    Option<T>(section, key, defaultValue), 
    IBindableOption<T>
{
    public override T Value
    {
        get => _configEntry.Value;
        set => _configEntry.Value = value;
    }

    public event Action<T> OptionChange;
    
    private ConfigEntry<T> _configEntry;

    public void Bind(ConfigEntry<T> bind)
    {
        _configEntry = bind;

        _configEntry.SettingChanged += (_, _) =>
        {
            OnOptionChange(_configEntry.Value);
        };
    }
    
    private void OnOptionChange(T newValue)
    {
        OptionChange?.Invoke(newValue);
    }
}

/*public class TrackerOptions : IBindableOptionGroup
{
    public BindableOption<bool> IsActive { get; init; }
    public BindableOption<float> MaxEventSummation { get; init; }
    public BindableOption<float> LingerMs { get; init; }
    [CanBeNull] public BindableOption<bool> TrackOnlyHost { get; init; }
    public event Action AnyOptionChanged;

    public TrackerOptions(BindableOption<bool> isActive, BindableOption<float> maxEventSummation,
        BindableOption<float> lingerMs, [CanBeNull] BindableOption<bool> trackOnlyHost = null)
    {
        IsActive = isActive;
        MaxEventSummation = maxEventSummation;
        LingerMs = lingerMs;
        TrackOnlyHost = trackOnlyHost;

        IsActive.OptionChange += b => OnAnyOptionChanged();
        MaxEventSummation.OptionChange += b => OnAnyOptionChanged();
        LingerMs.OptionChange += b => OnAnyOptionChanged();

        if (TrackOnlyHost != null)
            TrackOnlyHost.OptionChange += b => OnAnyOptionChanged();
    }

    protected virtual void OnAnyOptionChanged()
    {
        AnyOptionChanged?.Invoke();
    }

    public IEnumerable<OptionBase> GetOption()
    {
        yield return IsActive;
        yield return MaxEventSummation;
        yield return LingerMs;
        yield return TrackOnlyHost;
    }
}*/