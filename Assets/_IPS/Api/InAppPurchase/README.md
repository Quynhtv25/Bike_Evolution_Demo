# IPS `InAppPurchase` PACKAGE
> Last Update: 19/04/2024

## 1. Installation
[*(Documentation)*](https://docs.unity3d.com/Packages/com.unity.purchasing@4.5/manual/Overview.html)

- Open unity menu `Window/Package Manage`, search & install **In App Purchasing** (in Unity Registry)
- Open unity menu `Service/In-app Purchasing/Configure...`, link the project, then turn on **In-app Purchases** toggle.
- Enter google **`base64`** license key into field under region **Google Play Configruration / Revenue Validation**, then click 
    - **`Verify`** button under *Revenue Validation*.
    - **`Obsfucate License Keys`** under *Receipt Obsfucator* 
    - **`Obsfucate Apple License Keys`** under *Apple Configuration*


- Open unity menu `IPS/Api/ApiSettings`, turn ON the option `Use In App Purchase`, then `Save` 
 

## 2. Define Products

There are 2 ways to define all products but prefer this:
- Open unity menu `Service/In-app Purchasing/IAP Catalogue..`, setup all products with field:
    - **Require field**: ID, Type, Price (Advanced/Google Configuration/Price & Advanced/Apple Configuration/Price Tier)
    - Optional field: *Advanced/Description*, 
    - When setup all finished, click `App Store Export` to export xml/csv file for submit to store.
    
Or other way: 
- Open script `IAP.cs`, override method `AddProduct(builder)` then code into it, for example:

        builder.Add(productId, ProductType.NonConsumable);
        

## 2. CODE IMPLEMENTATION

Buy a product:

           IAP.Instance.Buy(productId, onBuySuccess, onBuyFailed);
           
           
Check whether a product has bought or not (for non-consumable only):


           IAP.Instance.IsOwned(productId);
           
Restore all bought non-consumable products (use in case re-install game or cleared data):

           IAP.Instance.RestorePurchase(OnSuccess); 
           // You need to continue use `IsOwned` to restore products which user had bought.
  


## 3. Issue

if build error by duplicate billing, open file `Assets/Plugin/Android/mainTemplate.gradle`, patse this code into the last of block `dependencies`:


     configurations.all {
	exclude group: 'com.android.billingclient', module: 'billing'
     }
  