# FUGAS AdMob Ads Assets for Unity

------

### Usage:

1. Download and import this repo or **unitypackage**.

2. Download the latest [Google Ads plugin](https://github.com/googleads/googleads-mobile-unity/releases).

3. Set `Assets/Google Mobile Ads/Settings` > `App Unit ID`

4. Set values in settings `Assets/FUGAS/Ads` for `Unit Ad Id` those you will use.

5. Drag&Drop **AdsSource** prefab on your scene (Singleton GameObject is **recommended**).

6. Chose and Drag&Drop preferable Ad types in prefabs and configure them on scene (eg. InterstitialAd for skippable full screen ad).

   View `Assets/MainScript.cs` for reference.

   To configure requests and other values, see `Assets/FUGAS/Ads/AdMobInitializer.cs`

7. Profit!



