using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using IPS;
using System.IO;
using System;

public class EditorUtils {
    [MenuItem("GAME/PLAY", priority = 10)]
    public static void Play() {
        Resources.UnloadUnusedAssets();
        GC.Collect();
#if GDPR
        OpenGDPRScene();
//#elif ADS || FIREBASE
#else
        OpenSplashScene();
        //OpenGameSene();
#endif
        EditorApplication.isPlaying = true;
    }

#if GDPR
    [MenuItem("GAME/Scenes/Open GDPR Scene", priority = 1)]
    public static void OpenGDPRScene() {
        EditorSceneManager.OpenScene($"Assets/{ApiSettings.LIB_FOLDER}/Api/GDPR/GDPR.unity");
    }
#endif

    //#if ADS || FIREBASE
    [MenuItem("GAME/Scenes/Open Splash Scene", priority = 1)]
    public static void OpenSplashScene() {
        EditorSceneManager.OpenScene($"Assets/_GAME/Scenes/SplashScene.unity");
    }

    //#endif
    [MenuItem("GAME/Scenes/Open Home Scene", priority = 4)]
    public static void OpenHomeSene() {
        EditorSceneManager.OpenScene("Assets/_GAME/Scenes/HomeScene.unity");
    }

    [MenuItem("GAME/Scenes/Open Game Scene", priority = 5)]
    public static void OpenGameSene() {
        EditorSceneManager.OpenScene("Assets/_GAME/Scenes/GameScene.unity");
    }

    [MenuItem("GAME/ClearAllData", priority = 1000)]
    public static void ClearAllData() {
        if (EditorUtility.DisplayDialog("Clear all", "Do you want to clear all data?", "Yes")) {
            UserData.DeleteAll();
        }
    }
}
