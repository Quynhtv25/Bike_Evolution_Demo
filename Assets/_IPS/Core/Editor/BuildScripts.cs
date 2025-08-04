//#define HIGAME

using IPS;
using IPS.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public class BuildScript {
    //[MenuItem("IPS/Build/Build Jenskin IOS")]
    static void PerformBuildIOS() {
        BuildPipeline.BuildPlayer(GetEnabledScenes(), "./builds/IOS",
          BuildTarget.iOS, BuildOptions.None);
    }

    static string[] GetEnabledScenes() {
        return (
          from scene in EditorBuildSettings.scenes
          where scene.enabled
          where !string.IsNullOrEmpty(scene.path)
          select scene.path
        ).ToArray();
    }

    [MenuItem("IPS/Build/Log/Turn ON Log")]
    static void TurnOnLog() {
        SetupFullLog(true);
    }

    [MenuItem("IPS/Build/Log/Turn OFF Log")]
    static void TurnOffLog() {
        SetupFullLog(false);
    }
    [MenuItem("IPS/Build/Cheat/Turn ON No ads")]
    static void TurnOnNoAd() {
        SetupNoAds(true);
    }

    [MenuItem("IPS/Build/Cheat/Turn OFF NoAds")]
    static void TurnOffNoAd() {
        SetupNoAds(false);
    }

    [MenuItem("IPS/Build/Cheat/Build Cheat")]
    static void BuildCheat() {
#if UNITY_ANDROID
        SetupAAB(false);
#endif
        SetupNoAds(false);
#if UNITY_ANDROID
        PerformBuildAndroid();
#endif
    }

    [MenuItem("IPS/Build/Cheat/Build Cheat Ads Test")]
    static void BuildCheatAdsTest() {
#if UNITY_ANDROID
        SetupAAB(false);
#endif
        SetupNoAds(false);
        SetupAdTest(true);
#if UNITY_ANDROID
        PerformBuildAndroid();
#endif
    }

    [MenuItem("IPS/Build/Cheat/Build Cheat NO Ads")]
    static void BuildCheatNoAds() {
#if UNITY_ANDROID
        SetupAAB(false);
#endif
        //SetupAdTest(false);
        SetupNoAds(true);
#if UNITY_ANDROID
        PerformBuildAndroid();
#endif
    }

    [MenuItem("IPS/Build/Production/Build Production APK")]
    static void BuildProductionAPK() {
#if UNITY_ANDROID
        SetupAAB(false);
#endif
        SetupProduction(true);
        SetupFullLog(false);
        SetupAdTest(false);
        SetupNoAds(false);
#if UNITY_ANDROID
        PerformBuildAndroid();
#endif
    }


    [MenuItem("IPS/Build/Production/Build Production AAB")]
    static void BuildProductionAAB() {
#if UNITY_ANDROID
        SetupAAB(true);
#endif
        SetupFullLog(false);
        SetupAdTest(false);
        SetupNoAds(false);
#if UNITY_ANDROID
        PerformBuildAndroid();
#endif
    }

    [MenuItem("IPS/Build/Build Addressables Only")]
    public static bool BuildAddressables() {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        var group = settings.DefaultGroup;
        AddressableAssetSettings.CleanPlayerContent(settings.ActivePlayerDataBuilder);
        settings.ActivePlayerDataBuilder.ClearCachedData();

        AddressableAssetSettings.BuildPlayerContent(out AddressablesPlayerBuildResult result);
        bool success = string.IsNullOrEmpty(result.Error);

        if (!success) {
            Debug.LogError("Addressables build error encountered: " + result.Error);
        }

        return success;
    }


    //[MenuItem("IPS/Build/Build Jenskin Android")]
    static void PerformBuildAndroid() {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
#if ADS
        AdmobEditor.SaveAdmobConfig();
#endif

        // target android
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4;
        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        // setup enabled scenes
        buildPlayerOptions.scenes = GetEnabledScenes();

        // SetScriptingBackend for IL2CPP
        PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

        // Support ARMv7 and ARMv64
        PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARM64 | AndroidArchitecture.ARMv7;

        SetupParameter();
        SetupKeystore();
        // app name

        var burst = BurstCompiler.Options;//.GetOrCreateSettings(buildTarget)
        burst.EnableBurstCompilation = is_production;

        buildPlayerOptions.locationPathName = BuildName();

        bool buildAddressable = false;
        bool buildAddressableSuccess = false;
        if (GetEnv(RebuildAddressable, out var rebuildAddressable)) {
            if (rebuildAddressable == "true") {
                buildAddressable = true;
                buildAddressableSuccess = BuildAddressables();
            }
        }

        if (!buildAddressable || buildAddressableSuccess) {
            IPS.BootstrapConfig.Instance.VersionCode = PlayerSettings.Android.bundleVersionCode;

            var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

            var summary = buildReport.summary;

            if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) {
                Debug.LogError($"Build Failed with status: {summary.result}");
                throw new Exception($"Failed to build Android with {summary.result} status");
            }

            Debug.Log($"Build Successfully file= {buildPlayerOptions.locationPathName}, size={summary.totalSize /1000000f}MB, total time={(int)summary.totalTime.TotalSeconds}s");
            Console.WriteLine("Build Successfully file: " + buildPlayerOptions.locationPathName + ", size: " + summary.totalSize);
        }
    }

    static void SetupKeystore() {
        if (!is_aab) {
            PlayerSettings.Android.useCustomKeystore = false;
        }
        else {
            PlayerSettings.Android.useCustomKeystore = true;
#if HIGAME
            string key = "higame-Minder";
            PlayerSettings.Android.keystoreName = $"{key}.keystore";
            PlayerSettings.Android.keystorePass = key;
            PlayerSettings.Android.keyaliasName = key;
            PlayerSettings.Android.keyaliasPass = key;
#else
            PlayerSettings.Android.keystoreName = $"{Application.identifier}.keystore";
            PlayerSettings.Android.keystorePass = "123456";
            PlayerSettings.Android.keyaliasName = $"{Application.identifier}";
            PlayerSettings.Android.keyaliasPass = "123456";
#endif
        }
    }

    static void SetupParameter() {
        Debug.Log("Set up parameter");

        if (GetEnv(BundleKey, out var bundle)) {
            int.TryParse(bundle, out var bundleID);
            if (bundleID != -1) {
                PlayerSettings.Android.bundleVersionCode = bundleID;
            }
        }

        if (GetEnv(VersionKey, out var buildVersion)) {
            if (buildVersion != "") {
                PlayerSettings.bundleVersion = buildVersion;
            }
        }
        
        if (GetEnv(IsAAB, out var isaab)) {
            SetupAAB(isaab == "true");
        }

        if (!is_production && GetEnv(ProductionBuild, out var param)) {
            SetupProduction(param == "true");
        }

        if (GetEnv(RemoveAdsBuild, out var paramRemoveAds)) {
            SetupNoAds(!is_production && paramRemoveAds == "true");
        }

        if (GetEnv(DevelopmentBuild, out var isDevelopmentBuild)) {
            SetupFullLog(isDevelopmentBuild == "true");
        }

#if ADS
        if (GetEnv(UseAdTest, out var result)) {
            SetupAdTest(!is_aab && result == "true");
        }
#endif
    }

    static void SetupAAB(bool aab) {
        BootstrapConfig.Instance.IsAAB = aab;
        UnityEditor.EditorUtility.SetDirty(BootstrapConfig.Instance);
        UnityEditor.AssetDatabase.SaveAssets();

        is_aab = aab;
        EditorUserBuildSettings.buildAppBundle = is_aab;
        SetupProduction(is_aab);
    }

    static void SetupProduction(bool production) {
        is_production = production;
        UpdateSymbols(new KeyValuePair<string, bool>(IPS.ApiSettings.ProductionDefine, is_production));
    }

    static void SetupFullLog(bool fullLog) {
        EditorUserBuildSettings.development = fullLog;
        UpdateSymbols(new KeyValuePair<string, bool>(IPS.ApiSettings.DebugDefine, fullLog));
    }

    static void SetupNoAds(bool noAds) {
        is_removeads = noAds;
        UpdateSymbols(new KeyValuePair<string, bool>(IPS.ApiSettings.NoAdDefine, is_removeads));
    }

    static void SetupAdTest(bool test) {
#if ADS
        useAdTest = test;
        if (useAdTest) {
            AdmobEditor.SaveConfigForAdTest();
#if MAX
            MaxSettingsEditor.TurnOffMaxForTestAdLogic();
#endif
#if IS
            IronSourceSettingsEditor.SaveConfig();
#endif
        }
#endif
    }
    
    static string BuildName() {
        string debug = is_production ? "production" : "cheat";
        if (is_removeads) debug += "_noads";
        if (useAdTest) debug += "_adtest";
        if (EditorUserBuildSettings.development) debug += "_fulllog";

        return $"Builds/{Application.productName.Replace(" ", string.Empty).Replace(":", string.Empty)}_v{Application.version}" +
               $"_c{PlayerSettings.Android.bundleVersionCode}" +
               $"_{debug}_{DateTime.Now:ddMMyyyy_HHmm}." +
               (is_aab ? "aab" : "apk");
    }

    private static BuildTargetGroup Platform {
        get {
#if UNITY_ANDROID
            return BuildTargetGroup.Android;
#elif UNITY_IOS
                return BuildTargetGroup.iOS;
#else
                return BuildTargetGroup.Standalone;
#endif
        }
    }
    public static void UpdateSymbols(params KeyValuePair<string, bool>[] symbols) {
        if (symbols == null || symbols.Length == 0) return;

        var list = GetCurrentSymbols();

        foreach (var item in symbols) {
            UpdateFlag(list, item.Key, item.Value);
        }

        SaveSymbols(list);
    }

    static List<string> GetCurrentSymbols() {
        string flagString = PlayerSettings.GetScriptingDefineSymbolsForGroup(Platform);
        return new List<string>(flagString.Split(';'));
    }

    static void SaveSymbols(List<string> list) {
        string newFlags = string.Join(";", list);
        Debug.Log($"SetScriptingDefineSymbols: {newFlags}");
        PlayerSettings.SetScriptingDefineSymbolsForGroup(Platform, newFlags);
    }

    static void UpdateFlag(List<string> currentList, string flag, bool enable) {
        if (enable && !currentList.Contains(flag)) {
            currentList.Add(flag);
        }
        else if (!enable && currentList.Contains(flag)) {
            currentList.Remove(flag);
        }
    }

    static void BuildIOS() {
        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();

        // app name
        buildPlayerOptions.locationPathName = BuildName();

        // target iOS
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.CompressWithLz4HC;

        // setup enabled scenes
        buildPlayerOptions.scenes = GetEnabledScenes();

        var buildReport = BuildPipeline.BuildPlayer(buildPlayerOptions);

        if (buildReport.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            throw new Exception($"Failed to build IOS with {buildReport.summary.result} status");

        var summary = buildReport.summary;
        Console.WriteLine("Succeed to create " + buildPlayerOptions.locationPathName + " outputPath: " + summary.outputPath);
    }


    static bool GetEnv(string key, out string value) {
        value = Environment.GetEnvironmentVariable(key);
        return !string.IsNullOrEmpty(value);
    }

    // Version
    private const string VersionKey = "version";
    private const string BundleKey = "bundle_id";
    private const string RebuildAddressable = "rebuildAddressable";
    private const string RemoveAdsBuild = "removeAdsBuild";
    private const string UseAdTest = "useAdTest";
    private const string DevelopmentBuild = "developmentBuild";
    private const string ProductionBuild = "productionBuild";
    private const string IsAAB = "AAB";
    private static bool is_aab = false;
    private static bool is_production = false;
    private static bool is_removeads = false;
    private static bool useAdTest = false;

}