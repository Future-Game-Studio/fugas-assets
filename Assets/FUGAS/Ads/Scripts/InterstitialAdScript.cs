using System;
using Assets.FUGAS.Ads.Scripts.Abstractions;
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

        void Awake()
        {
            AdMobInitializer.EnsureReady();

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
        }
        private void Start()
        {
            CreateAdRequest();
            LoadAd();
        }
        public InterstitialAd GetView() => _view;
        public bool IsReady => _view != default && !_viewUsed;

        public void CreateAdRequest()
        {
            if (!_viewUsed && _view != default && _view.IsLoaded())
            {
                // acts like cached ad => less data usage
                return;
            }

            // cleanup before reusing
            _view?.Destroy();

            // reconfigure that stuff
            _configuringAdRequest = AdMobInitializer.Instance.ConfigureAdRequestFor<InterstitialAdScript>();
            _configuringAdView = AdMobInitializer.Instance.ConfigureView<InterstitialAd>();

            _viewUsed = false;
            _view = new InterstitialAd(_isTest);

            BindEvents();
            _configuringAdView?.Invoke(_view);

            // Create an empty ad request.
            var builder = new AdRequest.Builder();
            _configuringAdRequest?.Invoke(builder);
            _request = builder.Build();
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

#pragma warning disable 67
        public event EventHandler<EventArgs> OnAdLoaded;
        public event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;
        public event EventHandler<EventArgs> OnAdOpening;
        public event EventHandler<EventArgs> OnAdClosed;
        public event EventHandler<EventArgs> OnAdLeavingApplication;
#pragma warning restore 67

        private void BindEvents()
        {
            var instance = _view;
            instance.OnAdLoaded += (sender, args) =>
               SyncContext.RunOnUnityThread(() => OnAdLoaded?.Invoke(this, args));

            instance.OnAdFailedToLoad += (sender, args) =>
                SyncContext.RunOnUnityThread(() => OnAdFailedToLoad?.Invoke(this, new AdErrorEventArgs { Message = args.Message }));

            instance.OnAdOpening += (sender, args) =>
                SyncContext.RunOnUnityThread(() => OnAdOpening?.Invoke(this, args));

            instance.OnAdClosed += (sender, args) =>
                SyncContext.RunOnUnityThread(() => OnAdClosed?.Invoke(this, args));

            instance.OnAdLeavingApplication += (sender, args) =>
                SyncContext.RunOnUnityThread(() => OnAdLeavingApplication?.Invoke(this, args));
        }

        #endregion
    }
}