using System;
using Assets.FUGAS.Ads.Scripts.Abstractions;
using GoogleMobileAds.Api;

namespace Assets.FUGAS.Ads.Scripts
{
    public partial class AdMobInitializer
    {
        class AdRequestConfiguration : IAdRequestConfiguration
        {
            private readonly AdMobInitializer _target;

            public AdRequestConfiguration(AdMobInitializer target)
            {
                _target = target;
            }

            private IAdRequestConfiguration CoreAd<T>(Action<T> viewConfigurator, Action<AdRequest.Builder> requestConfigurator = default)
            {
                if (requestConfigurator != default)
                    _target._requestConfigurationMap.Add(typeof(T), requestConfigurator);
                if (viewConfigurator == default)
                    throw new ArgumentNullException();
                _target._viewConfiguratorMap.Add(typeof(T), x => viewConfigurator((T)x));
                return this;
            }

            public IAdRequestConfiguration ConfigureRequest(Action<AdRequest.Builder> requestConfigurator)
            {
                if (requestConfigurator == default) throw new ArgumentNullException();
                _target._defaultRequestConfiguration = requestConfigurator;
                return this;
            }

            public IAdRequestConfiguration ConfigureBannerAd(
                Action<AdRequest.Builder> requestConfigurator, Action<BannerView> viewConfigurator)
            {
                return CoreAd<BannerView>(viewConfigurator, requestConfigurator);
            }

            public IAdRequestConfiguration ConfigureRewardedAd(
                Action<AdRequest.Builder> requestConfigurator, Action<RewardedAd> viewConfigurator)
            {
                return CoreAd<RewardedAd>(viewConfigurator, requestConfigurator);
            }

            public IAdRequestConfiguration ConfigureInterstitialAd(
                Action<AdRequest.Builder> requestConfigurator, Action<InterstitialAd> viewConfigurator)
            {
                return CoreAd<InterstitialAd>(viewConfigurator, requestConfigurator);
            }

            public IAdRequestConfiguration ConfigureRewardBasedVideoAd(
                Action<AdRequest.Builder> requestConfigurator, Action<RewardBasedVideoAd> viewConfigurator)
            {
                return CoreAd<RewardBasedVideoAd>(viewConfigurator, requestConfigurator);
            }
        }
    }
}