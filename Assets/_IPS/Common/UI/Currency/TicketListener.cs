
namespace IPS {
    public class TicketListener : CurrencyListener<UserData.OnTicketChanged> {
        public override CurrencyType CurrencyType => CurrencyType.Ticket;
        protected override ulong Value => UserData.CurrentTicket;
        protected override bool CanShowCounting(UserData.OnTicketChanged param) { return param.showCounting; }
    }

    #region UserData
    public partial class  UserData {
        private const string CurrentTicketKey = "CurrentTicket";
        public static ulong CurrentTicket => GetULong(CurrentTicketKey, 0);

        public static void SetTicket(ulong value, bool showCountingEff = true) {
            SetULong(CurrentTicketKey, value);
            EventDispatcher.Instance.Dispatch(new OnTicketChanged() { showCounting = showCountingEff });
        }
        public struct OnTicketChanged : IEventParam {
            public bool showCounting;
        }
    }
    #endregion
}