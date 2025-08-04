
namespace IPS {
    public class CashListener : CurrencyListener<UserData.OnCashChanged> {
        public override CurrencyType CurrencyType => CurrencyType.Cash;
        protected override ulong Value => UserData.CurrentCash;
        protected override bool CanShowCounting(UserData.OnCashChanged param) { return param.showCounting; }
    }

    #region UserData
    public partial class  UserData {
        private const string CurrentCashKey = "CurrentCash";
        public static ulong CurrentCash => GetULong(CurrentCashKey, 0);

        public static void SetCash(ulong value, bool showCountingEff = true) {
            SetULong(CurrentCashKey, value);
            EventDispatcher.Instance.Dispatch(new OnCashChanged() { showCounting = showCountingEff });
        }
        public struct OnCashChanged : IEventParam {
            public bool showCounting;
        }
    }
    #endregion
}