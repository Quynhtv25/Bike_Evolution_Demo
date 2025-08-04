using IPS;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace IPS {
    public class AddressableLoader : Service<AddressableLoader> {
        protected override void Initialize() {
        }

        public void SpawnAsset<T>(string assetKey, Action<T> callback = null) where T : UnityEngine.Object {
            LoadAsset<T>(assetKey, (data) => {
                if (data) UnityEngine.Object.Instantiate(data);
                if (callback != null) callback.Invoke(data);
            });
        }

        public void SpawnAsset<T>(AssetReference assetReference, Action<T> callback = null) where T : UnityEngine.Object {
            LoadAsset<T>(assetReference, (data) => {
                if (data) UnityEngine.Object.Instantiate(data);
                if (callback != null) callback.Invoke(data);
            });
        }

        public void LoadAsset<T>(string assetKey, Action<T> callback) where T : UnityEngine.Object {
            var async = Addressables.LoadAssetAsync<T>(assetKey);
            async.Completed += (result) => OnAsyncComplete(assetKey, result, callback);
        }

        public void LoadAsset<T>(AssetReference assetReference, Action<T> callback) where T : UnityEngine.Object {
            var handler = assetReference.LoadAssetAsync<T>();
            handler.Completed += (result) => OnAsyncComplete(assetReference.SubObjectName, result, callback);
        }

        private void OnAsyncComplete<T>(string assetKey, AsyncOperationHandle<T> async, Action<T> callback) where T : UnityEngine.Object {
            if (async.Status == AsyncOperationStatus.Succeeded) {
                if (async.Result != null) {
                    Logs.Log($"[{typeof(AddressableLoader).Name}] LoadAsset successfully, name={assetKey}");
                }
                else Logs.LogError($"[{typeof(AddressableLoader).Name}] LoadAsset success but result object is null! assetKey={assetKey}");
                if (callback != null) callback.Invoke(async.Result);
            }
            else {
                Tracking.Instance.LogException(typeof(AddressableLoader).Name, nameof(LoadAsset), $"Fail to load asset key={assetKey}, errormsg={(async.OperationException != null ? async.OperationException.Message : "Unknow")}");
                if (callback != null) callback(null);
            }

            //async.Release();

        }
    }
}

