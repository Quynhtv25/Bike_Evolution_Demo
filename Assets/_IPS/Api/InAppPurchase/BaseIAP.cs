using System;
using UnityEngine;
using System.Transactions;


#if IAP
using Unity.Services.Core;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Extension;
#endif

namespace IPS.Api.IAP {
#if IAP
#if UNITY_2022_3_OR_NEWER
    public abstract class BaseIap<T> : SingletonBehaviourDontDestroy<T>, IDetailedStoreListener where T : MonoBehaviour {
#else
    public abstract class BaseIap<T> : SingletonBehaviourDontDestroy<T>, IStoreListener where T : MonoBehaviour {
#endif
#else
    public abstract class BaseIap<T> : SingletonBehaviourDontDestroy<T> where T : MonoBehaviour {
#endif
        public static bool IsPurchasing { get; private set; }
        public bool isRequesting;
        public Action onBuyFailed;
        public Action onBuyCompleted;
        public event Action<string> OnPurchasingComplete;
        private const char DefaultSymbol = '$';
        private const string DefaultIsoCurrencyCode = "USD";

        public class Meta {
            public readonly string isoCurrencyCode;
            public readonly string localizedPriceString;
            public readonly Decimal localizedPrice;
            public readonly char symbol;

            public string LocalizedPriceSymbol { get; private set; }

            public Meta(decimal localizedPrice, char symbol, string isoCurrencyCode) {
                this.isoCurrencyCode = isoCurrencyCode;
                this.localizedPrice = localizedPrice;
                if (!string.IsNullOrEmpty(isoCurrencyCode)) {
                    localizedPriceString = string.Format("{0:#,0.##}{1}", this.localizedPrice, isoCurrencyCode);
                }

                this.symbol = symbol;

                LocalizedPriceSymbol = string.Format("{0:#,0.##}{1}", localizedPrice, symbol);
            }

            public Meta(decimal localizedPrice, string localizedPriceString, string isoCurrencyCode) {
                this.isoCurrencyCode = isoCurrencyCode;
                this.localizedPriceString = localizedPriceString;
                this.localizedPrice = localizedPrice;

                if (string.IsNullOrEmpty(this.localizedPriceString)) {
                    symbol = DefaultSymbol;
                    LocalizedPriceSymbol = string.Format("{1}{0:#,0.##}", localizedPrice, symbol);
                }
                else {
                    if (!char.IsDigit(this.localizedPriceString[0])) {
                        symbol = this.localizedPriceString[0];
                        LocalizedPriceSymbol = string.Format("{1}{0:#,0.##}", localizedPrice, symbol);
                    }
                    else if (!char.IsDigit(this.localizedPriceString[this.localizedPriceString.Length - 1])) {
                        symbol = this.localizedPriceString[this.localizedPriceString.Length - 1];
                        LocalizedPriceSymbol = string.Format("{0:#,0.##}{1}", localizedPrice, symbol);
                    }
                    else {
                        symbol = DefaultSymbol;
                        LocalizedPriceSymbol = string.Format("{1}{0:#,0.##}", localizedPrice, symbol);
                    }
                }

            }
        }

        private static readonly Meta emptyMeta = new Meta(0, DefaultSymbol, DefaultIsoCurrencyCode);
#if IAP
        private IStoreController storeController;
        private IExtensionProvider storeExtensionProvider;
#endif

        public void RegisterOnPurchasingComplete(Action<string> onPurchasingComplete) {
            OnPurchasingComplete += onPurchasingComplete;
        }

        public void UnRegisterOnPurchasingComplete(Action<string> onPurchasingComplete) {
            OnPurchasingComplete -= onPurchasingComplete;
        }

        public virtual void OnPurchaseStart() { }

        public virtual void OnPurchaseEnd() { }

