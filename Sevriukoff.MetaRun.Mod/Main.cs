using System;
using BepInEx;
using Confluent.Kafka;
using R2API;
using RoR2;

namespace Sevriukoff.MetaRun.Mod;

[BepInDependency(ItemAPI.PluginGUID)]
[BepInDependency(R2API.R2API.PluginGUID)]
[BepInDependency(LanguageAPI.PluginGUID)]

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]

public class Main : BaseUnityPlugin
{
    public const string PluginGuid = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "HookAjor";
    public const string PluginName = "MetaRun";
    public const string PluginVersion = "0.0.1";
    
    public void Awake()
    {
        string librdkafka =
            @"C:\Users\Bellatrix\AppData\Roaming\r2modmanPlus-local\RiskOfRain2\profiles\DevMod\BepInEx\plugins\Hook Ajor-MetaRun\librdkafka\x64\librdkafka.dll";
        Library.Load(librdkafka);

        On.RoR2.HealthComponent.TakeDamage += (orig, self, info) =>
        {
            orig(self, info);
        };

        On.RoR2.HealthComponent.Heal += (orig, self, amount, mask, regen) =>
        {
            return orig(self, amount, mask, regen);
        };

        GlobalEventManager.onCharacterDeathGlobal += report =>
        {

        };
    }

    private void Update()
    {
        
    }
}