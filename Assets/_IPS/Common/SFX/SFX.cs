using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace IPS {
    public partial class SFX : SingletonBehaviourResourcesDontDestroy<SFX> {
        [SerializeField] private AudioSource audioSourceBMG;
        [SerializeField] private AudioSource audioSourceSFX;

        float fadeTime = 0.5f;

        private Dictionary<int, AudioSource> soundSFXs = new Dictionary<int, AudioSource>();
        private static int _soundIdIncremental = 0;

        private int SoundIdIncremental {
            get { 
                _soundIdIncremental++;
                return _soundIdIncremental; 
            }
        }

        private void OnSceneUnLoaded(Scene scene) {
            foreach(var s in soundSFXs.Keys) {
                StopSound(soundSFXs[s]);
            }
            soundSFXs.Clear();
            _soundIdIncremental = 0;
        }

        protected override void OnAwake() {
            Vibration.Init();
            PlayBgMusic();
            audioSourceSFX.RegisterPool();
            SceneManager.sceneUnloaded += OnSceneUnLoaded;
        }

        public bool MusicEnable {
            get => PlayerPrefs.GetInt("MusicEnable", 1) == 1;
            set {
                PlayerPrefs.SetInt("MusicEnable", value ? 1 : 0);
                if (value) PlayBgMusic();
                else PauseBgMusic();
            }
        }

        public bool SoundEnable {
            get => PlayerPrefs.GetInt("SoundEnable", 1) == 1;
            set => PlayerPrefs.SetInt("SoundEnable", value ? 1 : 0);
        }

        public bool VibrateEnable {
            get => PlayerPrefs.GetInt("VibrateEnable", 1) == 1;
            set => PlayerPrefs.SetInt("VibrateEnable", value ? 1 : 0);
        }
        
        /// <summary>
        /// Vibrate with very small haptic feedback on Android, normal vibration on iOS.
        /// </summary>
        public void VibrateHaptic() {
#if UNITY_EDITOR
            return;
#endif
            if (!VibrateEnable) return;
            try {
#if UNITY_ANDROID
                Vibration.VibrateAndroid(20, 1);
#else
                Vibration.VibratePop();
#endif
            }
            catch { }
        }

        public void VibrateMedium() {
#if UNITY_EDITOR
            return;
#endif
            if (!VibrateEnable) return;
            try {
                Vibration.VibratePop();
            }
            catch { }
        }

        public void VibrateHeavy() {
#if UNITY_EDITOR
            return;
#endif
            if (!VibrateEnable) return;
            try {
                Vibration.VibratePeek();
            }
            catch { }
        }
        
        public void Vibrate(long milliseconds) {
#if UNITY_EDITOR
            return;
#endif
            if (!VibrateEnable) return;
            try {
#if UNITY_ANDROID
                Vibration.VibrateAndroid(milliseconds);
#elif UNITY_IOS
                Vibration.Vibrate();
#endif
            }
            catch { }
        }

        private void PlayBgMusic() {
            PlayBgMusic(audioSourceBMG.clip, audioSourceBMG.volume);
        }

        public void PlayBgMusic(AudioClip bgm, float volume = 1) {
            if (audioSourceBMG == null) return;

            if (audioSourceBMG.isPlaying && audioSourceBMG.clip != null && audioSourceBMG.clip != bgm) {
                audioSourceBMG.DOKill();
                audioSourceBMG.DOFade(0, fadeTime).OnComplete(() => {
                    audioSourceBMG.clip = bgm;
                    audioSourceBMG.DOFade(volume, fadeTime);
                    if (MusicEnable) audioSourceBMG.Play();
                });
            }
            else {
                audioSourceBMG.clip = bgm;
                audioSourceBMG.volume = volume;
                if (MusicEnable) audioSourceBMG.Play();
            }
        }

        public void PauseBgMusic() {
            if (audioSourceBMG != null) {
                audioSourceBMG.Pause();
            }
        }

        public void StopBgMusic() {
            if (audioSourceBMG == null) return;
            audioSourceBMG.DOKill();
            audioSourceBMG.Stop();
        }

        /// <summary> Sound will be play one shot and forget, cannot manual stop it</summary>
        public void PlaySound(AudioClip clip, float volume = 1) {
            if (!SoundEnable || clip == null || audioSourceSFX == null) return;
            audioSourceSFX.PlayOneShot(clip, volume);
        }

        /// <summary> Sound will be play with new audioSource, you can save this return value then manual call stop</summary>
        public int PlaySound(AudioClip clip, bool loop, out AudioSource sfx,int soundId , float volume = 1,float blendDuration =1) {
            sfx = null;
            int id = -1;
            if (soundSFXs.TryGetValue(soundId, out sfx)) {
                id = PlaySound(clip, sfx, loop, true,true,soundId,id, volume,blendDuration);
            }
            else {
                id = PlaySound(clip, sfx, loop, true, false, soundId, id, volume,blendDuration);
            }
            return id;
        }

        public int PlaySound(AudioClip clip, bool loop, float volume = 1,bool blend = false) {
            if (!SoundEnable || clip == null || audioSourceSFX == null) return -1;
            var sfx = audioSourceSFX.Spawn();
            int id = SoundIdIncremental;
            soundSFXs.Add(id, sfx);
            sfx.name = $"SFX-{id}-{clip.length}";
            sfx.clip = clip;
            sfx.loop = loop;
            sfx.volume = volume;
            sfx.time = 0;
            sfx.Play();

            if (!loop) {
                StartCoroutine(IEWaitForStopSFX(id, sfx));
            }
            return id;
        }
        /// <summary> Sound will be play with new audioSource and mix 2 audio sources together</summary>

        public int PlaySound(AudioClip clip,AudioSource audioSourceOld, bool loop, bool blend,bool hasOld, int idOld, int idNew, float volume = 1,float blendDuration = 1f) {
            if (!SoundEnable || clip == null || audioSourceSFX == null) return - 1;
            var sfx = Instantiate(audioSourceSFX, transform);
            //var sfx = audioSourceSFX.Spawn();
            int id = SoundIdIncremental;
            soundSFXs.Add(id, sfx);
            sfx.name = $"SFX-{id}-{clip.name}-{clip.length}";
            sfx.clip = clip;
            sfx.loop = loop;
            sfx.volume = volume;
            sfx.time = 0;
            StartCoroutine(CrossFade(audioSourceOld, sfx,blendDuration,sfx.clip.length, hasOld,loop, idOld, idNew));
            return id;
        }
        private IEnumerator IEWaitForStopSFX(int id, AudioSource audioSource = null) {
            if (audioSource == null) yield break;

            yield return Yielder.Wait(audioSource.clip.length);
            StopSound(audioSource);
            soundSFXs.Remove(id);
        }
        

        private IEnumerator CrossFade(AudioSource soundOld, AudioSource soundNew, float blendDuration,float endFadeDuration,bool hasOld,bool loop, int idOld, int idNew) {
            float elapse = 0;
            float oldVolume = 0;
            if (soundOld != null) {
                 oldVolume = soundOld.volume;
            }
            float newVolume = soundNew.volume;

            soundNew.Play();
            //soundNew.volume = 0; // TODO: prepare for fadein

            while (elapse < blendDuration) {
                elapse += Time.deltaTime;
                float t = elapse / blendDuration;
                if(soundNew != null) {
                    soundNew.volume = Mathf.Lerp(0, newVolume, t); // fade in new sound
                }
                if(soundOld != null) {
                    soundOld.volume = Mathf.Lerp(oldVolume, 0, t); // fade out old sound
                }
                yield return null;
            }
            if (soundNew != null) {
                if (!loop) {
                    //StopSound(soundNew);
                    Destroy(soundNew.gameObject, endFadeDuration);
                }
            }
            if (soundOld != null) {
                Destroy(soundOld.gameObject);
                //StopSound(soundOld);
            }
        }
        /// <summary>
        /// NOTE: soundId is the value which return by PlaySound with loop param, not same soundEventId in the SoundEvents.cs
        /// <para>You need to cache the return value of that method to call this method.</para>
        /// </summary>
        public void StopSound(int soundId) {
            if (soundSFXs.TryGetValue(soundId, out var audioSource)) {
                StopSound(audioSource);
                soundSFXs.Remove(soundId);
            }
        }

        private void StopSound(AudioSource audioSource) {
            if (audioSource == null) return;
            audioSource.Stop();
            audioSource.clip = null;
            audioSource.loop = false;
            audioSource.Recycle();
        }

        public void StopCurrentSound() {
            //if (soundSFXs.Count == 0) return;
            //int idx = soundSFXs.Count - 1;
            //StopSound(soundSFXs.);
            //soundSFXs.Remove(idx);
        }
        public void StopSound(bool isPause) {
            AudioListener.pause = isPause; 
        }
    }
}