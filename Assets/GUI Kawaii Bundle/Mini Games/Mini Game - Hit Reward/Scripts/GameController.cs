using System.Collections;
using UnityEngine.UI;
using UnityEngine;

namespace HitReward
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
            Life,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };
        [System.Serializable]
        public class MultiDimensionalArray
        {
            public RewardEnum rewardCategory;
            public Color backgroundColor;
            public Sprite backgroundSprite;
            [HideInInspector]
            public Sprite rewardIcon;
            public int rewardAmount;
            public Texture confetiIcon;            

        }
        [Header("Customizable Rewards Settings")]
        [Space]
        public MultiDimensionalArray[] Rewards;

        [Header("Game Inputs")]
        [Space]
        public bool useCustomBackgrounds;       //Set true if you want to use package's custom design or set false if you want to use your color selections at Inspector from Rewards array.
        public Sprite[] CustomBackgrounds;
        [System.Serializable]
        public class CategoryIcons
        {
            public RewardEnum category;
            public Sprite rewardIcon;
        }
        public CategoryIcons[] rewardCategoryIcons;   //Category icons used for avoiding drag and drop for every change at Rewards array but if you want you can add a sprite property there and remove categoyIcons.  
        public GameObject piecePrefab;          // One piece for fortune wheel.
        public GameObject knifePrefab,knifeContainer;
        public GameObject wheelParent;
        public int playCost;                    // How much coins user spend to turn the wheel

        [Header("Reward Popup Panel Inputs")]
        [Space]
        public Transform popupPanel;

        [Header("UI Elements")]
        [Space]
        public Button hitButton;
        public Image rewardImagePopup;
        public Text rewardTextPopup;
        public Text turnCostText;
        public Text totalGoldText, totalEnergyText, totalLifeText, totalGem1Text, totalGem2Text, totalGem3Text, totalGem4Text, totalMoneyText;           // Pop-up text with cost or rewarded coins amount

        [Header("Effect Settings")]
        [Space]
        public ParticleSystem confetiEffect;
        public Material confetiCurrency;
        public GameObject WheelTable;
        private Animator animationHit;

        private GameObject knife;
        private bool turnWheel;
        private float turningSpeed;

        private int totalGold, totalEnergy, totalLife, totalGem1, totalGem2, totalGem3, totalGem4, totalMoney;                                          // Started coins amount. In your project it can be set up from PlayerProgress, DataController or from PlayerPrefs 
        private int previousGold, previousEnergy, previousLife, previousGem1, previousGem2, previousGem3, previousGem4, previousMoney;                  // For spent counting animation

        private int rewardIndex;
        private int rewardMultiplier = 1;       //As default it setted to 1. If player selects x2 button and watches rewarded as set this property to 2 to double prize.

        private string goldString = "totalGold";
        private string energyString = "totalEnergy";
        private string lifeString = "totalLife";
        private string moneyString = "totalMoney";
        private string gem1String = "totalGem1";
        private string gem2String = "totalGem2";
        private string gem3String = "totalGem3";
        private string gem4String = "totalGem4";

        private void Awake()
        {
            if (_ins == null)
                _ins = this;

            //UI declerations
            previousGold = totalGold;
            totalGoldText.text = totalGold.ToString();
            turnCostText.text = playCost.ToString();

            animationHit = WheelTable.GetComponent<Animator>();

            GetPlayerProgress();
            CreateWheel();
        }

        public void CreateWheel()
        {
            float startingAngle = 0;

            for (int i = 0; i < Rewards.Length; i++)
            {
                GameObject pieceObj = Instantiate(piecePrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), wheelParent.transform);

                pieceObj.transform.name = "Piece " + (i + 1);
                pieceObj.transform.localPosition = new Vector3(0, 0, 0);
                pieceObj.transform.Rotate(0, 0, Mathf.Abs(startingAngle), 0);
                pieceObj.transform.GetComponent<Piece>().SetValues(i);
                startingAngle += 45;
            }

            StartCoroutine(TurnRoutine());
            CreateKnife();
        }

        private IEnumerator TurnRoutine()
        {
            turnWheel = true;
            turningSpeed = 330;

            while (turnWheel)
            {
                wheelParent.transform.Rotate(0, 0, turningSpeed * Time.deltaTime); //Rotates as turningSpeed degrees per second around z axis

                yield return new WaitForFixedUpdate();
            }
        }

        private void CreateKnife()
        {
            GameObject knifeObject = Instantiate(knifePrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), knifeContainer.transform);
            knifeObject.transform.localPosition = Vector3.zero;
            knife = knifeObject;
        }
        public void HitReward()
        {
            hitButton.interactable = false;
            if (totalGold >= playCost)
            {
                ClaimDropCost();
                knife.GetComponent<Knife>().HitTarget();
            }
            else
            {
                Debug.LogWarning("Player does not have enough gold. Here you should open the shop for in app purchase.");
            }
        }

        private void ClaimDropCost()
        {
            previousGold = totalGold;
            totalGold -= playCost;  // Decrease money for the droping coin
            totalGoldText.text = totalGold.ToString();

        }

        public void PlayHitAnim ()
        {
            animationHit.SetBool("isKnifeHit",true);
        }

        public void SlowDownWheel()
        {
            StartCoroutine(SlowDown());
        }

        private IEnumerator SlowDown()
        {
            while (turningSpeed > 0) 
            {
                turningSpeed -= 40;
                yield return new WaitForSeconds(0.3f);
            }

            Debug.Log("Stopped...");
            turnWheel = false;
            StopCoroutine(TurnRoutine());
            animationHit.SetBool("isKnifeHit", false);
            RewardResults();
        }
        
        private void RewardResults()
        {
            rewardIndex = knife.GetComponentInParent<Piece>().index;
            StartCoroutine(RewardPopup(rewardIndex));

            //If player clicks "Claim", give that reward Call ClaimReward() function OR if player selects "x2 Button", then show rewarded ad and then give double reward Call DoubleReward() function
        }

        IEnumerator RewardPopup(int rewardIndex)
        {
            yield return new WaitForSeconds(0.1f);
            rewardImagePopup.sprite = Rewards[rewardIndex].rewardIcon;
            rewardTextPopup.text = Rewards[rewardIndex].rewardAmount.ToString();
            popupPanel.gameObject.SetActive(true);

            StopCoroutine(TurnRoutine());

            //If player clicks "Claim", give that reward Call ClaimReward() function or if player selects "x2 Button", then show rewarded ad and then give double reward Call DoubleReward() function
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
            confetiEffect.GetComponent<ParticleSystemRenderer>().material.mainTexture = Rewards[rewardIndex].confetiIcon;
            confetiEffect.Play();
            popupPanel.gameObject.SetActive(false);

            switch (Rewards[rewardIndex].rewardCategory)
            {
                case RewardEnum.Gold:
                    {
                        totalGold += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Energy:
                    {
                        totalEnergy += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Life:
                    {
                        totalLife += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem1:
                    {
                        totalGem1 += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem2:
                    {
                        totalGem2 += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem3:
                    {
                        totalGem3 += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem4:
                    {
                        totalGem4 += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Money:
                    {
                        totalMoney += Rewards[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                default:
                    Debug.Log("There is no reward for this angle, please check angles");
                    break;
            }

            rewardMultiplier = 1;           //Reset for next rewards
            hitButton.interactable = true;  //Now player can play again
            StartCoroutine(UpdateRewardAmount());
            CasualSauce.sauce.ReloadScene();
            //ResetGame();
        }

        private void ResetGame()
        {
            popupPanel.gameObject.SetActive(false);
            Destroy(knife.gameObject);
            StartCoroutine(TurnRoutine());
            CreateKnife();
        }

        private IEnumerator UpdateRewardAmount()
        {
            // Animation for increasing and decreasing of currencies amount
            const float seconds = 0.5f;
            float elapsedTime = 0;

            while (elapsedTime < seconds)
            {
                totalGoldText.text = Mathf.Floor(Mathf.Lerp(previousGold, totalGold, (elapsedTime / seconds))).ToString();
                totalLifeText.text = Mathf.Floor(Mathf.Lerp(previousLife, totalLife, (elapsedTime / seconds))).ToString();
                totalEnergyText.text = Mathf.Floor(Mathf.Lerp(previousEnergy, totalEnergy, (elapsedTime / seconds))).ToString();
                totalMoneyText.text = Mathf.Floor(Mathf.Lerp(previousMoney, totalMoney, (elapsedTime / seconds))).ToString();

                totalGem1Text.text = Mathf.Floor(Mathf.Lerp(previousGem1, totalGem1, (elapsedTime / seconds))).ToString();
                totalGem2Text.text = Mathf.Floor(Mathf.Lerp(previousGem2, totalGem2, (elapsedTime / seconds))).ToString();
                totalGem3Text.text = Mathf.Floor(Mathf.Lerp(previousGem3, totalGem3, (elapsedTime / seconds))).ToString();
                totalGem4Text.text = Mathf.Floor(Mathf.Lerp(previousGem4, totalGem4, (elapsedTime / seconds))).ToString();

                elapsedTime += Time.deltaTime;

                yield return new WaitForEndOfFrame();
            }

            previousGold = totalGold;
            previousLife = totalLife;
            previousEnergy = totalEnergy;
            previousMoney = totalMoney;
            previousGem1 = totalGem1;
            previousGem2 = totalGem2;
            previousGem3 = totalGem3;
            previousGem4 = totalGem4;

            totalGoldText.text = totalGold.ToString();
            totalLifeText.text = totalLife.ToString();
            totalEnergyText.text = totalEnergy.ToString();
            totalMoneyText.text = totalMoney.ToString();
            totalGem1Text.text = totalGem1.ToString();
            totalGem2Text.text = totalGem2.ToString();
            totalGem3Text.text = totalGem3.ToString();
            totalGem4Text.text = totalGem4.ToString();

            SavePlayerProgress(goldString, totalGold);
            SavePlayerProgress(energyString, totalEnergy);
            SavePlayerProgress(lifeString, totalLife);
            SavePlayerProgress(moneyString, totalMoney);
            SavePlayerProgress(gem1String, totalGem1);
            SavePlayerProgress(gem2String, totalGem2);
            SavePlayerProgress(gem3String, totalGem3);
            SavePlayerProgress(gem4String, totalGem4);
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
            StartCoroutine(UpdateRewardAmount());
        }


        private void SavePlayerProgress(string st, int value)
        {
            PlayerPrefs.SetInt(st, value);
            PlayerPrefs.Save();
        }
    }
}