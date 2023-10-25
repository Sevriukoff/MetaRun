using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using BepInEx.Configuration;
using Sevriukoff.MetaRun.Domain.Base;

namespace Sevriukoff.MetaRun.Mod.Base;

public class TrackerOptions : IBindableOptions
{
    #region IsActive

    private BindableOption<bool> _isActive;
    public BindableOption<bool> IsActive => _isActive ??= GetOption<bool>();

    #endregion
    
    #region MaxEventSummation

    private BindableOption<float> _maxEventSummation;
    public BindableOption<float> MaxEventSummation => _maxEventSummation ??= GetOption<float>();

    #endregion

    #region LingerMs

    private BindableOption<float> _lingerMs;
    public BindableOption<float> LingerMs => _lingerMs ??= GetOption<float>();

    #endregion

    #region TrackOnlyHost

    private BindableOption<bool> _trackOnlyHost;
    public BindableOption<bool> TrackOnlyHost => _trackOnlyHost ??= GetOption<bool>();

    #endregion
    
    public Action<EventMetaData> OnEventTracked;
    public event Action AnyOptionChanged;
    
    private readonly Dictionary<string, BindableOptionBase> _binds = new();

    public void Bind<T>(string propName, ConfigEntry<T> bind)
    {
        if (!_binds.ContainsKey(propName))
            _binds.Add(propName, new BindableOption<T>(bind));
    }

    private BindableOption<T> GetOption<T>([CallerMemberName] string propName = null)
        => (BindableOption<T>) _binds[propName!];
}

public interface IBindableOptions
{
    void Bind<T>(string propName, ConfigEntry<T> bind);
    event Action AnyOptionChanged;
}

public class BindableOption<T> : BindableOptionBase
{
    private readonly ConfigEntry<T> _configEntry;

    public new T Value => _configEntry.Value;

    public event Action<T> OptionChange;

    public BindableOption(ConfigEntry<T> configEntry)
    {
        _configEntry = configEntry;

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

public class BindableOptionBase
{
    public object Value { get; set; }
}