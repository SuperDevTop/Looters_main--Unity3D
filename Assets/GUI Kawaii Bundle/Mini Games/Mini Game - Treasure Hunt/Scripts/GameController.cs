using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace TreasureHunt
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
        public enum RewardEnum                      // Your Custom Reward Category
        {
            None,
            Gold,
            Energy,
            Life,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };

        [System.Serializable]
        public class MultiDimensionalRewards
        {
            public RewardEnum rewardType;
            public Sprite icon;
            public Texture confetiIcon;             //For animation after claim reward
            public int rewardAmount;                //Amount of how many reward earned from this currency
            [HideInInspector]
            public int lootboxId;
        }
        [Header("Rewards Custom Settings")]
        [Space]
        public MultiDimensionalRewards[] LootboxRewards;

        [Header("Game Inputs")]
        [Space]
        public GameObject lootboxPrefab;
        public Transform[] lootboxContainers;       //LootboxRewards and LootboxContainers lenght should be equal (LootboxCount = RewardCount)
        public int keyCount;                        //One key opens only one lootboxs

        [Header("Reward Popup Panel Inputs")]
        [Space]
        public Transform popupPanel, popupParent;
        public GameObject popupRewardItem;          //To show user gets which items as reward

        [Header("UI Elements")]
        [Space]
        public GameObject keyObject;                //For animation
        public GameObject noMoreKeyLabel;
        public GameObject noMoreKeyText;
        public Text keyCountText;
        public Text totalGoldText, totalEnergyText, totalGem1Text, totalGem2Text, totalGem3Text, totalGem4Text, totalLifeText, totalMoneyText;   // Pop-up text

        [Header("Effect Settings")]
        [Space]
        public ParticleSystem confetiEffect;

        static System.Random _random = new System.Random(); // Used in Shuffle(T).

        private List<int> selectedRewardIds = new List<int>();
        private int[] LootboxIndex;
        private int openedLootboxCount = 0;
        private int rewardMultiplier = 1;       //As default it setted to 1. If player selects x2 button and watches rewarded as set this property to 2 to double prize.
        private int totalGold, totalEnergy, totalGem1, totalGem2, totalGem3, totalGem4, totalLife, totalMoney;                                  // Started coins amount. In your project it can be set up from PlayerProgress, DataController or from PlayerPrefs 
        private int previousGold, previousEnergy, previousGem1, previousGem2, previousGem3, previousGem4, previousLife, previousMoney;          // For spent coins animation
        private int collectedGold, collectedEnergy, collectedGem1, collectedGem2, collectedGem3, collectedGem4, collectedLife, collectedMoney;
        private int goldIndex, energyIndex, gem1Index, gem2Index, gem3Index, gem4Index, moneyIndex, lifeIndex;                                  //Just for using icons

        private string goldString = "totalGold";
        private string energyString = "totalEnergy";
        private string gem1String = "totalGem1";
        private string gem2String = "totalGem2";
        private string gem3String = "totalGem3";
        private string gem4String = "totalGem4";
        private string lifeString = "totalLife";
        private string moneyString = "totalMoney";

        private void Awake()
        {
            if (_ins == null)
                _ins = this;

            keyCountText.text = keyCount.ToString();
            GetPlayerProgress();
        }

        private void Start()
        {
            SpawnLootboxes();
            SetLootboxIndexes(); //Just at start
        }

        private void SpawnLootboxes()
        {
            for (int i = 0; i < LootboxRewards.Length; i++)
            {
                GameObject lootboxObj = Instantiate(lootboxPrefab, Vector3.zero, transform.rotation, lootboxContainers[i]);
                lootboxObj.transform.localPosition = Vector3.zero;
            }
        }

        private void SetLootboxIndexes()
        {
            LootboxIndex = new int[LootboxRewards.Length];
            for (int i = 0; i < LootboxRewards.Length; i++)
            {
                LootboxIndex[i] = i;
            }

            SetRewardPositions();
        }

        private void SetRewardPositions()
        {
            Shuffle(LootboxIndex);                          //Shuffle Positions

            for (int i = 0; i < LootboxRewards.Length; i++) //After shuffle positions set rewards one by one
            {
                LootboxRewards[i].lootboxId = LootboxIndex[i];
            }
        }

        static void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            for (int i = 0; i < (n - 1); i++)
            {
                // Use Next on random instance with an argument.
                // ... The argument is an exclusive bound.
                //     So we will not go past the end of the array.
                int r = i + _random.Next(n - i);
                T t = array[r];
                array[r] = array[i];
                array[i] = t;
            }
        }

        public void LootboxSelected(int index)
        {
            if (keyCount > 0)
            {
                keyObject.GetComponent<Animator>().SetBool("KeyUsed", true);
                openedLootboxCount += 1;

                for (int i = 0; i < LootboxRewards.Length; i++)
                {
                    if (LootboxRewards[i].lootboxId == index)
                    {
                        Debug.Log("Selected lootbox contains " + LootboxRewards[i].rewardAmount.ToString() + " " + LootboxRewards[i].rewardType.ToString());
                        lootboxContainers[index].GetChild(0).GetComponent<Animator>().Play("Loot Box Open");

                        lootboxContainers[index].parent.GetChild(1).gameObject.SetActive(true);

                        lootboxContainers[index].GetChild(0).GetComponent<Lootbox>().rewardImage.sprite = LootboxRewards[i].icon;
                        lootboxContainers[index].GetChild(0).GetComponent<Lootbox>().rewardText.text = LootboxRewards[i].rewardAmount.ToString();
                        selectedRewardIds.Add(i);

                        keyCount -= 1;
                        keyCountText.text = keyCount.ToString();

                        if (keyCount == 0)
                        {
                            StartCoroutine(RewardResults());
                            noMoreKeyLabel.gameObject.SetActive(true);
                            noMoreKeyText.gameObject.SetActive(true);
                        }
                        else
                            Debug.Log("Player can continue to open lootboxes");
                        break;
                    }
                }
            }
            StartCoroutine(SetAnimationFalse());
        }

        IEnumerator SetAnimationFalse()
        {
            yield return new WaitForSeconds(1f);
            keyObject.GetComponent<Animator>().SetBool("KeyUsed", false);
        }

        public void AddKey()
        {
            if (openedLootboxCount + keyCount < 9) //We do not want to give more keys than player need to open all lootboxes
            {
                keyCount += 1;
                keyCountText.text = keyCount.ToString();

                noMoreKeyText.gameObject.SetActive(false);

                if (openedLootboxCount + keyCount == 9)
                    noMoreKeyLabel.gameObject.SetActive(false);
            }
        }

        private IEnumerator RewardResults()
        {
            yield return new WaitForSeconds(2f);

            for (int r = 0; r < selectedRewardIds.Count; r++)
            {
                int index = selectedRewardIds[r];

                switch (LootboxRewards[index].rewardType)
                {
                    case RewardEnum.Gold:
                        {
                            collectedGold += LootboxRewards[index].rewardAmount;
                            goldIndex = index;
                        }
                        break;
                    case RewardEnum.Energy:
                        {
                            collectedEnergy += LootboxRewards[index].rewardAmount;
                            energyIndex = index;
                        }
                        break;
                    case RewardEnum.Gem1:
                        {
                            collectedGem1 += LootboxRewards[index].rewardAmount;
                            gem1Index = index;
                        }
                        break;
                    case RewardEnum.Gem2:
                        {
                            collectedGem2 += LootboxRewards[index].rewardAmount;
                            gem2Index = index;
                        }
                        break;
                    case RewardEnum.Gem3:
                        {
                            collectedGem3 += LootboxRewards[index].rewardAmount;
                            gem3Index = index;
                        }
                        break;
                    case RewardEnum.Gem4:
                        {
                            collectedGem4 += LootboxRewards[index].rewardAmount;
                            gem4Index = index;
                        }
                        break;
                    case RewardEnum.Money:
                        {
                            collectedMoney += LootboxRewards[index].rewardAmount;
                            moneyIndex = index;
                        }
                        break;
                    case RewardEnum.Life:
                        {
                            collectedLife += LootboxRewards[index].rewardAmount;
                            lifeIndex = index;
                        }
                        break;
                    default:
                        Debug.LogWarning("Reward can not be given unless Reward Categorgy selected for this slot... Here reward index: " + index);
                        break;
                }

            }

            if (collectedGold > 0)
            {
                CreateRewardItem(goldIndex, collectedGold);
            }
            if (collectedEnergy > 0)
            {
                CreateRewardItem(energyIndex, collectedEnergy);
            }
            if (collectedGem1 > 0)
            {
                CreateRewardItem(gem1Index, collectedGem1);
            }
            if (collectedGem2 > 0)
            {
                CreateRewardItem(gem2Index, collectedGem2);
            }
            if (collectedGem3 > 0)
            {
                CreateRewardItem(gem3Index, collectedGem3);
            }
            if (collectedGem4 > 0)
            {
                CreateRewardItem(gem4Index, collectedGem4);
            }
            if (collectedMoney > 0)
            {
                CreateRewardItem(moneyIndex, collectedMoney);
            }
            if (collectedLife > 0)
            {
                CreateRewardItem(lifeIndex, collectedLife);
            }
            popupPanel.gameObject.SetActive(true);
            openedLootboxCount = 0;
        }

        private void CreateRewardItem(int index, int count)
        {
            GameObject popupItem = Instantiate(popupRewardItem, Vector3.zero, transform.rotation, popupParent);
            popupItem.transform.localPosition = Vector3.zero;
            popupItem.GetComponent<PopupRewardItem>().rewardIcon.sprite = LootboxRewards[index].icon;
            popupItem.GetComponent<PopupRewardItem>().rewardAmount.text = count.ToString();
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
            totalGold += collectedGold * rewardMultiplier;
            totalEnergy += collectedEnergy * rewardMultiplier;
            totalGem1 += collectedGem1 * rewardMultiplier;
            totalGem2 += collectedGem2 * rewardMultiplier;
            totalGem3 += collectedGem3 * rewardMultiplier;
            totalGem4 += collectedGem4 * rewardMultiplier;
            totalMoney += collectedMoney * rewardMultiplier;
            totalLife += collectedLife * rewardMultiplier;

            rewardMultiplier = 1;   //Reset for next rewards
            confetiEffect.Play();
            StartCoroutine(UpdateRewardsAmount());
            ResetGame();
        }

        private void ResetGame()
        {
            popupPanel.gameObject.SetActive(false);
            selectedRewardIds.Clear();
            collectedGold = 0;
            collectedEnergy = 0;
            collectedGem1 = 0;
            collectedGem2 = 0;
            collectedGem3 = 0;
            collectedGem4 = 0;
            collectedMoney = 0;
            collectedLife = 0;

            foreach (Transform child in popupParent.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < lootboxContainers.Length; i++)
            {
                lootboxContainers[i].parent.GetChild(1).gameObject.SetActive(false);
                Destroy(lootboxContainers[i].transform.GetChild(0).gameObject);
            }

            SpawnLootboxes();
            SetRewardPositions();
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
                totalMoneyText.text = Mathf.Floor(Mathf.Lerp(previousMoney, totalMoney, (elapsedTime / seconds))).ToString();

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
            previousMoney = totalMoney;

            totalGoldText.text = totalGold.ToString();
            totalLifeText.text = totalLife.ToString();
            totalGem1Text.text = totalGem1.ToString();
            totalGem2Text.text = totalGem2.ToString();
            totalGem3Text.text = totalGem3.ToString();
            totalGem4Text.text = totalGem4.ToString();
            totalEnergyText.text = totalEnergy.ToString();
            totalMoneyText.text = totalMoney.ToString();

            SavePlayerProgress(goldString, totalGold);
            SavePlayerProgress(energyString, totalEnergy);
            SavePlayerProgress(gem1String, totalGem1);
            SavePlayerProgress(gem2String, totalGem2);
            SavePlayerProgress(gem3String, totalGem3);
            SavePlayerProgress(gem4String, totalGem4);
            SavePlayerProgress(lifeString, totalLife);
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