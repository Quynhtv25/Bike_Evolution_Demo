
#if FIREBASE && FCS
using Firebase.Extensions;
using Firebase.Storage;
using IPS.Api.IPSFirebase;
#endif

using System;

namespace IPS {
    public class CloudStorage : SingletonBehaviourDontDestroy<CloudStorage> {
        private bool available;
        private event System.Action callOnAvailable;


#if FIREBASE && FCS
        /// <summary> Cannot upload data with a reference to the root of your Cloud Storage bucket. Your reference must point to a child URL </summary>
        private StorageReference rootStorage;
#endif

        protected override void OnAwake() {
            available = false;
#if FIREBASE && FCS
            IPSFirebaseCore.Instance.InitModule(InitCloudStorage);
#else
            DebugLog("Flag FIREBASE & FCS was not set for Firebase Cloud Storage");
#endif
        }

        private void InitCloudStorage() {
#if FIREBASE && FCS
            if (available) return;
            rootStorage = FirebaseStorage.DefaultInstance.RootReference;
            DebugLog("Cloud Storage Initialized.");
            available = true;
            CallOnAvailable(callOnAvailable);
#endif
        }

        private void CallOnAvailable(Action task) {
            if (available) Excutor.Schedule(task);
            else callOnAvailable += task;
        }

        private void DebugLog(string message, bool error = false) {
            if (!error) {
                Logs.Log($"[FIREBASE-FCS] {message}");
            }
            else {
                Logs.LogError($"[FIREBASE-FCS] {message}");
            }
        }

#if FIREBASE && FCS
        private StorageReference GetChild(string childPath) {
            if (rootStorage != null) {
                var d = rootStorage.Child(childPath);
                if (d != null) return d;
                else DebugLog($"Cloud child path does not exist: {childPath}");
            }
            return null;
        }
#endif

        #region DOWNLOAD
        /// <summary>
        /// Download data of a file from storage to URI, then continue use a URL with Unity's WWW or UnityWebRequest. 
        /// </summary>
        /// <param name="cloudChildPath">Example: "images/demo.jpg"</param>
        /// <param name="callback">The callback using WWW or UnityWebRequest to continue download</param>
        public void Download(string cloudChildPath, Action<Uri> callback) {
#if FIREBASE && FCS
            if (available) {
                Uri result = null;

                var d = GetChild(cloudChildPath);
                if (d != null) {
                    d.GetDownloadUrlAsync().ContinueWithOnMainThread(task => {
                        if (task.IsCompleted) {
                            DebugLog($"Downloaded data={task.Result}");
                            result = task.Result;
                        }
                        else {
                            DebugLog($"Download failed err={task.Exception}", true);
                        }
                    });
                }

                if (callback != null) callback.Invoke(result);
            }
            else CallOnAvailable(() => { Download(cloudChildPath, callback); });
#else
            DebugLog("Turn on `Use Cloud Storage` in `ApiSettings` first");
#endif
        }

        /// <summary>
        /// Download a file from cloud into a byte buffer in memory with a maximum allowed size
        /// </summary>
        /// <param name="cloudChildPath">Example: "images/demo.jpg"</param>
        /// <param name="maxAllowSize">1024 mean max size of the file is 1MB (1 * 1024 * 1024 bytes)</param>
        public void Download(string cloudChildPath, int maxAllowSize = 1024, Action<byte[]> callback = null) {
#if FIREBASE && FCS
            if (available) {
                byte[] data = null;
                var d = GetChild(cloudChildPath);
                if (d != null) {
                    long allowedSize = 1 * maxAllowSize * maxAllowSize;
                    d.GetBytesAsync(allowedSize).ContinueWithOnMainThread(task => {
                        if (task.IsFaulted || task.IsCanceled) {
                            DebugLog($"Download failed error = {task.Exception}", true);
                        }
                        else {
                            DebugLog("Finished downloading!");
                            data = task.Result;
                        }
                    });
                }

                if (callback != null) callback.Invoke(data);
            }
            else CallOnAvailable(() => { Download(cloudChildPath, maxAllowSize, callback); });
#else
            DebugLog("Turn on `Use Cloud Storage` in `ApiSettings` first");
#endif
        }

        /// <summary>
        /// Download a file from cloud url then save to local url
        /// </summary>
        /// <param name="cloudChildPath">Example: "images/demo.jpg"</param>
        /// <param name="destinationFilePath">Example: "file:///local/images/demo.jpg"</param>
        public void Download(string cloudChildPath, string destinationFilePath) {
#if FIREBASE && FCS
            if (available) {
                var d = GetChild(cloudChildPath);
                if (d != null) {
                    d.GetFileAsync(destinationFilePath).ContinueWithOnMainThread(task => {
                        if (task.IsFaulted || task.IsCanceled) {
                            DebugLog($"Download failed error = {task.Exception}", true);
                        }
                        else {
                            DebugLog($"File downloaded into local url={destinationFilePath}");
                        }
                    });
                }
            }
            else CallOnAvailable(() => { Download(cloudChildPath, destinationFilePath); });
#else
            DebugLog("Turn on `Use Cloud Storage` in `ApiSettings` first");
#endif
        }
#endregion

    }
}