        protected override void OnAwake() {
#if IAP
            DoInitialize();
#endif
        }

#if IAP
        private async void DoInitialize() {
            StandardPurchasingModule module = StandardPurchasingModule.Instance();
#if UNITY_EDITOR
            module.useFakeStoreUIMode = FakeStoreUIMode.StandardUser;
#endif
            var builder = ConfigurationBuilder.Instance(module);
            var productCatalog = ProductCatalog.LoadDefaultCatalog();
            if (productCatalog != null && productCatalog.allProducts != null && productCatalog.allProducts.Count > 0) {
                foreach (var product in productCatalog.allProducts) {
#if UNITY_IOS
                    product.id = $"{Application.identifier}.{product.id}";
#endif
                    builder.AddProduct(product.id, product.type);
                }
            }

            AddProduct(builder);
            
            if (builder.products.Count == 0) {
                Debug.LogError("[IAP] You need to setup IAP ProductCatalog first (Goto 'Services/In-App Purchasing/IAP Catalog')");
            }

            Debug.Log($"[IAP] Start Initialize total {builder.products.Count} products");

            try {
                await UnityServices.InitializeAsync(new InitializationOptions().SetOption("environment", !IPSConfig.CheatEnable ? "production" : "sandbox"));
                UnityPurchasing.Initialize(this, builder);
            }
            catch (Exception ex) {
                Debug.LogError("[IAP] " + ex.Message);
                UnityPurchasing.Initialize(this, builder);
            }
        }

        protected virtual void AddProduct(ConfigurationBuilder builder) { }


        private bool IsInitialized() {
            var isInitialized = storeController != null && storeExtensionProvider != null;
            if (!isInitialized) {
                Debug.LogErrorFormat("[IAP] Not initialized.");
            }

            return isInitialized;
        }


        public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
            Debug.Log("[IAP]  OnInitialized");
            storeController = controller;
            storeExtensionProvider = extensions;
            OnInitSuccessCallback();
        }

        protected abstract void OnInitSuccessCallback();

        public void OnInitializeFailed(InitializationFailureReason error) {
            Debug.LogErrorFormat("[IAP] OnInitializeFailed error: {0}", error);
            isRequesting = false;
        }

#if UNITY_2022_3_OR_NEWER
        public void OnInitializeFailed(InitializationFailureReason error, string msg) {
            Debug.LogErrorFormat("[IAP] OnInitializeFailed error: {0}", error);
            isRequesting = false;
        }
#endif

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
            var validPurchase = true;
            string errorMsg = "ProcessPurchase fail: ";

#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX

            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);
            string id = string.Empty;
            string transactionId = string.Empty;

            try {
                if (args !=  null && args.purchasedProduct != null) {
                    id = args.purchasedProduct.definition.id;
                    transactionId = args.purchasedProduct.transactionID;
                }
                var result = validator.Validate(args.purchasedProduct.receipt);
                foreach (IPurchaseReceipt productReceipt in result) {
                    if (string.IsNullOrEmpty(id)) id = productReceipt.productID;
                    if (string.IsNullOrEmpty(transactionId)) transactionId = productReceipt.transactionID;
                    Debug.LogFormat("productID:={0}, purchaseDate={1}, transactionID={2}", productReceipt.productID, productReceipt.purchaseDate, productReceipt.transactionID);
                }
            }
            catch (Exception e) {
#if !UNITY_EDITOR
                Debug.LogError("Invalid receipt, not unlocking content");
                errorMsg += e != null ? e.Message : "Invalid receipt";
                validPurchase = false;
#endif
            }
#else
            Debug.LogError("Please ask store admin for google public key of iap purchase");
#endif


            if (validPurchase) {
                try {
                    var meta = args.purchasedProduct.metadata;
                    if (OnPurchasingComplete != null) {
                        OnPurchasingComplete.Invoke(id);
                    }

                    if (onBuyCompleted != null) {
                        onBuyCompleted.Invoke();
                    }

                    Debug.Log($"[IAP] Purchase sucessfully");
                    Tracking.Instance.LogPurchaseSuccess(id, meta.isoCurrencyCode, (double)meta.localizedPrice, args.purchasedProduct.transactionID);
                }
                catch (Exception e) {
                    validPurchase = false;
                    errorMsg += "receipt valid but failure: ";
                    if (e != null) errorMsg += e.Message;
                }
            }

