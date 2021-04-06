using System;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts
{
    public class AdMobInitializer : MonoBehaviour
    {
        [HideInInspector]
        public bool Ready;
        private Dictionary<Type, Action<AdRequest.Builder>> _requestConfigurationMap;
        private Dictionary<Type, Action<object>> _viewConfiguratorMap;
        public static AdMobInitializer Instance { get; private set; }

        #region Configure

        private void Initialize()
        {
            Action<AdRequest.Builder> defaultRequestConfiguration = _ => { };

            if (Debug.isDebugBuild)
            {
                MobileAds.SetiOSAppPauseOnBackground(true);

                var deviceIds = new List<string> { AdRequest.TestDeviceSimulator };

                // Add some test device IDs (replace with your own device IDs).
#if UNITY_IPHONE
                deviceIds.Add("96e23e80653bb28980d3f40beb58915c");
#elif UNITY_ANDROID
                deviceIds.Add("75EF8D155528C04DACBBA6F36F433035");
#endif

                // Configure TagForChildDirectedTreatment and test device IDs.
                var requestConfiguration =
                    new RequestConfiguration.Builder()
                        .SetTagForChildDirectedTreatment(TagForChildDirectedTreatment.Unspecified)
                        .SetTestDeviceIds(deviceIds).build();

                MobileAds.SetRequestConfiguration(requestConfiguration);

                // here we can override above settings for each client
                defaultRequestConfiguration = builder =>
                {
                    builder.AddTestDevice(AdRequest.TestDeviceSimulator)
                       .AddKeyword("unity-admob-sample")
                       .TagForChildDirectedTreatment(false)
                       .AddExtra("color_bg", "9B30FF");
                };
            }
            _requestConfigurationMap = new Dictionary<Type, Action<AdRequest.Builder>>
            {
                // or implement your request configuration here
                {typeof(BannerAdScript), defaultRequestConfiguration},
                {typeof(InterstitialAdScript), defaultRequestConfiguration},
                {typeof(RewardedAdScript), defaultRequestConfiguration},
            };

            _viewConfiguratorMap = new Dictionary<Type, Action<object>>
            {
                {typeof(BannerAdScript),  builder =>
                {
                    // implement your view configuration here
                    // builder as BannerView
                }},
                {typeof(InterstitialAdScript), builder =>
                {
                    // implement your view configuration here
                    // builder as InterstitialAd
                }},
                {typeof(RewardedAdScript), builder =>
                {
                    // implement your view configuration here
                    // builder as RewardedAd
                }},
            };

            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize(_ => { });

            Ready = true;
        }

        #endregion

        #region Service

        public static void EnsureReady()
        {
            if (!Instance.Ready)
                Instance.Initialize();
        }

        public void Awake()
        {
            if (Instance is null)
            {
                Instance = this;
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public void Start()
        {
            EnsureReady();
        }

        public Action<AdRequest.Builder> ConfigureAdRequestFor<T>() where T : IAdProvider
        {
            var t = typeof(T);
            return _requestConfigurationMap.ContainsKey(t) ? _requestConfigurationMap[t] : x => { };
        }

        public Action<TView> ConfigureViewFor<T, TView>() where T : IAdProvider
        {
            var t = typeof(T);
            return _viewConfiguratorMap.ContainsKey(t) ?
                (Action<TView>)(b => _viewConfiguratorMap[t](b))
                : x => { };
        }

        #endregion
    }
}