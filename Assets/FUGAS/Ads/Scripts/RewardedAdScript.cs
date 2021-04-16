using System;
using Assets.FUGAS.Ads.Scripts.Abstractions;
using Assets.FUGAS.Ads.Scripts.Helpers;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts
{
    public class RewardedAdScript : MonoBehaviour, IAdProvider
    {
        public AdvertisementSettings Settings;

        private RewardedAd _view;
        private string _isTest;
        private Action<AdRequest.Builder> _configuringAdRequest;
        private Action<RewardedAd> _configuringAdView;
        private bool _viewUsed;
        private AdRequest _request;

        public void Awake()
        {
            AdMobInitializer.EnsureReady();

            string testUnitId;
#if UNITY_ANDROID
            testUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            testUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            testUnitId = "unexpected_platform";
#endif
            var settings = Settings;

            _isTest = AdUtils.IsTestMode(settings.TestMode)
                ? testUnitId // Google's Sample TestId
                : settings.RewardedUnitId;
        }
        private void Start()
        {
            CreateAdRequest();
            LoadAd();
        }
        public RewardedAd GetView() => _view;
        public bool IsReady => _view != default && !_viewUsed;

        public void CreateAdRequest()
        {
            if (!_viewUsed && _view != default && _view.IsLoaded())
            {
                // acts like cached ad => less data usage
                return;
            }
            _viewUsed = false;
            _configuringAdRequest = AdMobInitializer.Instance.ConfigureAdRequestFor<RewardedAdScript>();
            _configuringAdView = AdMobInitializer.Instance.ConfigureView<RewardedAd>();
            _view = new RewardedAd(_isTest);

            // Called when the user should be rewarded for interacting with the ad.
            _view.OnUserEarnedReward += HandleUserEarnedReward;
            _view.OnPaidEvent += HandleOnPaidEvent;
            BindEvents();
            _configuringAdView?.Invoke(_view);

            // Create an empty ad request.
            var builder = new AdRequest.Builder();
            _configuringAdRequest?.Invoke(builder);
            _request = builder.Build();
        }

        private void HandleUserEarnedReward(object sender, Reward e)
        {
            print("User earned reward event received");
        }

        private void HandleOnPaidEvent(object sender, AdValueEventArgs e)
        {
            print("HandlePaid event received");
        }

        public void OnShowRewardClick()
        {
            ShowAd();
        }

        public void LoadAd()
        {
            if (_request == default)
                CreateAdRequest();
            // Load the rewarded ad with the request.
            _view.LoadAd(_request);
        }
        public void ShowAd() => ShowAd(true);
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
        [Obsolete]
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
        }

        #endregion
    }
}