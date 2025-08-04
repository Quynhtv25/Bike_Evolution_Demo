using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    public enum CurrencyType {
        Cash = 0,
        Gem = 1, 
        Ticket = 2,
        Coin = 3,
        // Add new type below here
    }

    public abstract class CurrencyListener<T> : MonoBehaviour where T: IEventParam {
        [SerializeField] private TextMeshProUGUI currencyText;
        [SerializeField] private float animDuration = 0.3f;

        private ulong currencyCache;

        public abstract CurrencyType CurrencyType { get; }
        protected abstract ulong Value { get; }
        protected T eventKey;

        private void OnEnable() {
            this.AddListener<T>(OnCurrencyChanged);
            currencyCache = Value;

            UpdateVisual(false);
        }

        private void OnCurrencyChanged(T param) {
            UpdateVisual(CanShowCounting(param));
        }

        protected abstract bool CanShowCounting(T param);

        private void UpdateVisual(bool showCounting) {
            if (currencyCache >= Value) {
                currencyCache = Value;
                SetText(currencyText, Value.ToString());
                return;
            }

            StopAllCoroutines();
            StartCoroutine(IECounting(currencyText, currencyCache, Value, showCounting));
        }

        private IEnumerator IECounting(TextMeshProUGUI text, ulong from, ulong to, bool playCountingSfx) {
            if (from == to) yield break;

            if (playCountingSfx) {
                SFX.Instance.PlaySound(SoundEvent.CurrencyCounting);
                float duration = animDuration;
                float elapse = 0;

                while (elapse < duration && currencyCache < to) {
                    elapse += Time.deltaTime;
                    currencyCache = (ulong)(Mathf.Lerp(from, to, elapse));
                    SetText(text, currencyCache.ToString());
                    yield return null;
                }
            }


            currencyCache = to;
            SetText(text, to.ToString());
        }

        protected virtual void SetText(TextMeshProUGUI text, string value) {
            if (value.Length < 5) {
                text.SetText(value.ToDotCurrency());
            }
            else text.SetText(value.ToBigNumber());
        }
    }
}