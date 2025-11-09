using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections.Generic;
namespace DailyRewardSystem {

    [Serializable] public struct Reward
    {
        public int Amount;
        public int Day;
    }
    public class DailyRewards : MonoBehaviour
    {
        [Header("Main Menu UI")]
        [SerializeField] private TMPro.TMP_Text totalCoins;

        [Space]
        [Header("Reward UI")]
        [SerializeField] private GameObject rewardPanel;
        [SerializeField] private Button openRewardButton;
        [SerializeField] private Button closeRewardButton;
        [SerializeField] private Button claimButton;
        [SerializeField] private GameObject notification;
        
        [Space]
        [Header("Reward Database")]
        [SerializeField] private RewardsDatabase rewardsDB;
        [SerializeField] private List<GameObject> rewardsPanel;

        [Space]
        [Header("Timing")]
        [SerializeField] private double nextRewardDelay = 1f;
        [SerializeField] private float checkInterval = 300f;
       
        private int nextRewardIndex;

        private void Start()
        {
            this.rewardPanel.gameObject.SetActive(false);
            this.Initialize();
            InvokeRepeating(nameof(CheckForReward), 0f, checkInterval);
        }

        private void Initialize()
        {
            //Load Next Reward Index
            this.LoadNextRewardIndex();

            //index = 0 thì ẩn các reward khác
            this.LoadAllReward();
            this.LoadPreviousReward();

            //Add event click
            this.AddEventClick();

            // Check the first time
            this.SetDateTimeFirstTime();
        }
            
        //Open/Close UI
        private void OnOpenRewardButtonClick()
        {
            rewardPanel.SetActive(true);
           
        }
        
        private void OnCloseRewardButtonClick()
        {
           
            rewardPanel.SetActive(false);
        }

        private void OnClaimButtonClick()
        {
            Reward reward = rewardsDB.GetReward(nextRewardIndex);
            GameData.Instance.ToTalCoins += reward.Amount;
            this.UpdateCoinsTextUI();
            DeactiveReward();
            Debug.Log("<color=yelow>" + reward.Amount + " index: " + nextRewardIndex);
            this.GetNoRewardPanel(nextRewardIndex).SetActive(true);
      
            this.nextRewardIndex++;
            if(nextRewardIndex >= rewardsDB.Count)
            {
                this.nextRewardIndex = 0;
            }
            
            PlayerPrefs.SetString(GameSetting.TIMEDATE_REWARD_DELAY, DateTime.Now.ToString());
            PlayerPrefs.SetInt(GameSetting.NEXT_REWARD_INDEX, nextRewardIndex);
            PlayerPrefs.Save();
        }
        public void UpdateCoinsTextUI()
        {
            this.totalCoins.text = GameData.Instance.ToTalCoins.ToString();
        }

        private void ActivateReward()
        {
            claimButton.interactable = true;
            notification.SetActive(true);
        }

       
        private void CheckForReward()
        {
            DateTime current = DateTime.Now;
            DateTime lastClaimTime = DateTime.Parse(PlayerPrefs.GetString(GameSetting.TIMEDATE_REWARD_DELAY.ToString()));
            double elapsed = (current - lastClaimTime).TotalDays;
         //   Debug.Log("current: " + current + " lasttime: " + lastClaimTime + " elap: " + elapsed + "day: " + nextRewardDelay);
            if(elapsed >= nextRewardDelay)
            {
                this.ActivateReward();
               // Debug.Log("Active");
            }
            else
            {
                this.DeactiveReward();
              //  Debug.Log("Deactive");
            }
        }

        private void DeactiveReward()
        {
            claimButton.interactable = false;
            notification.SetActive(false);
        }

        private void AddEventClick()
        {
            openRewardButton.onClick.RemoveAllListeners();
            openRewardButton.onClick.AddListener(OnOpenRewardButtonClick);

            closeRewardButton.onClick.RemoveAllListeners();
            closeRewardButton.onClick.AddListener(OnCloseRewardButtonClick);

            claimButton.onClick.RemoveAllListeners();
            claimButton.onClick.AddListener(OnClaimButtonClick);


        }
        private void LoadNextRewardIndex()
        {
            nextRewardIndex = PlayerPrefs.GetInt(GameSetting.NEXT_REWARD_INDEX, 0);
        }

        private void LoadAllReward()
        {
            if(nextRewardIndex == 0)
            {
                for(int i = 0; i < rewardsDB.Count; i++)
                {
                    this.GetNoRewardPanel(i).SetActive(false);
                }
            }
        }

        private void LoadPreviousReward()
        {
            if (nextRewardIndex > 0)
            {
                for (int i = 0; i < nextRewardIndex; i++)
                {
                    this.GetNoRewardPanel(i).SetActive(true);
                }
            }
        }

        private GameObject GetNoRewardPanel(int index)
        {
            return rewardsPanel[index].transform.Find("NoReward").gameObject;
        }

        private void SetDateTimeFirstTime()
        {
            if (string.IsNullOrEmpty(PlayerPrefs.GetString(GameSetting.TIMEDATE_REWARD_DELAY)))
            {
                PlayerPrefs.SetString(GameSetting.TIMEDATE_REWARD_DELAY, DateTime.Now.ToString());
                PlayerPrefs.Save();
            }
        }
    }
}

