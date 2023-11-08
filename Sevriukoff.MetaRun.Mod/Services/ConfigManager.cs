using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Base.Interfaces;

namespace Sevriukoff.MetaRun.Mod.Services;

public class ConfigManager
{
    private readonly ConfigFile _config;
    private readonly Dictionary<string, OptionBase> _options;

    public ConfigManager(AssetManager assetManager)
    {
        Type[] types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();

        _options = new Dictionary<string, OptionBase>(capacity: types.Length * 3);
        
        _config = new ConfigFile(Paths.ConfigPath + "\\HookAjor-MetaRun.cfg", true);
        
        ModSettingsManager.SetModDescription("MetaRun представляет расширенную статистику о забеге в режиме реального времени." +
                                             " Статистика поможет понять кто из твоих друзей больше ворует шмот и пылесосит всю карту." +
                                             " Если без шуток, то MetaRun поможет понять на каком этапе у тебя просел урон или какого типа урона тебе не хватает," +
                                             " какой объём у тебя хила, насколько тщательно ты исследуешь этапы и многое другое.");
        ModSettingsManager.SetModIcon(assetManager.ModIcon);
    }

    public void Configure<T>(Option<T> option, BaseOptionConfig optConfig = null, bool renderUi = true)
    {
        ConfigureInternal<T>(option, optConfig, renderUi);
    }
    
    public void Configure<T>(OptionBase option, BaseOptionConfig optConfig = null, bool renderUi = true)
    {
        ConfigureInternal<T>(option, optConfig, renderUi);
    }

    private void ConfigureInternal<T>(OptionBase option, BaseOptionConfig optConfig = null, bool renderUi = true)
    {
        if (option == null)
            return;
        
        var configEntry = _config.Bind
        (
            option.Section,
            option.Key,
            (T)option.DefaultValue,
            new ConfigDescription(option.Description)
        );

        if (option is IBindableOption<T> opt)
            opt.Bind(configEntry);
        
        var key = $"{option.Section}.{option.Key}";
        
        if (!_options.ContainsKey(key))
            _options.Add(key, option);
        
        if (!renderUi)
            return;
        
        switch (configEntry)
        {
            case ConfigEntry<bool> configEntryBool:
                ModSettingsManager.AddOption(new CheckBoxOption(configEntryBool,
                    optConfig as CheckBoxConfig ?? new CheckBoxConfig()));
                break;
            case ConfigEntry<float> configEntryFloat:
                ModSettingsManager.AddOption(new StepSliderOption(configEntryFloat,
                    optConfig as StepSliderConfig ?? new StepSliderConfig()));
                break;
        }
    }

    public void ConfigureGroup(IOptionGroup optionGroup, bool renderUi = true)
    {
        foreach (var (option, config) in optionGroup.GetOption())
        {
            typeof(ConfigManager)
                .GetMethod(nameof(ConfigureInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
                .MakeGenericMethod(option.Type)
                .Invoke(this, new object[] { option, config, renderUi });
        }
    }

    public OptionBase GetOption(string optionPath) => _options[optionPath];

    public bool TryGetOption(string optionPath, out OptionBase value)
    {
        value = null;
        
        try
        {
            return _options.TryGetValue(optionPath, out value);
        }
        catch (Exception e)
        {
            return false;
        }
    }
}