using UnityEngine;
using UnityEngine.UI;

namespace IPS {
    [RequireComponent(typeof(Button))]
    public class DebugAddCurrency : Debuger {
        [SerializeField] CurrencyType type = CurrencyType.Coin;
        [SerializeField] ulong bonus = 100000;

        private void OnValidate() {
            var text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) text.SetText($"+{bonus.ToString().ToBigNumber()}");            
        }

        protected override void OnStart() {
            GetComponent<Button>().onClick.AddListener(AddCurrency);
            var text = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (text != null) text.SetText($"+{bonus.ToString().ToBigNumber()}");
        }

        private void AddCurrency() {
            switch (type) {
                case CurrencyType.Cash: UserData.SetCash(bonus); break;
                case CurrencyType.Gem: UserData.SetGem(bonus); break;
                case CurrencyType.Ticket: UserData.SetTicket(bonus); break;
                case CurrencyType.Coin: UserData.SetCoin(bonus); break;
                default: break;
            }
        }
    }
}