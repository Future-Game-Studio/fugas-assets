using System;
using Assets.FUGAS.Ads.Scripts.Helpers;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts
{
    public class BannerAdScript : MonoBehaviour, IAdProvider
    {
        public AdvertisementSettings Settings;

        private BannerView _view;
        private string _isTest;
        private Action<AdRequest.Builder> _configuringAdRequest;
        private Action<BannerView> _configuringAdView;

        public void Start()
        {
            AdMobInitializer.EnsureReady();
            _configuringAdRequest = AdMobInitializer.Instance.ConfigureAdRequestFor<BannerAdScript>();
            _configuringAdView = AdMobInitializer.Instance.ConfigureViewFor<BannerAdScript, BannerView>();

            string testUnitId;
#if UNITY_ANDROID
            testUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
            testUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
            testUnitId = "unexpected_platform";
#endif
            var settings = Settings;

            _isTest = AdUtils.IsTestMode(settings.TestMode)
                ? testUnitId // Google's Sample TestId 
                : settings.BannerUnitId;
            RequestAdLoad();
        }

        public BannerView GetView() => _view;

        public void RequestAdLoad()
        {
            // cleanup before reusing
            _view?.Destroy();

            _view = new BannerView(_isTest, AdSize.Banner, AdPosition.Bottom);

            BindEvents();
            _configuringAdView?.Invoke(_view);

            // Create an empty ad request.
            var builder = new AdRequest.Builder();
            _configuringAdRequest?.Invoke(builder);
            var request = builder.Build();

            // Load the banner with the request.
            _view.LoadAd(request);
        }

        public void ShowAd() => ShowAd(false);
        public void ShowAd(bool requestNew)
        {
            _view.Show();
            if (requestNew)
                RequestAdLoad();
        }

        void OnDestroy()
        {
            _view?.Destroy();
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

            instance.OnAdFailedToLoad += (sender, args) =>
                OnAdFailedToLoad?.Invoke(this, new AdErrorEventArgs { Message = args.Message });

            instance.OnAdOpening += (sender, args) => OnAdOpening?.Invoke(this, args);

            instance.OnAdClosed += (sender, args) => OnAdClosed?.Invoke(this, args);

            instance.OnAdLeavingApplication += (sender, args) => OnAdLeavingApplication?.Invoke(this, args);
        }

        #endregion
    }
}