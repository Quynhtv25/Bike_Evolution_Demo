using System.IO;
using UnityEngine;

namespace IPS {

    [CreateAssetMenu(fileName = "SoundData", menuName = "_GAME/SFX/SoundData")]
    public partial class SoundData : SingletonResourcesScriptable<SoundData> {
        public SoundInfo[] data = new SoundInfo[0];

        protected override void Initialize() {

        }

        public AudioClip GetSound(int eventId) {
            return GetSound(data, eventId);
        }

        public AudioClip GetSound(SoundInfo[] arrayData, int eventId) {
            if (arrayData == null || arrayData.Length == 0) return null;
            var sound = System.Array.Find(arrayData, i => i.eventId == eventId);
            if (sound == null) return null;
            return sound.clip;
        }


#if UNITY_EDITOR
        [UnityEditor.MenuItem("GAME/DATA/Open Sound Data")]
        public static void OpenSoundDataFile() {
            if (SoundData.Instance == null) {
                string path = $"Assets/_GAME/SFX/Resources/{typeof(SoundData).Name}/";
                Directory.CreateDirectory(path);
                var ins = ScriptableObject.CreateInstance<SoundData>();
                string assetPath = Path.Combine(path, $"{typeof(SoundData).Name}.asset");
                UnityEditor.AssetDatabase.CreateAsset(ins, assetPath);
                UnityEditor.AssetDatabase.SaveAssets();
            }
            UnityEditor.Selection.activeObject = UnityEditor.AssetDatabase.LoadAssetAtPath<SoundData>($"Assets/_GAME/SFX/Resources/{typeof(SoundData).Name}/{typeof(SoundData).Name}.asset");
        }

#endif
    }

    [System.Serializable]
    public class SoundInfo {
        public int eventId;
        public AudioClip clip;
    }

    public partial class SFX {
        /// <summary> soundEventId in the class SoundEvent.cs </summary>
        public void PlaySound(int soundEventId, float volume = 1) {
            SFX.Instance.PlaySound(SoundData.Instance.GetSound(soundEventId), volume);
        }
        
        /// <summary> Play the sound which can be stop. The soundEventId in the class SoundEvent.cs
        /// <para>You should cache the return value of this method to manual stop the sound.</para>
        /// </summary>
        public int PlaySound(int soundEventId, bool loop, float volume = 1) {
            return SFX.Instance.PlaySound(SoundData.Instance.GetSound(soundEventId), loop, volume);
        }

        public int PlaySound(int soundEventIdNew,int soundEventIdOld, bool loop, bool blend,out AudioSource sfx, float volume = 1 ,float blendDuration = 1) {
           return SFX.Instance.PlaySound(SoundData.Instance.GetSound(soundEventIdNew), loop, out sfx,soundEventIdOld, volume,blendDuration);
        }
        /// <summary> eventId in the class SoundEvent.cs </summary>
        public void PlayBgMusic(int eventId, float volume = 1) {
            SFX.Instance.PlayBgMusic(SoundData.Instance.GetSound(eventId), volume);
        }
    }

}