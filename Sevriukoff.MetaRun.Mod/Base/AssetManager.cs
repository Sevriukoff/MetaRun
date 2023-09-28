using System.Reflection;
using UnityEngine;

namespace Sevriukoff.MetaRun.Mod.Base;

public class AssetManager
{
    public Sprite ModIcon { get; set; }
    
    private AssetBundle _assetBundle;
    
    public AssetManager()
    {
        _assetBundle = LoadAssetBundle("metarunui");

        ModIcon = _assetBundle.LoadAsset<Sprite>("Assets/MetaRunUIIcon.png");
    }

    private AssetBundle LoadAssetBundle(string bundleName)
    {
        using var resourceStream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream($"Sevriukoff.MetaRun.Mod.Resources.AssetBundles.{bundleName}");
        //return AssetBundle.LoadFromStream(resourceStream); //TODO: Fix Asset load
        return AssetBundle.LoadFromFile("C:\\Users\\Bellatrix\\Documents\\UnityProjects\\MetaRunUI\\ThunderKit\\Staging\\Unknown\\plugins\\Unknown\\metarunui");

        //_assetBundle = AssetBundle.LoadFromFile("C:\\Users\\Bellatrix\\Documents\\UnityProjects\\MetaRunUI\\ThunderKit\\Staging\\Unknown\\plugins\\Unknown\\metarunui");
    }
}