#if UNITY_EDITOR
using UnityEngine;
using System.Reflection;
using UnityEditor;
using System;
#endif

namespace IPS {
#if UNITY_EDITOR
    [CustomEditor(typeof(SoundData))]
    public class SoundDataEditor : UnityEditor.Editor {

        FieldInfo[] fields;
        string[] soundId;
        int[] soundValue;

        static bool foldout;
        static bool[] foldoutContents;

        private void Init() {
            if (fields == null || fields.Length == 0) {
                if ((target as SoundData).data != null) Debug.Log("SoundData length=" + (target as SoundData).data.Length);
                fields = typeof(SoundEvent).GetFields(System.Reflection.BindingFlags.Static |
                                                    System.Reflection.BindingFlags.Public |
                                                    System.Reflection.BindingFlags.NonPublic);

                soundId = new string[fields.Length];
                soundValue = new int[fields.Length];

                for (int i = 0; i < fields.Length; ++i) {
                    if (fields[i].FieldType == typeof(int) && fields[i].IsLiteral) {
                        int value = (int)fields[i].GetValue(target as SoundData);
                        soundId[i] = $"{fields[i].Name} ({value})";
                        soundValue[i] = value;
                    }
                }
            }
        }

        public override void OnInspectorGUI() {
            var script = target as SoundData;

            Init();

            if (foldoutContents == null || foldoutContents.Length != script.data.Length) {
                foldoutContents = new bool[script.data.Length];
            }

            EditorGUILayout.BeginVertical();
            {

                EditorGUILayout.BeginHorizontal();
                {
                    foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, "Data");
                    EditorGUILayout.EndFoldoutHeaderGroup();

                    var length = EditorGUILayout.DelayedIntField(script.data.Length, GUILayout.Width(50), GUILayout.Height(20));
                    if (length >= 0 && length != script.data.Length) {
                        var newList = new SoundInfo[length];
                        bool[] foldContent = new bool[length];
                        for (int i = 0; i < length; ++i) {
                            if (i < script.data.Length) {
                                newList[i] = script.data[i];
                                foldContent[i] = foldoutContents[i];
                            }
                            else if (script.data.Length > 0) {
                                newList[i] = new SoundInfo();
                            }
                        }
                        script.data = newList;
                        foldoutContents = foldContent;

                        EditorUtility.SetDirty(script);
                        AssetDatabase.SaveAssets();
                    }

                    EditorGUILayout.EndHorizontal();
                }


                if (foldout) {
                    EditorGUILayout.BeginVertical();
                    {
                        for (int i = 0; i < script.data.Length; ++i) {
                            var item = script.data[i];

                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(string.Empty, GUILayout.Width(5));
                            bool show = i < foldoutContents.Length ? foldoutContents[i] : false;
                            foldoutContents[i] = EditorGUILayout.BeginFoldoutHeaderGroup(show, $"Event {soundId[Array.FindIndex(soundValue, i => i == item.eventId)]}");
                            EditorGUILayout.EndFoldoutHeaderGroup();
                            EditorGUILayout.EndHorizontal();

                            if (foldoutContents[i]) {
                                EditorGUILayout.BeginVertical();
                                {
                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("  Event ", GUILayout.Width(50));
                                    int id = EditorGUILayout.IntPopup(item.eventId, soundId, soundValue);
                                    if (id != item.eventId) {
                                        script.data[i].eventId = id;
                                        EditorUtility.SetDirty(script);
                                        AssetDatabase.SaveAssets();
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField("  Clip ", GUILayout.Width(50));
                                    var clip = EditorGUILayout.ObjectField(item.clip, typeof(AudioClip), false) as AudioClip;
                                    if (clip != item.clip) {
                                        script.data[i].clip = clip;
                                        EditorUtility.SetDirty(script);
                                        AssetDatabase.SaveAssets();
                                    }
                                    EditorGUILayout.EndHorizontal();

                                    EditorGUILayout.Space(10);
                                    EditorGUILayout.EndVertical();
                                }

                            }
                        }

                        EditorGUILayout.EndVertical();
                    }
                }

                EditorGUILayout.EndVertical();
            }

        }
    }
#endif
}