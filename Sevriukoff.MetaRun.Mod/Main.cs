using System;
using System.Threading.Tasks;
using BepInEx;
using Confluent.Kafka;
using R2API;
using R2API.Utils;
using RoR2;
using Sevriukoff.MetaRun.Domain;
using Sevriukoff.MetaRun.Domain.Base;
using Sevriukoff.MetaRun.Mod.Base;
using Sevriukoff.MetaRun.Mod.Trackers.Character;
using Sevriukoff.MetaRun.Mod.Utils;

namespace Sevriukoff.MetaRun.Mod;

[BepInDependency(ItemAPI.PluginGUID)]
[BepInDependency(R2API.R2API.PluginGUID)]
[BepInDependency(LanguageAPI.PluginGUID)]

[NetworkCompatibility(CompatibilityLevel.NoNeedForSync, VersionStrictness.DifferentModVersionsAreOk)]

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]

public class Main : BaseUnityPlugin
{
    public const string PluginGuid = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "HookAjor";
    public const string PluginName = "MetaRun";
    public const string PluginVersion = "0.1.0";

    private TrackerManager _trackerManager;
    private IProducer _producer;
    
    public void Awake()
    {
        string librdkafka =
            @"C:\Users\Bellatrix\AppData\Roaming\r2modmanPlus-local\RiskOfRain2\profiles\DevMod\BepInEx\plugins\Hook Ajor-MetaRun\librdkafka\x64\librdkafka.dll";
        Library.Load(librdkafka);
        
        _trackerManager = new();
        _producer = new KafkaProducer("localhost:9092");
        EventMetaDataUtil.Init();

        On.RoR2.Run.Start += (orig, self) =>
        {
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

        _trackerManager.ConfigureTracker<CharacterDamageTracker>
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
        );

        _assetBundle = AssetBundle.LoadFromFile("C:\\Users\\Bellatrix\\RiderProjects\\Sevriukoff.MetaRun\\Sevriukoff.MetaRun.Mod\\Unity\\betteruiassets");
        _modPanelPrefab = _assetBundle.LoadAsset<GameObject>($"Assets/ModPanel.prefab");
        _modSettingPrefab = _assetBundle.LoadAsset<GameObject>($"Assets/BetterUIWindow.prefab");

        //On.RoR2.UI.MainMenu.BaseMainMenuScreen.Awake += (orig, self) =>
        //{
        //    _mainMenu = self.transform;
        //    var transform = self.transform.Find("SafeZone/GenericMenuButtonPanel");
        //    var DescriptionGameObject = self.transform.Find("SafeZone/GenericMenuButtonPanel/JuicePanel/DescriptionPanel, Naked/ContentSizeFitter/DescriptionText");

        //    if (transform != null && DescriptionGameObject != null)
        //    {
        //        var modPanel = GameObject.Instantiate(_modPanelPrefab, transform);
        //        var button = modPanel.GetComponentInChildren<HGButton>();
        //        button.onClick.AddListener(OnClickModButton);
        //    }

        //    orig(self);
        //};

        On.RoR2.UI.PauseScreenController.Awake += (orig, self) =>
        {
            orig(self);

            var transform = self.transform.Find("SafeZone/GenericMenuButtonPanel");
            transform = self.mainPanel.transform.Find("SafeZone/GenericMenuButtonPanel");
            transform = self.rootPanel.transform.Find("SafeZone/GenericMenuButtonPanel");

            GameObject.Instantiate( _modPanelPrefab, self.rootPanel.transform);
        };

        On.RoR2.UI.PauseScreenController.OpenSettingsMenu += (orig, self) =>
        {
            orig(self);
        };
        
        /*_trackerManager.ConfigureTracker<CharacterMovedTracker>
        (
            new TrackerOptions
            {
                IsActive = false,
            }
        );

        _trackerManager.ConfigureTracker<CharacterMinionDamageTracker>
        (
            new TrackerOptions
            {
                IsActive = false,
            }
        );

        _trackerManager.ConfigureTracker<CharacterHealedTracker>
       (
           new TrackerOptions
           {
               IsActive = false,
           }
       );*/

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