#if FACEBOOK

using System.Collections.Generic;

using IPS.Api.Analytics;
using Facebook.Unity;
using UnityEditor;
using UnityEngine;
using Facebook.Unity.Settings;
using System.IO;

namespace IPS.Editor {
    [CustomEditor(typeof(IPSFacebookSettings))]
    public class IPSFacebookEditor : UnityEditor.Editor {

        private const string SDKPath = "Assets/" + FacebookSettings.FacebookSettingsPath + "/ " + FacebookSettings.FacebookSettingsAssetName + ".asset";

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var settings = target as IPSFacebookSettings;

            GUILayout.Space(5);
            if (GUILayout.Button("Save", GUILayout.Height(30))) {
                var fb = FacebookSettings.Instance;// AssetDatabase.LoadAssetAtPath<FacebookSettings>(SDKPath);
                if (fb == null) {
                    Debug.LogError("Click Unity menu first: Facebook/ Edit Settings");
                    return;
                }
                FacebookSettings.AppLabels = settings.AppNames;
                FacebookSettings.AppIds = settings.AppIds;
                FacebookSettings.ClientTokens = settings.ClientTokens;
                FacebookSettings.SelectedAppIndex = settings.ActiveIndex;
                EditorUtility.SetDirty(fb);
                AssetDatabase.SaveAssets();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Verify Setting", GUILayout.Height(30))) {
                var fb = FacebookSettings.Instance;// AssetDatabase.LoadAssetAtPath<FacebookSettings>(SDKPath);
                if (fb == null) {
                    Debug.LogError("Click Unity menu first: Facebook/ Edit Settings");
                    return;
                }
                SelectFbSettings();
            }
        }

        public static void UpdateSelectedAppIndex() {
            if (FacebookSettings.AppIds.Count > 1) {
#if UNITY_IOS
                FacebookSettings.SelectedAppIndex = 1;
#else
                FacebookSettings.SelectedAppIndex = 0;
#endif
            }
            else FacebookSettings.SelectedAppIndex = 0;

            EditorUtility.SetDirty(FacebookSettings.Instance);
            AssetDatabase.SaveAssets();
        }

        public static void SelectFbSettings() {
            UnityEditor.Selection.activeObject = AssetDatabase.LoadAssetAtPath<FacebookSettings>(SDKPath);
        }

        [UnityEditor.MenuItem(ApiSettings.LIB_MENU + "/Api/Analytics/Facebook Settings")]
        public static void SelectIPSSettings() {
            string path = $"Assets/{ApiSettings.LIB_FOLDER}/Api/Analytics/IPSFacebook/Settings/{typeof(IPSFacebookSettings).Name}/";
            string assetPath = Path.Combine(path, $"{typeof(IPSFacebookSettings).Name}.asset");

            UnityEditor.Selection.activeObject = AssetDatabase.LoadAssetAtPath<IPSFacebookSettings>(assetPath);

            if (UnityEditor.Selection.activeObject == null) {
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<IPSFacebookSettings>();
                AssetDatabase.CreateAsset(ins, assetPath);
                AssetDatabase.SaveAssets();

                SelectIPSSettings();
            }
        }

#if UNITY_IOS
        // Automatically set the "Always Embed Swift Standard Libraries" option to "No" in UnityFramework Target in XCode
        [PostProcessBuildAttribute(Int32.MaxValue)] //We want this code to run last!
        public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuildProject) {
            if (buildTarget != BuildTarget.iOS) return; // Make sure its iOS build
            
            // Getting access to the xcode project file
            string projectPath = pathToBuildProject + "/Unity-iPhone.xcodeproj/project.pbxproj";
            UnityEditor.iOS.Xcode.PBXProject pbxProject = new UnityEditor.iOS.Xcode.PBXProject();
            pbxProject.ReadFromFile(projectPath);
            
            // Getting the UnityFramework Target and changing build settings
            string target = pbxProject.GetUnityFrameworkTargetGuid();
            pbxProject.SetBuildProperty(target, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");

            // After we're done editing the build settings we save it 
            pbxProject.WriteToFile(projectPath);
        }
#endif
    }
}
#endif
