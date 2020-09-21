using System;
using GoogleMobileAds.Api;

namespace Assets.FUGAS.Ads.Scripts
{
    public interface IAdProvider
    {
        event EventHandler<AdErrorEventArgs> OnAdFailedToLoad;

        event EventHandler<EventArgs> OnAdLoaded;

        event EventHandler<EventArgs> OnAdOpening;

        event EventHandler<EventArgs> OnAdClosed;

        event EventHandler<EventArgs> OnAdLeavingApplication;

        /// <summary>
        /// Creates a request to load advertisement
        /// </summary>
        void CreateAdRequest();

        /// <summary>
        /// Loads ad using request
        /// </summary>
        void LoadAd();

        /// <summary>
        /// Invokes ad view and requests new ad if used
        /// </summary>
        void ShowAd(bool requestNew);

        /// <summary>
        /// Invokes ad view
        /// </summary>
        void ShowAd();
    }
}