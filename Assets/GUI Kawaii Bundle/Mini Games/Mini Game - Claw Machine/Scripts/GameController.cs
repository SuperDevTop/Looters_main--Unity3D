using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClawMachine
{
    public class GameController : MonoBehaviour
    {
        private static GameController _ins;
        public static GameController ins
        {
            get
            {
                return _ins;
            }
        }
        public ClawController clawController;

        public enum RewardEnum                  // Your Custom Reward Category
        {
            None,
            Gold,
            Energy,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Life,
            Star
        };
        [System.Serializable]
        public class MultiDimensionalArray
        {
            public RewardEnum rewardType;
            public GameObject rewardPrefab;
            public Sprite icon;
            public int minCount, maxCount;
            [HideInInspector]
            public int rewardAmount = 0;        //Amount of how many reward pulled from this currency per pull
        }

        [Header("Rewards Custom Settings")]
        [Space]
        public MultiDimensionalArray[] Rewards;

        [Header("Game Inputs")]
        [Space]
        public int pullCost;                                // How much coins user spend to pull rewards
        public int jumpForce;

        [Header("Reward Popup Panel Inputs")]
        [Space]
        public Transform rewardsContainer;
        public Transform popupPanel, popupParent;
        public GameObject popupRewardItem;                  //To show user gets which items as reward

        [Header("Rewards spawn positions")]
        [Space]
        public GameObject spawnPositionL;
        public GameObject spawnPositionR;                 //Rewards spawn positions

        [Header("UI Elements")]
        [Space]
        public Text pullCostText;
        public Text totalGoldText, totalEnergyText, totalGem1Text, totalGem2Text, totalGem3Text, totalGem4Text, totalLifeText, totalStarText;            // Pop-up text with spet or rewarded coins amount

        [Header("Effect Settings")]
        [Space]
        public ParticleSystem confetiEffect;

        private int totalGold, totalEnergy, totalGem1, totalGem2, totalGem3, totalGem4, totalLife, totalStar;                                 // Started coins amount. In your project it can be set up from PlayerProgress, DataController or from PlayerPrefs 
        private int previousGold, previousEnergy, previousGem1, previousGem2, previousGem3, previousGem4, previousLife, previousStar;         // For spent coins animation
        private int rewardMultiplier = 1;       //As default it setted to 1. If player selects x2 button and watches rewarded as set this property to 2 to double prize.

        private float waitTime = 0.08f;
        private Vector3 jump;

        private string goldString = "totalGold";
        private string energyString = "totalEnergy";
        private string gem1String = "totalGem1";
        private string gem2String = "totalGem2";
        private string gem3String = "totalGem3";
        private string gem4String = "totalGem4";
        private string lifeString = "totalLife";
        private string starString = "totalStar";

        private bool rewardsCollected = false;

        public bool RewardsCollected
        {
            get
            {
                return rewardsCollected;
            }
            set
            {
                rewardsCollected = value;
            }
        }

        private void Awake()
        {
            if (_ins == null)
                _ins = this;

            pullCostText.text = pullCost.ToString();
            GetPlayerProgress();
        }

        private void Start()
        {
            StartCoroutine(SpawnRewards());
        }

        IEnumerator SpawnRewards()
        {
            for (int i = 0; i < Rewards.Length; i++)
            {
                int rewardCount = Random.Range(Rewards[i].minCount, Rewards[i].maxCount);

                for (int r = 0; r < rewardCount; r++)
                {
                    GameObject spawnPositionRandom = (Random.Range(0, 2) > 0) ? spawnPositionL : spawnPositionR;

                    jump = new Vector3(Random.Range(-2, 3), Random.Range(-1, -4), 0.0f);
                    GameObject rewardObj = Instantiate(Rewards[i].rewardPrefab, spawnPositionRandom.transform.position, rewardsContainer.rotation, rewardsContainer);
                    rewardObj.GetComponent<Reward>().rewardType = Rewards[i].rewardType;
                    rewardObj.transform.GetComponent<Rigidbody2D>().AddForce(jump * jumpForce, ForceMode2D.Impulse);
                    yield return new WaitForSeconds(waitTime);
                }
            }
        }

        public void PullReward()
        {
            if (totalGold >= pullCost)  // Player has enough money to pull reward
            {
                ClaimPullCost();
                clawController.PullingReward();
            }
            else
            {
                Debug.LogWarning("Player does not have enough gold. Here you should open the shop for in app purchase.");
            }
        }

        private void ClaimPullCost()
        {
            previousGold = totalGold;   //Set prev value for flipping uı animation
            totalGold -= pullCost;      // Decrease cost for the turn
            StartCoroutine(UpdateRewardsAmount());
        }

        public void RewardResults()
        {
            popupRewardItem.transform.localPosition = Vector3.zero;

            for (int r = 0; r < Rewards.Length; r++)
            {
                if (Rewards[r].rewardAmount > 0) //As result, reward type may be more than one. Because of that we create gameobject for every reward type.
                {
                    GameObject popupItem = Instantiate(popupRewardItem, Vector3.zero, transform.rotation, popupParent);
                    popupItem.transform.localPosition = Vector3.zero;
                    popupItem.GetComponent<PopupRewardItem>().rewardIcon.sprite = Rewards[r].icon;
                    popupItem.GetComponent<PopupRewardItem>().rewardAmount.text = Rewards[r].rewardAmount.ToString();
                }
            }
            popupPanel.gameObject.SetActive(true);

            //If player clicks "Claim", give that reward Call ClaimReward() function OR if player selects "x2 Button", then show rewarded ad and then give double reward Call DoubleReward() function
        }

        public void CallRewardedAd()
        {
            Debug.LogWarning("Here you should call your show rewarded ad function and when rewarded ad completed call DoubleReward() function as result.");
        }

        private void DoubleReward()
        {
            rewardMultiplier = 2;
            Debug.Log("Rewards multiplier setted as " + rewardMultiplier);
            ClaimRewards();
        }

        public void ClaimRewards()
        {
            for (int r = 0; r < Rewards.Length; r++)
            {
                if (Rewards[r].rewardAmount > 0)
                {
                    switch (Rewards[r].rewardType)
                    {
                        case RewardEnum.Gold:
                            {
                                totalGold += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        case RewardEnum.Energy:
                            {
                                totalEnergy += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        case RewardEnum.Gem1:
                            {
                                totalGem1 += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        case RewardEnum.Gem2:
                            {
                                totalGem2 += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        case RewardEnum.Gem3:
                            {
                                totalGem3 += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        case RewardEnum.Gem4:
                            {
                                totalGem4 += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        case RewardEnum.Star:
                            {
                                totalStar += Rewards[r].rewardAmount * rewardMultiplier;
                            }
                            break;
                        default:
                            Debug.LogWarning("Reward can not be given unless Reward Categorgy selected for this slot... Here reward index: " + r);
                            break;
                    }
                }
                rewardMultiplier = 1;   //Reset for next rewards
            }

            confetiEffect.Play();
            StartCoroutine(UpdateRewardsAmount());
            CasualSauce.sauce.ReloadScene();
            //ResetGame();
        }

        private void ResetGame()
        {
            popupPanel.gameObject.SetActive(false);

            foreach (Transform child in popupParent.transform)
            {
                Destroy(child.gameObject);
            }
            for (int r = 0; r < Rewards.Length; r++)
            {
                Rewards[r].rewardAmount = 0;
            }
            foreach (Transform child in rewardsContainer.transform)
            {
                Destroy(child.gameObject);
            }

            clawController.ClawPositionReset();
            StartCoroutine(SpawnRewards());
        }

        private IEnumerator UpdateRewardsAmount()
        {
            // Animation for increasing and decreasing of coins amount
            const float seconds = 0.5f;
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                totalGoldText.text = Mathf.Floor(Mathf.Lerp(previousGold, totalGold, (elapsedTime / seconds))).ToString();
                totalLifeText.text = Mathf.Floor(Mathf.Lerp(previousLife, totalLife, (elapsedTime / seconds))).ToString();
                totalGem1Text.text = Mathf.Floor(Mathf.Lerp(previousGem1, totalGem1, (elapsedTime / seconds))).ToString();
                totalGem2Text.text = Mathf.Floor(Mathf.Lerp(previousGem2, totalGem2, (elapsedTime / seconds))).ToString();
                totalGem3Text.text = Mathf.Floor(Mathf.Lerp(previousGem3, totalGem3, (elapsedTime / seconds))).ToString();
                totalGem4Text.text = Mathf.Floor(Mathf.Lerp(previousGem4, totalGem4, (elapsedTime / seconds))).ToString();
                totalEnergyText.text = Mathf.Floor(Mathf.Lerp(previousEnergy, totalEnergy, (elapsedTime / seconds))).ToString();
                totalStarText.text = Mathf.Floor(Mathf.Lerp(previousStar, totalStar, (elapsedTime / seconds))).ToString();

                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            previousGold = totalGold;
            previousLife = totalLife;
            previousGem1 = totalGem1;
            previousGem2 = totalGem2;
            previousGem3 = totalGem3;
            previousGem4 = totalGem4;
            previousEnergy = totalEnergy;
            previousStar = totalStar;

            totalGoldText.text = totalGold.ToString();
            totalLifeText.text = totalLife.ToString();
            totalGem1Text.text = totalGem1.ToString();
            totalGem2Text.text = totalGem2.ToString();
            totalGem3Text.text = totalGem3.ToString();
            totalGem4Text.text = totalGem4.ToString();
            totalEnergyText.text = totalEnergy.ToString();
            totalStarText.text = totalStar.ToString();

            SavePlayerProgress(goldString, totalGold);
            SavePlayerProgress(energyString, totalEnergy);
            SavePlayerProgress(gem1String, totalGem1);
            SavePlayerProgress(gem2String, totalGem2);
            SavePlayerProgress(gem3String, totalGem3);
            SavePlayerProgress(gem4String, totalGem4);
            SavePlayerProgress(lifeString, totalLife);
            SavePlayerProgress(starString, totalStar);
        }
        private void GetPlayerProgress()
        {
            //Gold
            if (PlayerPrefs.HasKey(goldString))
            {
                totalGold = PlayerPrefs.GetInt(goldString);
            }
            else
            {
                totalGold = 100; //Default Gold Value
                PlayerPrefs.SetInt(goldString, totalGold);
                PlayerPrefs.Save();
            }
            //Energy
            if (PlayerPrefs.HasKey(energyString))
            {
                totalEnergy = PlayerPrefs.GetInt(energyString);
            }
            else
            {
                totalEnergy = 0;
                PlayerPrefs.SetInt(energyString, totalEnergy);
            }
            //Life
            if (PlayerPrefs.HasKey(lifeString))
            {
                totalLife = PlayerPrefs.GetInt(lifeString);
            }
            else
            {
                totalLife = 0;
                PlayerPrefs.SetInt(lifeString, totalLife);
            }
            //Star
            if (PlayerPrefs.HasKey(starString))
            {
                totalStar = PlayerPrefs.GetInt(starString);
            }
            else
            {
                totalStar = 0;
                PlayerPrefs.SetInt(starString, totalStar);
            }
            //Gem1
            if (PlayerPrefs.HasKey(gem1String))
            {
                totalGem1 = PlayerPrefs.GetInt(gem1String);
            }
            else
            {
                totalGem1 = 0;
                PlayerPrefs.SetInt(gem1String, totalGem1);
            }
            //Gem2
            if (PlayerPrefs.HasKey(gem2String))
            {
                totalGem2 = PlayerPrefs.GetInt(gem2String);
            }
            else
            {
                totalGem2 = 0;
                PlayerPrefs.SetInt(gem2String, totalGem2);
            }
            //Gem3
            if (PlayerPrefs.HasKey(gem3String))
            {
                totalGem3 = PlayerPrefs.GetInt(gem3String);
            }
            else
            {
                totalGem3 = 0;
                PlayerPrefs.SetInt(gem3String, totalGem3);
            }
            //Gem4
            if (PlayerPrefs.HasKey(gem4String))
            {
                totalGem4 = PlayerPrefs.GetInt(gem4String);
            }
            else
            {
                totalGem4 = 0;
                PlayerPrefs.SetInt(gem4String, totalGem4);
            }

            PlayerPrefs.Save();
            StartCoroutine(UpdateRewardsAmount());
        }

        private void SavePlayerProgress(string st, int value)
        {
            PlayerPrefs.SetInt(st, value);
            PlayerPrefs.Save();
        }
    }
}