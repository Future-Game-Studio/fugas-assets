using System;
using System.Collections.Generic;
using Assets.FUGAS.Ads.Scripts.Abstractions;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts
{
    public partial class AdMobInitializer : MonoBehaviour
    {
        [HideInInspector]
        public bool Ready { get; private set; }
        private Action<AdRequest.Builder> _defaultRequestConfiguration;
        private Dictionary<Type, Action<AdRequest.Builder>> _requestConfigurationMap
        = new Dictionary<Type, Action<AdRequest.Builder>> { };
        private Dictionary<Type, Action<object>> _viewConfiguratorMap
        = new Dictionary<Type, Action<object>> { };
        private static AdMobInitializer _instance;
        private bool _adMobReady;

        public static AdMobInitializer Instance
        {
            get
            {
                if (!_instance)
                    EnsureReady();
                return _instance;
            }
            private set => _instance = value;
        }

        #region Configure

        private void Initialize()
        {
            _defaultRequestConfiguration ??=
               // here we can override settings for each client
               builder =>
                {
                    builder.AddTestDevice(AdRequest.TestDeviceSimulator)
                       .AddKeyword("unity-admob-sample")
                       .TagForChildDirectedTreatment(false)
                       .AddExtra("color_bg", "9B30FF");
                };

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

            }

            MobileAds.Initialize(_ => { });

            Ready = true;
        }

        #endregion

        #region Service

        public static void EnsureReady()
        {
            Configure(default);
        }

        public static void Configure(Action<IAdRequestConfiguration> builder)
        {
            if (!_instance)
            {
                // Loads AdsSource object from Assets/FUGAS/Resources
                var prefab = Resources.Load<AdMobInitializer>("AdsSource");
                // another way to set instance, alternative
                _instance = Instantiate(prefab);
                _instance.gameObject.AddComponent<SyncContext>();
            }

            if (!_instance)
            {
                Debug.LogError("FUGAS.ADS: Can't find Assets/FUGAS/Resources/AdsSource prefab");
                Debug.LogError("FUGAS.ADS: AdMob Initialization failed. Try to reimport FUGAS Ads package.");
                return;
            }

            if (builder != default)
            {
                // clear previous values for convinience
                _instance.Ready = false;
                _instance._requestConfigurationMap.Clear();
                _instance._viewConfiguratorMap.Clear();

                var configurator = new AdRequestConfiguration(_instance);
                builder(configurator);
                _instance.Initialize();
                return;
            }

            if (!_instance.Ready)
            {
                _instance.Initialize();
            }
        }


        public void Awake()
        {
            if (_instance is null)
            {
                _instance = this;
                _instance.gameObject.AddComponent<SyncContext>();
                EnsureReady();
            }
            else
            {
                DestroyImmediate(gameObject);
            }
        }

        public Action<AdRequest.Builder> ConfigureAdRequestFor<T>() where T : IAdProvider
        {
            var t = typeof(T);
            return _requestConfigurationMap.ContainsKey(t) ? _requestConfigurationMap[t] : x => { };
        }

        public Action<TView> ConfigureView<TView>()
        {
            var t = typeof(TView);
            return _viewConfiguratorMap.ContainsKey(t) ?
                (Action<TView>)(b => _viewConfiguratorMap[t](b))
                : x => { };
        }

        #endregion

    }
}