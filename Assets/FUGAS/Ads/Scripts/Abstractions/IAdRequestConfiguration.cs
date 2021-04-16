using System;
using GoogleMobileAds.Api;

namespace Assets.FUGAS.Ads.Scripts.Abstractions
{
    public interface IAdRequestConfiguration
    {
        IAdRequestConfiguration ConfigureRequest(Action<AdRequest.Builder> requestConfigurator);
        IAdRequestConfiguration ConfigureBannerAd(Action<AdRequest.Builder> requestConfigurator, Action<BannerView> viewConfigurator);
        IAdRequestConfiguration ConfigureInterstitialAd(Action<AdRequest.Builder> requestConfigurator, Action<InterstitialAd> viewConfigurator);
        IAdRequestConfiguration ConfigureRewardBasedVideoAd(Action<AdRequest.Builder> requestConfigurator, Action<RewardBasedVideoAd> viewConfigurator);
        IAdRequestConfiguration ConfigureRewardedAd(Action<AdRequest.Builder> requestConfigurator, Action<RewardedAd> viewConfigurator);
    }
}