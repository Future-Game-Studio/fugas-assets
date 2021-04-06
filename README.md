# FUGAS AdMob Ads Assets for Unity

------

### Usage:

1. Download and import this repo or **unitypackage**.

2. Download the [Google Ads plugin 5.4.0](https://github.com/googleads/googleads-mobile-unity/releases/tag/v5.4.0).

3. Go to `Assets/Google Mobile Ads/Settings` and set your `App Unit ID`. This ID can be found in [Google AdMob Console](https://admob.google.com/home/).

4. Also, view `Assets/FUGAS/Ads` and set your `Unit Ad Id`.

5. Drag&Drop **AdsSource** prefab on your scene (Singleton GameObject is **recommended**).

6. Chose and Drag&Drop preferable Ad types in prefabs and configure them on scene (eg. InterstitialAd for skippable full screen ad).

   View `Assets/MainScript.cs` for reference.

   To **configure** requests and other values, see `Assets/FUGAS/Ads/AdMobInitializer.cs`

7. Profit!

### Example how to handle Rewarded Ad callback:

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
            if (reward != default) {
            	var rewardType = reward.Type;
            	var rewardValue = reward.Amount;
            }
        };
        ad.ShowAd(true); // forcing to refresh cache
    }
}
```

