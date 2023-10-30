using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using JetBrains.Annotations;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

public class TrackerOptions : IBindableOptionGroup
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
}

public abstract class OptionBase(string section, string key, Type type, object defaultValue)
{
    public string Section { get; } = section;
    public string Key { get; } = key;
    public Type Type { get; } = type;
    public object DefaultValue { get; init; } = defaultValue;
    public abstract  object BoxedValue { get; set; }
    public string Description { get; init; }
}

public class Option<T>(string section, string key, T defaultValue) : OptionBase(section, key, typeof(T), defaultValue)
{
    public T Value { get; set; }
    public override object BoxedValue
    {
        get => Value;
        set => Value = (T) value;
    }
}

public class BindableOption<T>(string section, string key, T defaultValue = default(T)) : Option<T>(section, key, defaultValue), IBindableOption<T>
{
    public new T Value => _configEntry.Value; 
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

public interface IBindableOption<T>
{
    void Bind(ConfigEntry<T> bind);
}

public interface IOptionGroup
{
    IEnumerable<OptionBase> GetOption();
}

public interface IBindableOptionGroup : IOptionGroup
{
    event Action AnyOptionChanged;
}