using UnityEngine;

namespace Assets.FUGAS.Ads.Scripts
{
    [CreateAssetMenu(fileName = "Advertisement Settings", menuName = "FUGAS/Ads/Advertisement Settings", order = 1)]
    public class AdvertisementSettings : ScriptableObject
    { 
        public string InterstitialUnitId;
        public string RewardedUnitId;
        public string BannerUnitId;
        public bool TestMode = true;
    }
}