using BepInEx;
using Confluent.Kafka;
using R2API;
using R2API.Utils;
using RoR2;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Mod.Services;
using Sevriukoff.MetaRun.Mod.Utils;
using UnityEngine;

namespace Sevriukoff.MetaRun.Mod;

[BepInDependency(ItemAPI.PluginGUID)]
[BepInDependency(R2API.R2API.PluginGUID)]
[BepInDependency(LanguageAPI.PluginGUID)]
[BepInDependency("com.rune580.riskofoptions")]

[NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]

public class Main : BaseUnityPlugin
{
    public const string PluginGuid = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "HookAjor";
    public const string PluginName = "MetaRun";
    public const string PluginVersion = "0.4.0";

    private TrackerManager _trackerManager;
    private IProducer _producer;
    private ConfigManager _configManager;
    private AssetManager _assetManager;
    private AssetBundle _assetBundle;

    public void Awake()
    {
        string librdkafka =
            @"C:\Users\Bellatrix\AppData\Roaming\r2modmanPlus-local\RiskOfRain2\profiles\DevMod\BepInEx\plugins\Hook Ajor-MetaRun\librdkafka\x64\librdkafka.dll";
        Library.Load(librdkafka);

        _assetManager = new AssetManager();
        _configManager = new ConfigManager(_assetManager);
        _trackerManager = new(_configManager);
        _producer = new KafkaProducer("localhost:9092");
        EventMetaDataUtil.Init();

        if (!_configManager.TryGetOption("Misc.IsFirstStart", out var opt))
        {
            opt = new Option<bool>("Misc", "IsFirstStart", true){Description = ""};
            _configManager.Configure<bool>(opt, renderUi:false);
            
            //_producer.Produce();
        }
        
        On.RoR2.Run.Start += (orig, self) =>
        {
            var modName = Language.GetString("METARUN_TITLE");
            
            _trackerManager.OnEventTracked += OnOnEventTracked;
            _trackerManager.StartTracking();
            
            orig(self);
        };

        On.RoR2.Run.BeginGameOver += (orig, self, def) =>
        {
            _trackerManager.OnEventTracked -= OnOnEventTracked;
            _trackerManager.StopTracking();
            
            orig(self, def);
        };

        /*_trackerManager.ConfigureTracker<CharacterDamageTracker>
        (
            new TrackerOptions
            {
                IsActive = true,
                LingerMs = 1000,
                MaxEventSummation = 25
            }
        );
        
        _trackerManager.ConfigureTracker<CharacterMinionDamageTracker>
        (
            new TrackerOptions
            {
                IsActive = true,
                LingerMs = 1000,
                MaxEventSummation = 25
            }
        );
        
        _trackerManager.ConfigureTracker<CharacterMovedTracker>
        (
            new TrackerOptions
            {
                IsActive = false
            }
        );
        
        _trackerManager.ConfigureTracker<CharacterHealedTracker>
        (
            new TrackerOptions
            {
                IsActive = true,
                LingerMs = 3000,
                MaxEventSummation = 25
            }
        );*/
        
        /*_assetBundle = AssetBundle.LoadFromFile("C:\\Users\\Bellatrix\\Documents\\UnityProjects\\MetaRunUI\\ThunderKit\\Staging\\Unknown\\plugins\\Unknown\\metarunui.test");

        var cfg = new ConfigFile(Paths.ConfigPath + "\\MetaRun.cfg", true);
        var enableTracking = cfg.Bind
        (
            "CharacterDamageTracker",
            "Enable tracking damage",
            true,
            new ConfigDescription("If true the tracker will track the damage of the character", tags: "TrackerType")
        );

        enableTracking.SettingChanged += (sender, args) =>
        {
            
        };

        cfg.SettingChanged += (sender, args) =>
        {
            var trackerType = (string)args.ChangedSetting.Description.Tags[0];
        };
        
        ModSettingsManager.AddOption(new CheckBoxOption(enableTracking));
        ModSettingsManager.SetModIcon(_assetBundle.LoadAsset<Sprite>("Assets/MetaRunUIIcon.png"));
        ModSettingsManager.AddOption(new GenericButtonOption("About tracker", "CharacterDamageTracker", GetInfoAboutTracker));

        void GetInfoAboutTracker()
        {
            var modPanel = _assetBundle.LoadAsset<GameObject>("Assets/MetaRunUI.prefab");

            GameObject.Instantiate(modPanel);
        }*/
        
        var networkUsers = NetworkUser.instancesList;

        foreach (NetworkUser networkUser in networkUsers)
        {
            // Получение CharacterMaster каждого NetworkUser
            CharacterMaster characterMaster = networkUser.master;

            if (characterMaster != null)
            {
                // Получение CharacterBody, если это необходимо
                CharacterBody characterBody = characterMaster.GetBody();

                if (characterBody != null)
                {
                    // Теперь у вас есть доступ к каждому персонажу в забеге
                    // Вы можете выполнять с ними нужные операции
                }
            }
        }
    }
    
    private void Update()
    {
        
    }

    private void OnOnEventTracked(EventMetaData eventMetaData)
    {
        _producer.Produce(eventMetaData);
    }
}