            if (!validPurchase) {
                Debug.LogError($"[IAP] ProcessPurchase failed: id={id}, transactionId={transactionId}, errormsg= {errorMsg}");
                Tracking.Instance.LogPurchaseFailure(id, transactionId, errorMsg);
            }

            isRequesting = false;
            IsPurchasing = false;
            OnPurchaseEnd();
            return PurchaseProcessingResult.Complete;
        }


        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
            Debug.LogError($"[IAP] PurchaseFailed reason={failureReason}");
            if (!isRequesting) return;
            isRequesting = false;

            if (onBuyFailed != null) {
                onBuyFailed.Invoke();
                onBuyFailed = null;
            }
            IsPurchasing = false;
            OnPurchaseEnd();

            Tracking.Instance.LogPurchaseFailure(product != null && product.definition != null ? product.definition.id : "Unknow id",
                                                product != null ? product.transactionID : "Unknow transactionId", failureReason.ToString());
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
            if (!isRequesting) return;
            isRequesting = false;

            if (onBuyFailed != null) {
                onBuyFailed.Invoke();
                onBuyFailed = null;
            }
            IsPurchasing = false;
            OnPurchaseEnd();
            string producId = product != null && product.definition != null ? product.definition.id : failureDescription != null ? failureDescription.productId : "Unknow Id";
            string transactionId = producId != null && product.definition != null && !string.IsNullOrEmpty(product.transactionID) ? product.transactionID : "Unknow transactionId";
            string reason = failureDescription != null ? failureDescription.reason.ToString() : "Unknow reason";
            string msg = failureDescription != null ? failureDescription.message : "Unknow errormsg";

            Debug.LogError($"[IAP] PurchaseFailed productId={producId}, transactionId={transactionId}, reason={reason}, message={msg}");
            Tracking.Instance.LogPurchaseFailure(producId, transactionId, reason);
        }
