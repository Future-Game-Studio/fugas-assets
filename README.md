# FUGAS AdMob Ads Assets for Unity

------

### Usage:

1. Download and import this repo or **unitypackage**.

2. Download the [Google Ads plugin 5.4.0](https://github.com/googleads/googleads-mobile-unity/releases/tag/v5.4.0).

3. Go to `Assets/Google Mobile Ads/Settings` and set your `App Unit ID`. This ID can be found in [Google AdMob Console](https://admob.google.com/home/). For debugging there is a sample app test id.

4. Also, view `Assets/FUGAS/Ads` and set your `Unit Ad Id`.

5. (Optionally from V2.0) Drag&Drop **AdsSource** prefab on your scene.
   **NOTE**: AdsSource is a singleton and it's better to keep all ad views in entire app lifetime because AdMob library loads ads **ASYNC**hronously!   

6. Chose and Drag&Drop preferable Ad types in prefabs and configure them on scene (eg. InterstitialAd for skippable full screen ad).

   View `Assets/MainScript.cs` for reference.

   To **configure** requests and other values, see `Assets/FUGAS/Ads/AdMobInitializer.cs`

7. Profit!

#### Example how to configure and listen to ad events:

```csharp
// ASYNC version. Awake strictly recommended
AdMobInitializer.Configure(x =>
{
    // configuring view events. 
    // NOTE: they are NOT synchronized with unity threads!
    x.ConfigureRewardedAd(request => { /* custom request builder */ }, 
        view =>
        {	
            /* view builder */
            // default event usage
            view.OnAdClosed += (s, e) => { print(s.GetType().Name + " closed (configured event)"); };
            view.OnAdFailedToShow += (s, e) => { print(s.GetType().Name + " failed (configured event)"); };
            
            // How to synchronize context
            view.OnAdOpening += (s, e) => 
            {
               SyncContext.RunOnUnityThread(() => {
                  someUiController.text = (s.GetType().Name + " oppening (configured event)"); 
               });
            };
        });
});

// SYNC version can be placed on Start or Awake
// these events are already synchronized by SyncContext!
_interstitialAd = FindObjectOfType<InterstitialAdScript>();
_interstitialAd.OnAdLoaded += (s, e) => print("InterstitialAd loaded! (OnStart event)");

```

#### Example how to handle Rewarded Ad callback:

Other ad types have similar API.

```csharp
public void OnGetRewardClick()
{
    var ad = FindObjectOfType<RewardedAdScript>(); // this can be a field
    if (ad != default)
    {
        var view = ad.GetView(); // get current ad view (cached one)
        view.OnUserEarnedReward += (s, e) =>
        {
            var reward = view.GetRewardItem();
            if (reward != default) 
            {
            	var rewardType = reward.Type;
            	var rewardValue = reward.Amount;
            }
        };
        ad.ShowAd(true); // forcing to refresh cache
    }
}
```

## FAQ

#### **Q:  There some missing types**

A: Try to import [Google Ads plugin 5.4.0](https://github.com/googleads/googleads-mobile-unity/releases/tag/v5.4.0).

#### **Q: Views, what are they?** 

A: You can read more here - [link](https://developers.google.com/admob/android/quick-start#select_an_ad_format).

#### **Q: I want to change a banner position **

A: Use `AdMobInitializer.Configure` and configure `BannerAd` view with `SetPosition` method.

#### **Q: App is crashing. Can't find and load ad dependencies.**

A: Try to clean resolved libraries (Assets/External Dependency Manager/Android Resolver/Delete Resolved Libraries) and reimport all assets (Assets/Reimport All)

#### **Q: Ads are not shown in editor**

A: They won't be shown, there is a stub in AdMob. Open console and check for DummyClient output.

#### **Q: Ads are not shown in development or production**

A: Try to subscribe for `OnAdFailedToLoad` and `OnAdLoaded` events. Log output to console. Remember Ads are loading asynchronously!

#### **Q: OnAdFailedToLoad event responds with error code 3 (**ERROR_CODE_NO_FILL**).**

A: The only thing we can do here is wait. You can read more here - [link](https://stackoverflow.com/questions/33566485/failed-to-load-ad-3).

#### **Q: Why should I use SyncContext? Why Canvas UI is not updated in event?**

A: AdMob's Java code is called in RPC way. It's called on separated synchronization context, not on unity thread (update thread).

#### **Q: Can I use events in Start or Update?**

Yes. Just subscribe on `IAdProvider` event, not on admob native one. They are synchronized already.

**NOTE**: Subscribing in Update is a **bad idea**. All configuration and subscription should be done in **Awake, Start or OnEnable**.

#### **Q: How to test ads on debug?**

A: Set test ad checkbox to **true**  in `FUGAS/Scripts/AdvertisementSettings.asset` and build a `development` build.

#### **Q: What types of views are available?**

A: This plugin supports **Banner, Interstitial and Rewarded** views. If you want to use some other views like **Native**, you should implement it on your own.

#### **Q: Some magic happening.**

A: Only God knows how that stuff is working. Just kidding) If you have any issues feel free to ask here.