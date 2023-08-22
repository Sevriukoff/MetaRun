using System;
using System.Text.Json;
using BepInEx;
using Confluent.Kafka;
using R2API;
using RoR2;
using Sevriukoff.MetaRun.Mod.Base;

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

    private readonly TrackerContainer _trackerContainer = new();
    private readonly IProducer _producer = new KafkaProducer();
    
    public void Awake()
    {
        string librdkafka =
            @"C:\Users\Bellatrix\AppData\Roaming\r2modmanPlus-local\RiskOfRain2\profiles\DevMod\BepInEx\plugins\Hook Ajor-MetaRun\librdkafka\x64\librdkafka.dll";
        Library.Load(librdkafka);

        _trackerContainer.OnEventTracked += o => _producer.Produce(JsonSerializer.Serialize(o));
        _trackerContainer.StartTracking();
    }

    private void Update()
    {
        
    }
}