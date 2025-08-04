
namespace IPS {
    public class GemListener : CurrencyListener<UserData.OnGemChanged> {
        public override CurrencyType CurrencyType => CurrencyType.Gem;
        protected override ulong Value => UserData.CurrentGem;
        protected override bool CanShowCounting(UserData.OnGemChanged param) { return param.showCounting; }
    }

    #region UserData
    public partial class  UserData {
        private const string CurrentGemKey = "CurrentGem";
        public static ulong CurrentGem => GetULong(CurrentGemKey, 0);

        public static void SetGem(ulong value, bool showCountingEff = true) {
            SetULong(CurrentGemKey, value);
            EventDispatcher.Instance.Dispatch(new OnGemChanged() { showCounting = showCountingEff });
        }
        public struct OnGemChanged : IEventParam {
            public bool showCounting;
        }
    }
    #endregion
}