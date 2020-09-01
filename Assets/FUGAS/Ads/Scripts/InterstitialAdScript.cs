using System;
using Assets.FUGAS.Ads.Scripts.Helpers;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts
{
    public class InterstitialAdScript : MonoBehaviour, IAdProvider
    {
        public AdvertisementSettings Settings;

        private InterstitialAd _view;
        private string _isTest;
        private bool _viewUsed;
        private Action<AdRequest.Builder> _configuringAdRequest;
        private Action<InterstitialAd> _configuringAdView;

        void Start()
        {
            AdMobInitializer.EnsureReady();
            _configuringAdRequest = AdMobInitializer.Instance.ConfigureAdRequestFor<InterstitialAdScript>();
            _configuringAdView = AdMobInitializer.Instance.ConfigureViewFor<InterstitialAdScript, InterstitialAd>();

            string testUnitId;
#if UNITY_ANDROID
            testUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            testUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
            testUnitId = "unexpected_platform";
#endif
            var settings = Settings;

            _isTest = AdUtils.IsTestMode(settings.TestMode)
                ? testUnitId // Google's Sample TestId
                : settings.InterstitialUnitId;
            RequestAdLoad();
        }

        public InterstitialAd GetView() => _view;

        public void RequestAdLoad()
        {
            if (!_viewUsed && _view != default && _view.IsLoaded())
            {
                // acts like cached ad => less data usage
                return;
            }

            // cleanup before reusing
            _view?.Destroy();
            _viewUsed = false;
            _view = new InterstitialAd(_isTest);

            BindEvents();
            _configuringAdView?.Invoke(_view);

            // Create an empty ad request.
            var builder = new AdRequest.Builder();
            _configuringAdRequest?.Invoke(builder);
            var request = builder.Build();
            // Load the interstitial with the request.
            _view.LoadAd(request);
        }

        public void OnDestroy()
        {
            _view?.Destroy();
        }
         
        public void ShowAd() => ShowAd(false);
        public void ShowAd(bool requestNew)
        {
            if (_view.IsLoaded())
            {
                _viewUsed = true;
                _view.Show();
                if (requestNew)
                    RequestAdLoad();
            }
        }

        #region Clone

        public event EventHandler<EventArgs> OnAdLoaded;

        public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdOpening;
        public event EventHandler<EventArgs> OnAdClosed;
        public event EventHandler<EventArgs> OnAdLeavingApplication;

        private void BindEvents()
        {
            var instance = _view;
            instance.OnAdLoaded += (sender, args) =>
            {
                if (OnAdLoaded != null)
                {
                    OnAdLoaded(args, args);
                }
            };

            instance.OnAdFailedToLoad += (sender, args) =>
            {
                if (OnAdFailedToLoad != null)
                {
                    OnAdFailedToLoad(this, new AdErrorEventArgs { Message = args.Message });
                }
            };

            instance.OnAdOpening += (sender, args) =>
            {
                if (OnAdOpening != null)
                {
                    OnAdOpening(args, args);
                }
            };

            instance.OnAdClosed += (sender, args) =>
            {
                if (OnAdClosed != null)
                {
                    OnAdClosed(args, args);
                }
            };

            instance.OnAdLeavingApplication += (sender, args) =>
            {
                if (OnAdLeavingApplication != null)
                {
                    OnAdLeavingApplication(args, args);
                }
            };
        }

        #endregion
    }
}