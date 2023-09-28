using System;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using RiskOfOptions;
using RiskOfOptions.OptionConfigs;
using RiskOfOptions.Options;
using UnityEngine;

namespace Sevriukoff.MetaRun.Mod.Base;

public class ConfigManager
{
    public readonly ConfigFile Config;

    public ConfigManager(AssetManager assetManager)
    {
        Type[] types = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(x => x.IsSubclassOf(typeof(BaseEventTracker)))
            .ToArray();
        
        Config = new ConfigFile(Paths.ConfigPath + "\\HookAjor-MetaRun.cfg", true);
        
        ModSettingsManager.SetModDescription("MetaRun представляет расширенную статистику о забеге в режиме реального времени." +
                                             " Статистика поможет понять кто из твоих друзей больше ворует шмот и пылесосит всю карту." +
                                             " Если без шуток, то MetaRun поможет понять на каком этапе у тебя просел урон или какого типа урона тебе не хватает," +
                                             " какой объём у тебя хила, насколько тщательно ты исследуешь этапы и многое другое.");
        ModSettingsManager.SetModIcon(assetManager.ModIcon);

        foreach (var trackerType in types)
        {
            var isEnable =  Config.Bind
            (
                trackerType.Name,
                "Включить отслеживание",
                true,
                new ConfigDescription(
                    $"Можно выключить отслеживание событий типа: [{trackerType.Name.Remove(trackerType.Name.Length - 7)}]",
                    tags: trackerType)
            );
            ModSettingsManager.AddOption(new CheckBoxOption(isEnable));

            if (!trackerType.Name.StartsWith("Run"))
            {
                var trackOnlyHost = Config.Bind
                (
                    trackerType.Name,
                    "Отслеживать только хоста",
                    false,
                    new ConfigDescription(
                        "При включении опции трекер будет отслеживать только хоста. Может помочь снизить нагрузку при полном лобби",
                        tags: trackerType)
                );
                ModSettingsManager.AddOption(new CheckBoxOption(trackOnlyHost));
            }

            var maxEventSummation = Config.Bind
            (
                trackerType.Name,
                "Кол-во событий для суммирования",
                0f,
                new ConfigDescription(
                    "Сколько событий могут быть объедены в одно за время хранения сообщений в буфере перед отправкой на сервер. Позволяет снизить нагрузку при большом количестве собыйтий в секунду. Особенно полезно для трекера передвижения",
                    tags: trackerType)
            );
            ModSettingsManager.AddOption(new StepSliderOption(maxEventSummation, new StepSliderConfig{min = 0, max = 255, increment = 5f}));
            
            var lingerMs = Config.Bind
            (
                trackerType.Name,
                "Время хранения сообщений в мс",
                0f,
                new ConfigDescription(
                    "Сколько милисекунд будут храниться события во внутреннем буфере трекера перед отправкой на сервер. Отдельно это параметр практически не имеет значения. Рекомендуется использовать вместе с параметром \"Кол-во событий для суммирования\"",
                    tags: trackerType)
            );
            ModSettingsManager.AddOption(new StepSliderOption(lingerMs, new StepSliderConfig{min = 0, max = 60000, increment = 100f}));
        }
    }
}