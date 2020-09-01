using System;
using Assets.FUGAS.Ads.Helpers;
using GoogleMobileAds.Api;
using UnityEngine;

namespace Assets.FUGAS.Ads
{
    public class RewardedAdScript : MonoBehaviour, IAdProvider
    {
        public AdvertisementSettings Settings;

        private RewardedAd _view;
        private string _isTest;
        private Action<AdRequest.Builder> _configuringAdRequest;
        private Action<RewardedAd> _configuringAdView;
        private bool _viewUsed;

        public void Start()
        {
            AdMobInitializer.EnsureReady();
            _configuringAdRequest = AdMobInitializer.Instance.ConfigureAdRequestFor<RewardedAdScript>();
            _configuringAdView = AdMobInitializer.Instance.ConfigureViewFor<RewardedAdScript, RewardedAd>();

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
            RequestAdLoad();
        }
         
        public RewardedAd GetView() => _view;

        public void RequestAdLoad()
        {
            if (!_viewUsed && _view != default && _view.IsLoaded())
            {
                // acts like cached ad => less data usage
                return;
            }
            _view = new RewardedAd(_isTest);

            // Called when the user should be rewarded for interacting with the ad.
            _view.OnUserEarnedReward += HandleUserEarnedReward;
            _view.OnPaidEvent += HandleOnPaidEvent;
            BindEvents();
            _configuringAdView?.Invoke(_view);

            // Create an empty ad request.
            var builder = new AdRequest.Builder();
            _configuringAdRequest?.Invoke(builder);
            var request = builder.Build();
            // Load the rewarded ad with the request.
            _view.LoadAd(request);
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
        }

        #endregion
    }
}