using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using IPS;

namespace IPS {
    public class ItemCollectEffect : SingletonBehaviour<ItemCollectEffect> {
        [SerializeField] Transform target;
        [SerializeField] GameObject itemPrefab;

        private Vector3 DefaultTargetPos => target != null ? target.position : itemPrefab != null ? itemPrefab.transform.position : Vector3.zero;

        protected override void OnAwake() {
            if (itemPrefab) itemPrefab.gameObject.SetActive(false);
        }

        public void Collect(GameObject prefab, Vector3 fromPos, Transform targetPos, System.Action onStartCollect, System.Action onFinishCallback = null, int emission = 10, System.Action onStepCallback = null, Vector3 targetScale = default) {
            if (!Initialized) {
                Debug.LogError($"{typeof(ItemCollectEffect).Name} not found in scene");
                if (onStartCollect != null) onStartCollect.Invoke();
                if (onFinishCallback != null) onFinishCallback.Invoke();
                return;
            }
            StartCoroutine(IECollect(prefab ?? itemPrefab, fromPos, targetPos != null ? targetPos.position : DefaultTargetPos, onStartCollect, onFinishCallback, emission, onStepCallback, targetScale));
        }

        public void Collect(Vector3 fromPos, Transform targetPos, System.Action onStartCollect, System.Action onFinishCallback = null, int emission = 10, System.Action onStepCallback = null, Vector3 targetScale = default) {
            if (!Initialized) {
                Debug.LogError($"{typeof(ItemCollectEffect).Name} not found in scene");
                if (onStartCollect != null) onStartCollect.Invoke();
                if (onFinishCallback != null) onFinishCallback.Invoke();
                return;
            }
            StartCoroutine(IECollect(itemPrefab, fromPos,  targetPos != null ? targetPos.position : DefaultTargetPos, onStartCollect, onFinishCallback, emission, onStepCallback, targetScale));
        }

        private IEnumerator IECollect(GameObject prefab, Vector3 fromPos, Vector3 targetPos, System.Action onStartCollect, System.Action onFinishCallback = null, int emission = 5, System.Action onStepCallback = null, Vector3 targetScale = default) {
            if (prefab == null) {
                Debug.LogError("Collect item failed with null item prefab!");
                if (onStartCollect != null) onStartCollect.Invoke();
                if (onFinishCallback != null) onFinishCallback.Invoke();
                yield break;
            }

            emission = Mathf.Clamp(emission, 1, 50);

            int count = 0;
            if (targetScale == default) targetScale = prefab.transform.localScale;

            for (int i = 0; i < emission; ++i) {
                var item = Instantiate(prefab.gameObject, prefab.transform.parent);
                item.gameObject.SetActive(true);
                item.transform.localScale = prefab.transform.localScale;
                item.transform.DOKill();

                var canvas = item.AddComponent<Canvas>();
                canvas.overrideSorting = true;
                canvas.sortingOrder = 10;

                item.transform.position = fromPos;
                Vector2 spawnPos = emission == 1 ? item.transform.localPosition
                    : item.transform.localPosition + (Quaternion.AngleAxis(Random.Range(0, 359), Vector3.forward) * new Vector2(0, Random.Range(200 + emission * 2, 100 + emission)));

                item.transform.DOLocalMove(spawnPos, .7f).SetEase(Ease.OutBack).OnComplete(() => {
                    item.transform.DOScale(targetScale, .5f);
                    item.transform.DOMove(targetPos, .5f).SetEase(Ease.InSine).SetEase(Ease.OutFlash);
                    item.transform.DOScale(0, 0.1f).SetDelay(.5f).OnComplete(() => {
                        count++;
                        if (count == 1 && onStartCollect != null) {
                            onStartCollect.Invoke();
                            onStartCollect = null;
                        }

                        if (onStepCallback != null) onStepCallback.Invoke();

                        if (onFinishCallback != null) {
                            if (count == emission) {
                                onFinishCallback.Invoke();
                                onFinishCallback = null;
                            }
                        }
                        Destroy(item.gameObject);
                    });
                });

                yield return null;
            }
        }
    }
}