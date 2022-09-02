using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DropGame
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

        public enum RewardEnum                  // Your Custom Reward Category
        {
            None,
            Gold,
            Energy,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };
        [System.Serializable]
        public class MultiDimensionalArray
        {
            public RewardEnum rewardType;
            public GameObject rewardPrefab;
            public Sprite icon;
            public Texture confetiIcon;         //For animation after claim reward
            public int rewardAmount;            //Amount of how many reward earned from this currency

        }
        [Header("Rewards Custom Settings")]
        [Space]
        public MultiDimensionalArray[] RewardSlots;

        [Header("Game Inputs")]
        [Space]
        public GameObject[] rewardSpawnPosition;
        public GameObject coinObjectPrefab; //Dropping Coin Object
        public int minXLimit, maxXLimit;        //X position limits for moving coin object
        public int dropCost;

        [Header("Reward Popup Panel Inputs")]
        [Space]
        public Transform popupPanel;
        public Transform coinContainer;

        [Header("UI Elements")]
        [Space]
        public Image rewardIcon;
        public Button dropButton;
        public Text rewardAmount;
        public Text totalGoldText, totalEnergyText, totalGem1Text, totalGem2Text, totalGem3Text, totalGem4Text, totalMoneyText;             // Pop-up text with spet or rewarded coins amount
        public Text dropCostText;

        [Header("Effect Settings")]
        [Space]
        public ParticleSystem confetiEffect;

        private int totalGold, totalEnergy, totalGem1, totalGem2, totalGem3, totalGem4, totalMoney;          // Started coins amount. In your project it can be set up from PlayerProgress, DataController or from PlayerPrefs 
        private int previousGold, previousEnergy, previousGem1, previousGem2, previousGem3, previousGem4, previousMoney;         // For spent coins animation
        private int collectedRewardIndex;
        private int rewardMultiplier = 1;       //As default it setted to 1. If player selects x2 button and watches rewarded as set this property to 2 to double prize.

        private float coinSpeedTime;            //Here Important Point ---> 0.1f slower than 0.01f because its pause point every shift movement
        private float waitTime = 0.1f;

        private bool isDrop;

        private string moveDirection = "Left";  //Go Left side first at start
        private string goldString = "totalGold";
        private string energyString = "totalEnergy";
        private string gem1String = "totalGem1";
        private string gem2String = "totalGem2";
        private string gem3String = "totalGem3";
        private string gem4String = "totalGem4";
        private string moneyString = "totalMoney";

        private Vector3 coinStartPosition = new Vector3(0, 780, 0);
        private GameObject coinObject;

        public int CollectedRewardIndex
        {
            get
            {
                return collectedRewardIndex;
            }
            set
            {
                collectedRewardIndex = value;
            }
        }

        private void Awake()
        {
            if (_ins == null)
                _ins = this;

            dropCostText.text = dropCost.ToString();
            coinObject = Instantiate(coinObjectPrefab, Vector3.zero, coinContainer.rotation, coinContainer);
            coinObject.transform.localPosition = coinStartPosition;
            GetPlayerProgress();
        }

        private void Start()
        {
            StartCoroutine(SpawnRewards());
        }

        IEnumerator SpawnRewards()
        {
            for (int r = 0; r < RewardSlots.Length; r++)
            {
                GameObject rewardObj = Instantiate(RewardSlots[r].rewardPrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), rewardSpawnPosition[r].transform);
                rewardObj.transform.localPosition = Vector3.zero;
                rewardObj.GetComponent<Reward>().rewardAmountText.text = RewardSlots[r].rewardAmount.ToString();

                yield return new WaitForSeconds(waitTime);
            }

            dropButton.interactable = true;
            StartCoroutine(MoveCoin());
        }

        IEnumerator MoveCoin()
        {
            isDrop = false;

            while (!isDrop)
            {
                if (moveDirection == "Left")
                {
                    if (Mathf.RoundToInt(coinObject.transform.localPosition.x) < maxXLimit)
                    {
                        coinObject.transform.localPosition += new Vector3(10, 0, 0);
                    }
                    else
                        moveDirection = "Right";

                    yield return new WaitForSeconds(coinSpeedTime);
                }
                else if (moveDirection == "Right")
                {
                    if (Mathf.RoundToInt(coinObject.transform.localPosition.x) > minXLimit)
                    {
                        coinObject.transform.localPosition += new Vector3(-10, 0, 0);
                    }
                    else
                        moveDirection = "Left";

                    yield return new WaitForSeconds(coinSpeedTime);
                }
            }
        }

        public void DropCoin()
        {
            if (totalGold >= dropCost)
            {
                dropButton.interactable = false;
                isDrop = true;

                ClaimDropCost();

                coinObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            }
            else
            {
                Debug.LogWarning("Player does not have enough gold. Here you should open the shop for in app purchase.");
            }
        }

        private void ClaimDropCost()
        {
            previousGold = totalGold;
            totalGold -= dropCost;  // Decrease money for the droping coin
            totalGoldText.text = totalGold.ToString();

        }

        public void RewardResults()
        {
            rewardSpawnPosition[collectedRewardIndex].GetComponentInChildren<Animator>().SetBool("isRewardFly", true);
            rewardIcon.sprite = RewardSlots[collectedRewardIndex].icon;
            rewardAmount.text = RewardSlots[collectedRewardIndex].rewardAmount.ToString();
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
            ClaimReward();
        }

        public void ClaimReward()
        {
            rewardSpawnPosition[collectedRewardIndex].GetComponentInChildren<Animator>().SetBool("isRewardFly", false);
            confetiEffect.GetComponent<ParticleSystemRenderer>().material.mainTexture = RewardSlots[collectedRewardIndex].confetiIcon;
            RewardEnum collectedRewardType = RewardSlots[collectedRewardIndex].rewardType;

            switch (collectedRewardType)
            {
                case RewardEnum.Gold:
                    {
                        totalGold += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Energy:
                    {
                        totalEnergy += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem1:
                    {
                        totalGem1 += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem2:
                    {
                        totalGem2 += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem3:
                    {
                        totalGem3 += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem4:
                    {
                        totalGem4 += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Money:
                    {
                        totalMoney += RewardSlots[collectedRewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                default:
                    Debug.LogWarning("Reward can not be given unless Reward Categorgy selected for this slot... Here reward index: " + collectedRewardIndex);
                    break;
            }
            rewardMultiplier = 1;   //Reset for next rewards
            confetiEffect.Play();
            StartCoroutine(UpdateRewardsAmount());
            CasualSauce.sauce.ReloadScene();
            //ResetGame();
        }

        private void ResetGame()
        {
            popupPanel.gameObject.SetActive(false);
            isDrop = false;
            Destroy(coinObject);
            coinObject = Instantiate(coinObjectPrefab, Vector3.zero, coinContainer.rotation, coinContainer);
            coinObject.transform.localPosition = coinStartPosition;
            StartCoroutine(MoveCoin());
            dropButton.interactable = true;
        }

        private IEnumerator UpdateRewardsAmount()
        {
            // Animation for increasing and decreasing of coins amount
            const float seconds = 0.5f;
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                totalGoldText.text = Mathf.Floor(Mathf.Lerp(previousGold, totalGold, (elapsedTime / seconds))).ToString();
                totalEnergyText.text = Mathf.Floor(Mathf.Lerp(previousEnergy, totalEnergy, (elapsedTime / seconds))).ToString();
                totalGem1Text.text = Mathf.Floor(Mathf.Lerp(previousGem1, totalGem1, (elapsedTime / seconds))).ToString();
                totalGem2Text.text = Mathf.Floor(Mathf.Lerp(previousGem2, totalGem2, (elapsedTime / seconds))).ToString();
                totalGem3Text.text = Mathf.Floor(Mathf.Lerp(previousGem3, totalGem3, (elapsedTime / seconds))).ToString();
                totalGem4Text.text = Mathf.Floor(Mathf.Lerp(previousGem4, totalGem4, (elapsedTime / seconds))).ToString();
                totalMoneyText.text = Mathf.Floor(Mathf.Lerp(previousMoney, totalMoney, (elapsedTime / seconds))).ToString();

                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            previousGold = totalGold;
            previousEnergy = totalEnergy;
            previousGem1 = totalGem1;
            previousGem2 = totalGem2;
            previousGem3 = totalGem3;
            previousGem4 = totalGem4;
            previousMoney = totalMoney;

            totalGoldText.text = totalGold.ToString();
            totalEnergyText.text = totalEnergy.ToString();
            totalGem1Text.text = totalGem1.ToString();
            totalGem2Text.text = totalGem2.ToString();
            totalGem3Text.text = totalGem3.ToString();
            totalGem4Text.text = totalGem4.ToString();
            totalMoneyText.text = totalMoney.ToString();

            SavePlayerProgress(goldString, totalGold);
            SavePlayerProgress(energyString, totalEnergy);
            SavePlayerProgress(gem1String, totalGem1);
            SavePlayerProgress(gem2String, totalGem2);
            SavePlayerProgress(gem3String, totalGem3);
            SavePlayerProgress(gem4String, totalGem4);
            SavePlayerProgress(moneyString, totalMoney);
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
            //Money
            if (PlayerPrefs.HasKey(moneyString))
            {
                totalMoney = PlayerPrefs.GetInt(moneyString);
            }
            else
            {
                totalMoney = 0;
                PlayerPrefs.SetInt(moneyString, totalMoney);
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