#endif
        public Meta GetLocalPrice(string id, Decimal defaultPrice = 0, string defaultSymbol = "$", string defaultCurencyCode = DefaultIsoCurrencyCode) {
#if IAP
            if (storeController != null) {
                var product = GetProduct(id);
                if (product != null) {
#if UNITY_EDITOR
                    return new Meta(defaultPrice, defaultSymbol, defaultCurencyCode);
#endif
                    var productMetadata = product.metadata;
                    return new Meta(productMetadata.localizedPrice, productMetadata.localizedPriceString, productMetadata.isoCurrencyCode);
                }
                else {
                    Logs.LogError($"[IAP] Product id={id} not found! You need to setup product into IAP Catalog or add product via IAP.AddProduct method.");
                    return emptyMeta;
                }
            }
            else if (defaultPrice > 0) {
                return new Meta(defaultPrice, defaultSymbol, defaultCurencyCode);
            }

            return emptyMeta;
#else
#if UNITY_EDITOR
            Debug.Log("IAP is disable. Please add flag 'IAP' at PlayerSetting.");
#endif
            if (defaultPrice > 0) {
                return new Meta(defaultPrice, defaultSymbol, defaultCurencyCode);
            }

            return emptyMeta;
#endif
        }

        public void RestorePurchases(System.Action success = null) {
#if IAP
            if (IsInitialized()) {
                if (Application.platform == RuntimePlatform.Android) {
                    if (success != null) success.Invoke();
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer ||
                    Application.platform == RuntimePlatform.OSXPlayer) {
                    Debug.Log("RestorePurchases started ...");
#if ADS
                    AdsManager.PauseInsideApp = true;
#endif

                    var apple = storeExtensionProvider.GetExtension<IAppleExtensions>();

#if UNITY_2022_3_OR_NEWER
                    apple.RestoreTransactions((result, msg) => {
                        if (result && success != null) {
                            success.Invoke();
                        }
                        Debug.LogFormat(
                            "RestorePurchases continuing: {0}. If no further messages, no purchases available to restore.",
                            result);
                    });
#else
                    apple.RestoreTransactions((result) => {
                        if (result && success != null) {
                            success.Invoke();
                        }
                        Debug.LogFormat(
                            "RestorePurchases continuing: {0}. If no further messages, no purchases available to restore.",
                            result);
                    });
#endif
                }
                else {
                    Debug.LogFormat("RestorePurchases FAIL. Not supported on this platform. Current = {0}", Application.platform);
                }

                isRequesting = false;
            }
#elif UNITY_EDITOR
                    Debug.Log("IAP is disable. Please add flag 'IAP' at PlayerSetting.");
#endif
        }

        public virtual void Buy(string productId, Action onBuyCompleted = null, Action onBuyFailed = null, string placement = default) {
            Tracking.Instance.LogPurchaseEligible(productId, placement);
#if ADS
            AdsManager.PauseInsideApp = true;
#endif
#if IAP
#if UNITY_EDITOR
            if (onBuyCompleted != null) {
                onBuyCompleted.Invoke();
            }
            else onBuyFailed?.Invoke();
            return;
#endif
            if (isRequesting)
                return;
            this.onBuyCompleted = onBuyCompleted;
            this.onBuyFailed = onBuyFailed;

            Product product = GetProduct(productId);
            if (product != null) {
                if (product.availableToPurchase) {
                    Debug.LogFormat("Purchasing product asychronously: '{0}'", product.definition.id);
                    isRequesting = true;
                    IsPurchasing = true;
                    OnPurchaseStart();
                    storeController.InitiatePurchase(product);
                    Tracking.Instance.LogPurchaseStart(productId, placement);
                }
                else {
                    Debug.Log(
                        "BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            else {
                Logs.LogError($"[IAP] Product does not exist id={productId}");
                this.onBuyFailed?.Invoke();
                this.onBuyFailed = null;
                this.onBuyCompleted = null;
            }
#else
#if UNITY_EDITOR
            if (onBuyCompleted != null) {
                    onBuyCompleted.Invoke();
            }
#endif
            Debug.Log("IAP is disable. Please add flag 'IAP' at PlayerSetting.");
#endif
        }

        public bool IsOwned(string productID) {
#if IAP
            var product = GetProduct(productID);
#if UNITY_ANDROID
            if (product != null && product.hasReceipt) {
                var state = JsonUtility.FromJson<GooglePlayReceipt>(product.receipt);
                if (state != null && state.purchaseState != GooglePurchaseState.Purchased) return false;
                return true;
            }
#endif
            return product != null ? product.hasReceipt : false;
#else
            Debug.Log("IAP is disable. Please add flag 'IAP' at PlayerSetting.");
            return false;
#endif
        }

#if IAP
        private Product GetProduct(string productID) {
            if (!IsInitialized()) return null;
            if (string.IsNullOrEmpty(productID)) {
                Debug.LogError("[IAP] Empty productID");
                return null;
            }
            return storeController.products.WithID(productID);
        }

        /**<summary> Get the information of subscription product. 
        * <para>You shoud use these value:</para>
        * <para>isSubcribed (== Result.True): to check if currently subcribed or not </para>
        * <para>isExpired (!= Result.False): to check if has expired </para>
        * <para>getRemainingTime: to show countdown timer </para>
        * </summary>*/
        public SubscriptionInfo GetSubscriptionInfo(string id, string intro_json = "") {
            Product p = GetProduct(id);
            if (p == null) {
                Logs.LogError($"[IAP] Product id={id} not found! You need to setup product into IAP Catalog or add product via IAP.AddProduct method.");
                return null;
            }

            if (p.definition.type != ProductType.Subscription || !p.hasReceipt) return null;

            if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor) {
                Debug.LogError($"[IAP] Subscription does not suport on this platform.");
                return null;
            }

            SubscriptionManager sub = new SubscriptionManager(p, intro_json);
            SubscriptionInfo info = sub.getSubscriptionInfo();
            if (info == null) {
                Debug.LogError($"[IAP] Product had been purchase but cannot get SubscriptionInfo: {p.definition.id}");
                return null;
            }

            if (IPSConfig.LogEnable) {
                String s = $"[IAP] [Subscription] id = {info.getProductId()}";
                s += $"\n[Subscription] isSubscribed = {info.isSubscribed().ToString()}. (Currently subscribed or not)";
                s += $"\n[Subscription] isExpired = {info.isExpired().ToString()}";
                s += $"\n[Subscription] isAutoRenewing = {info.isAutoRenewing()}";
                s += $"\n[Subscription] isCancelled = {info.isCancelled()}. (A cancelled subscription means the Product is currently subscribed, but will not renew on the next billing date)";
                s += $"\n[Subscription] isFreeTrial = {info.isFreeTrial()}";
                s += $"\n[Subscription] PurchaseDate = {info.getPurchaseDate()} (For Apple, the purchase date is the date when the subscription was either purchased or renewed. For Google, the purchase date is the date when the subscription was originally purchased)";
                s += $"\n[Subscription] ExpireDate = {info.getExpireDate()} (the date of the Product�s next auto-renew or expiration)";
                s += $"\n[Subscription] RemainingTime = {info.getRemainingTime()} (How much time remains until the next billing date)";
                s += $"\n[Subscription] isIntroductoryPricePeriod = {info.isIntroductoryPricePeriod()} (is within an introductory price period)";
                s += $"\n[Subscription] IntroductoryPrice = {info.getIntroductoryPrice()} (the introductory price of the Product)";
                s += $"\n[Subscription] IntroductoryPricePeriod = {info.getIntroductoryPricePeriod()} (How much time remains for the introductory price period)";
                s += $"\n[Subscription] IntroductoryPricePeriodCycles = {info.getIntroductoryPricePeriodCycles()} (the number of introductory price periods that can be applied to this Product. Products in the Apple store return 0 if the application does not support iOS version 11.2+, macOS 10.13.2+, or tvOS 11.2+)";
                Logs.Log(s);
            }
            return info;
        }

        /// <summary>
        /// Use for check subscription status more easily.
        /// <para>status = "NotSubcribe" mean user NEVER subcribe, he CAN NOT use any content of this product.</para>
        /// <para>status = "Subcribing" mean user currently subcribing and has not expired, he CAN use all content of this product.</para>
        /// <para>status = "Expired" mean user currently subcribing but has remaining time = 0. he NEED TO REPURCHASE this product to continue get it's content.</para>
        /// </summary>
        public enum SubscriptionStatus { NotSubscribe, Subcribing, Expired }

        /// <summary>
        /// Use for check subscription status more easily.
        /// <para>status = "NotSubcribe" mean user NEVER subcribe, he CAN NOT use any content of this product.</para>
        /// <para>status = "Subcribing" mean user currently subcribing and has not expired, he CAN use all content of this product.</para>
        /// <para>status = "Expired" mean user currently subcribing but has remaining time = 0. he NEED TO REPURCHASE this product to continue get it's content.</para>
        /// </summary>
        public SubscriptionStatus GetSubscriptionStatus(string id) {
            var subInfor = GetSubscriptionInfo(id);
            if (subInfor == null || subInfor.isSubscribed() != Result.True) return SubscriptionStatus.NotSubscribe;
            if (subInfor.isExpired() != Result.True) return SubscriptionStatus.Subcribing;
            return SubscriptionStatus.Expired;
        }
#endif
    }
}