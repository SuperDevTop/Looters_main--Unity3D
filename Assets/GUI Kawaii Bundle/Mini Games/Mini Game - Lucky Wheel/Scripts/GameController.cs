using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace FortuneWheel
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

        public enum RewardEnum                  // Your Custom Reward Categories
        {
            None,
            Gold,
            Life,
            Energy,
            Gem1,
            Gem2,
            Gem3,
            Gem4,
            Money
        };

        [System.Serializable]
        public class PieceFeatures
        {
            public RewardEnum rewardCategory;
            public Color backgroundColor;
            public Sprite backgroundSprite;
            [HideInInspector]
            public Sprite rewardIcon;
            public int rewardAmount;
            public int rewardChance;            //Chance to give this slot as result reward if this property is 100 at every slot than every reward has %12.5 change to be selected at 8 sector fortune wheel.
            public Texture confetiIcon;
        }

        [Header("Rewards Custom Settings")]
        [Space]
        public PieceFeatures[] PiecesOfWheel;

        [Header("Game Inputs")]
        [Space]
        public GameObject wheelParent;
        public GameObject piecePrefab;          // One piece for fortune wheel.
        public Sprite[] CustomBackgrounds;
        [System.Serializable]
        public class CategoryIcons
        {
            public RewardEnum category;
            public Sprite rewardIcon;
        }
        public CategoryIcons[] categoryIcons;
        
        public int turnCost;                    // How much coins user spend to turn the wheel
        public bool controlResultReward;
        public bool useCustomBackgrounds;       //Set true if you want to use package's custom design or set false if you want to use your color selections at Inspector from PiecesOfWheel array.

        [Header("Reward Popup Panel Inputs")]
        [Space]
        public Transform popupPanel;

        [Header("UI Elements")]
        [Space]
        public Button turnButton;
        public Image rewardImageHeader;
        public Image rewardImagePopup;
        public Text rewardTextHeader;
        public Text rewardTextPopup;
        public Text turnCostText;
        public Text totalGoldText, totalEnergyText, totalLifeText, totalGem1Text, totalGem2Text, totalGem3Text, totalGem4Text, totalMoneyText;           // Pop-up text with cost or rewarded coins amount

        [Header("Effect Settings")]
        [Space]
        public ParticleSystem confetiEffect;
        public Material confetiCurrency;

        private float[] sectorsAngles = new float[] { -90, -135, -180, -225, -270, -315, -360, -45 };//{ -45, -90, -135, -180, -225, -270, -315, -360 }; // Fill the necessary angles (for example if you want to have 12 sectors you need to fill the angles with 30 degrees step) Here angles for 8 sectors
        private float startAngle = 0;
        private float finalAngle;
        private float currentLerpRotationTime;
        private float maxLerpRotationTime = 5f;

        private int totalGold, totalEnergy, totalLife, totalGem1, totalGem2, totalGem3, totalGem4, totalMoney;                                          // Started coins amount. In your project it can be set up from PlayerProgress, DataController or from PlayerPrefs 
        private int previousGold, previousEnergy, previousLife, previousGem1, previousGem2, previousGem3, previousGem4, previousMoney;                  // For spent counting animation
        private int rewardIndex;
        private int randomChange;               //To use different Easing functions and create diversity for rotations.
        private int rewardMultiplier = 1;       //As default it setted to 1. If player selects x2 button and watches rewarded as set this property to 2 to double prize.
        private int selectedRewardIndex;

        private bool isStarted;
        private bool firstSpin = true;
        private bool rewardSelected;

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
            turnCostText.text = turnCost.ToString();

            GetPlayerProgress();
            CreateWheel();
        }

        public void CreateWheel()
        {
            float startingAngle = 0;

            for (int i = 0; i < PiecesOfWheel.Length; i++)
            {
                GameObject pieceObj = Instantiate(piecePrefab, Vector3.zero, new Quaternion(0, 0, 0, 0), wheelParent.transform);

                pieceObj.transform.name = "Piece " + (i + 1);
                pieceObj.transform.localPosition = new Vector3(0, 0, 0);
                pieceObj.transform.Rotate(0, 0, Mathf.Abs(startingAngle), 0);
                pieceObj.transform.GetComponent<PieceObject>().SetValues(i);
                startingAngle += 45;
            }
        }

        public void TurnWheel()
        {
            turnButton.interactable = false;

            if (totalGold >= turnCost) // If player has enough gold to turn the wheel
            {
                ClaimTurnCost();

                randomChange = Random.Range(0, 3);
                currentLerpRotationTime = 0f;

                #region Wheel Header 
                //Only at first spin you need to fill rewardImage to avoid showing blank image
                if (firstSpin)
                {
                    rewardImageHeader.sprite = PiecesOfWheel[6].rewardIcon; //Stopper starts from 7th piece at this wheel. And its features are stored at 6th place of arrray. Change here if you use another wheel.
                    rewardTextHeader.text = PiecesOfWheel[6].rewardAmount.ToString();

                    rewardImageHeader.transform.gameObject.SetActive(true);
                    rewardTextHeader.transform.gameObject.SetActive(true);

                    firstSpin = false;
                }
                #endregion

                int fullCircles = Random.Range(5, 8);
                float randomFinalAngle;

                if (!controlResultReward)   //Randomly select one angle and give reward at that angle.
                    randomFinalAngle = sectorsAngles[UnityEngine.Random.Range(0, sectorsAngles.Length)];
                else                        //Here final angle selection according to percentages that setted at inspector.
                    randomFinalAngle = sectorsAngles[GetRewardIndex()];

                // Here we set up how many circles our wheel should rotate before stop
                finalAngle = -(fullCircles * 360 - randomFinalAngle);
                isStarted = true;
                StartCoroutine(TurnRoutine());
            }
            else
            {
                Debug.LogWarning("Player does not have enough gold. Here you should open the shop for in app purchase.");
            }
        }

        public int GetRewardIndex() //Here final angle selection according to percentages that setted at inspector. Here there is a still change factor but random selection effect is minimized.
        {
            int randomValue = Random.Range(0, 100);

            for (int i = 0; i < PiecesOfWheel.Length; i++)
            {
                if (randomValue <= PiecesOfWheel[i].rewardChance && !rewardSelected)
                {
                    rewardSelected = true;
                    selectedRewardIndex = i;
                }
            }

            rewardSelected = false;
            return selectedRewardIndex;
        }

        private void ClaimTurnCost()
        {
            previousGold = totalGold;   //Set prev value for flipping uı animation
            totalGold -= turnCost;      // Decrease cost for the turn
            StartCoroutine(UpdateRewardAmount());
        }

        private IEnumerator TurnRoutine()
        {
            while (isStarted)
            {
                float t = currentLerpRotationTime / maxLerpRotationTime;

                if (randomChange == 0)
                    t = 1f - (1f - t) * (1f - t);
                else
                    t = t * t * t * (t * (6f * t - 15f) + 10f);

                float angle = Mathf.Lerp(startAngle, finalAngle, t); //Linear Interpolation
                wheelParent.transform.eulerAngles = new Vector3(0, 0, angle);

                // Increment timer once per frame
                currentLerpRotationTime += Time.deltaTime;

                if (currentLerpRotationTime > maxLerpRotationTime || wheelParent.transform.eulerAngles.z == finalAngle)
                {
                    currentLerpRotationTime = maxLerpRotationTime;
                    isStarted = false;
                    startAngle = finalAngle % 360;

                    GiveAwardByAngle();
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void GiveAwardByAngle()
        {
            // Here you can set up rewards for every sector of wheel
            switch ((int)startAngle)
            {
                case 0:
                    rewardIndex = 6;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -45:
                    rewardIndex = 7;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -90:
                    rewardIndex = 0;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -135:
                    rewardIndex = 1;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -180:
                    rewardIndex = 2;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -225:
                    rewardIndex = 3;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -270:
                    rewardIndex = 4;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -315:
                    rewardIndex = 5;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                case -360:
                    rewardIndex = 6;
                    StartCoroutine(RewardPopup(rewardIndex));
                    break;
                default:
                    Debug.Log("There is no reward for this angle, please check angles");
                    break;
            }
        }

        IEnumerator RewardPopup(int rewardIndex)
        {
            yield return new WaitForSeconds(0.1f);
            rewardImagePopup.sprite = PiecesOfWheel[rewardIndex].rewardIcon;
            rewardTextPopup.text = PiecesOfWheel[rewardIndex].rewardAmount.ToString();
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
            confetiEffect.GetComponent<ParticleSystemRenderer>().material.mainTexture = PiecesOfWheel[rewardIndex].confetiIcon;
            confetiEffect.Play();
            popupPanel.gameObject.SetActive(false);

            switch (PiecesOfWheel[rewardIndex].rewardCategory)
            {
                case RewardEnum.Gold:
                    {
                        totalGold += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Energy:
                    {
                        totalEnergy += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Life:
                    {
                        totalLife += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem1:
                    {
                        totalGem1 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem2:
                    {
                        totalGem2 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem3:
                    {
                        totalGem3 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Gem4:
                    {
                        totalGem4 += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                case RewardEnum.Money:
                    {
                        totalMoney += PiecesOfWheel[rewardIndex].rewardAmount * rewardMultiplier;
                    }
                    break;
                default:
                    Debug.Log("There is no reward for this angle, please check angles");
                    break;
            }
            CasualSauce.sauce.ReloadScene();
            rewardMultiplier = 1;   //Reset for next rewards
            turnButton.interactable = true; //Now player can turn wheel again
            StartCoroutine(UpdateRewardAmount());
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