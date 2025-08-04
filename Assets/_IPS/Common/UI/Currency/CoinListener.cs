
namespace IPS {
    public class CoinListener: CurrencyListener<UserData.OnCoinChanged> {
        public override CurrencyType CurrencyType => CurrencyType.Coin;
        protected override ulong Value => UserData.CurrentCoin;
        protected override bool CanShowCounting(UserData.OnCoinChanged param) {
            return param.showCounting;
        }
    }

    #region UserData
    public partial class UserData {
        private const string CurrentCoinKey = "CurrentCoin";
        public static ulong CurrentCoin => GetULong(CurrentCoinKey, 0);

        public static void SetCoin(ulong value, bool showCountingEff = true, string placement = null) {
            if (!string.IsNullOrEmpty(placement) && value < CurrentCoin) {
                Tracking.Instance.LogUseCurrency(placement);
            }

            SetULong(CurrentCoinKey, value);
            EventDispatcher.Instance.Dispatch(new OnCoinChanged() { showCounting = showCountingEff });
        }
        public struct OnCoinChanged: IEventParam {
            public bool showCounting;
        }
    }
    #endregion
}