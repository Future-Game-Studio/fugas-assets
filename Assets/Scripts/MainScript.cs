using System.Text;
using Assets.FUGAS.Ads.Scripts;
using UnityEngine;
using UnityEngine.UI; 

namespace Assets.Scripts
{
    public class MainScript : MonoBehaviour
    {
        private Text statusText;
        private Text levelCaption;
        private Text rewardCaption;
        private InterstitialAdScript _interstitialAd;
        private RewardedAdScript _rewardedAd;
        private int _currentLevel = 1;
        private double _rewardValue;
        private string _rewardType;
        private StringBuilder prevStatus = new StringBuilder();
        private void Awake()
        {
            AdMobInitializer.Configure(x =>
            {
                // configuring view events. 
                // NOTE: they are NOT synchronized with unity threads!

                x.ConfigureRewardedAd(r => { }, v =>
                {
                    v.OnAdClosed += (s, e) => { SetStatus(s.GetType().Name + " closed (configured event)"); };
                    v.OnAdFailedToShow += (s, e) => { SetStatus(s.GetType().Name + " failed (configured event)"); };
                    v.OnAdOpening += (s, e) => { SetStatus(s.GetType().Name + " oppening (configured event)"); };
                });
                x.ConfigureInterstitialAd(r => { }, v =>
                {
                    v.OnAdClosed += (s, e) => { SetStatus(s.GetType().Name + " closed (configured event)"); };
                    v.OnAdLoaded += (s, e) => { SetStatus(s.GetType().Name + " loaded (configured event)"); };
                    v.OnAdOpening += (s, e) => { SetStatus(s.GetType().Name + " oppening (configured event)"); };
                });
            });
        }

        private void SetStatus(string v)
        {
            // you MUST use thread context synchronization on Google AdMob events
            // that's because natively ads writen on Java
            // and there some RPC invocations to external libraries
            SyncContext.RunOnUnityThread(() =>
            {
                prevStatus.AppendLine(v);
                statusText.text = prevStatus.ToString();
            });
        }

        // Start is called before the first frame update
        void Start()
        {
            statusText = GameObject.Find("StatusText").GetComponent<Text>();
            levelCaption = GameObject.Find("LevelCaption").GetComponent<Text>();
            rewardCaption = GameObject.Find("RewardCaption").GetComponent<Text>();

            // these events are already synchronized by SyncContext!
            _interstitialAd = FindObjectOfType<InterstitialAdScript>();
            _interstitialAd.OnAdLoaded += (s, e) => SetStatus("InterstitialAd loaded! (OnStart event)");
            _rewardedAd = FindObjectOfType<RewardedAdScript>();
            _rewardedAd.OnAdLoaded += (s, e) => SetStatus("RewardedAd loaded! (OnStart event)");
        }

        // Update is called once per frame
        void Update()
        {
            levelCaption.text = $"Level: {_currentLevel}";
            rewardCaption.text = $"Reward: {_rewardType} {_rewardValue}";
        }

        public void OnNextLevelClick()
        {
            _currentLevel++;

            // add will be shown every multiple of 3  
            if (_currentLevel % 3 == 0)
            {
                if (_interstitialAd)
                {
                    _interstitialAd.ShowAd(true);
                }
            }
        }

        public void OnGetRewardClick()
        {
            if (_rewardedAd)
            {
                var view = _rewardedAd.GetView();
                view.OnUserEarnedReward += (s, e) =>
                {
                    var reward = view.GetRewardItem();
                    if (reward != default)
                    {
                        _rewardType = reward.Type;
                        _rewardValue += reward.Amount;
                    }
                    else
                    {
                        // reward can be null on debug
                        _rewardValue++;
                    }
                };
                _rewardedAd.ShowAd(true);
            }
        }

        public void Exit() => Application.Quit(0);
    }
}