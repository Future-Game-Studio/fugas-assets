using Assets.FUGAS.Ads;
using Assets.FUGAS.Ads.Scripts;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class MainScript : MonoBehaviour
    {
        private Text levelCaption;
        private Text rewardCaption;
        private int _currentLevel = 1;
        private double _rewardValue;
        private string _rewardType;

        // Start is called before the first frame update
        void Start()
        {
            levelCaption = GameObject.Find("LevelCaption").GetComponent<Text>();
            rewardCaption = GameObject.Find("RewardCaption").GetComponent<Text>();
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
                var ad = FindObjectOfType<InterstitialAdScript>();
                if (ad != default)
                {
                    ad.ShowAd(true);
                }
            }
        }

        public void OnGetRewardClick()
        {
            var ad = FindObjectOfType<RewardedAdScript>();
            if (ad != default)
            {
                var view = ad.GetView(); 
                view.OnUserEarnedReward += (s, e) =>
                {
                    var reward = view.GetRewardItem();
                    _rewardType = reward.Type;
                    _rewardValue += reward.Amount;
                };
                ad.ShowAd(true);
            }
        }

        public void Exit() => Application.Quit(0);
    }
}