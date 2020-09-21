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
        private AdRequest _request;

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
            CreateAdRequest();
            LoadAd();
        }

        public InterstitialAd GetView() => _view;

        public void CreateAdRequest()
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
            _request = builder.Build();
            LoadAd();
        }

        public void LoadAd()
        {
            if (_request == default)
                CreateAdRequest();
            // Load the interstitial with the request.
            _view.LoadAd(_request);
        }

        public void OnDestroy()
        {
            _view?.Destroy();
        }

        public void ShowAd() => ShowAd(false);
        public void ShowAd(bool requestNew)
        {
            var retractor = 5;
            while (retractor > 0 && !_view.IsLoaded())
            {
                retractor--;
                LoadAd();
            }

            _viewUsed = true;
            _view.Show();
            if (requestNew)
            {
                CreateAdRequest();
                LoadAd();
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
            instance.OnAdLoaded += (sender, args) => OnAdLoaded?.Invoke(this, args);

            instance.OnAdFailedToLoad +=
                (sender, args) => OnAdFailedToLoad?.Invoke(this, new AdErrorEventArgs { Message = args.Message });

            instance.OnAdOpening += (sender, args) => OnAdOpening?.Invoke(this, args);

            instance.OnAdClosed += (sender, args) => OnAdClosed?.Invoke(this, args);

            instance.OnAdLeavingApplication += (sender, args) => OnAdLeavingApplication?.Invoke(this, args);
        }

        #endregion
    }
}