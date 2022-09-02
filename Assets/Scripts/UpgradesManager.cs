using System.Collections;
using System.Collections.Generic;
using Shapes2D;
using UnityEngine;
using UnityEngine.UI;

public class UpgradesManager : MonoBehaviour
{
    [Space(10)] [Header("Gold")] public int coinsInWallet;
    public Text coinsText;

    [Space(10)] [Header("Gem")] public int gemsInWallet;
    public Text gemsText;

    public Animator powerAnim, speedAnim, coinsAnim;

    [Space(10)] [Header("Basics")] public Animator csoonAnim;
    public bool pwrPossible, spdPossible, coinsPossible;

    [Space(10)] [Header("Unavailable Button")]
    public bool useless;

    public Color f1_Unavl, f2_Unavl, shad_Unavl, out_Unavl, cb_Unavl;

    [Space(10)] [Header("Power Button")] public GameObject powPart;
    public int pwrInitCost, pwrLevel, pwrMulti;
    public Shapes2D.Shape powerShape, powerCbShape;
    public Color f1_power, f2_power, shad_power, out_Power, cb_power;
    public Image powCoinImg, powVidImg, powRayImg;
    public Text powCoinText, powVidText, pwrLvlTxt;

    [Space(10)] [Header("Speed Button")] public GameObject spdPart;
    public int spdInitCost, spdLevel, spdMulti;
    public Shapes2D.Shape speedShape, speedCbShape;
    public Color f1_Speed, f2_Speed, shad_Speed, out_Speed, cb_Speed;
    public Image spdCoinImg, spdVidImg, spdRayImg;
    public Text spdCoinText, spdVidText, spdLvlTxt;

    [Space(10)] [Header("Coins Button")] public GameObject coinPart;
    public int coinsInitCost, coinsLevel, coinsMulti;
    public Shapes2D.Shape coinShape, coinCbShape;
    public Color f1_Coin, f2_Coin, shad_Coin, out_Coin, cb_Coin;
    public Image coinCoinImg, coinVidImg, coinRayImg;
    public Text coinCoinText, coinVidText, coinsLvlTxt;

    void Awake()
    {
        //coins
        coinsInWallet = PlayerPrefs.GetInt("totalGold", 0);
        coinsText.text = "$" + coinsInWallet.ToString("");

        //gems
        gemsInWallet = PlayerPrefs.GetInt("totalGem4", 0);
        gemsText.text = gemsInWallet.ToString("");

        //power
        pwrLevel = PlayerPrefs.GetInt("PowerLevel", 1);
        pwrLvlTxt.text = "Lvl" + pwrLevel.ToString("");

        pwrInitCost = PlayerPrefs.GetInt("PowerValue", 100);
        powCoinText.text = "$" + pwrInitCost.ToString("");


        //speed
        spdLevel = PlayerPrefs.GetInt("SpeedLevel", 1);
        spdLvlTxt.text = "Lvl" + spdLevel.ToString("");

        spdInitCost = PlayerPrefs.GetInt("SpeedValue", 100);
        spdCoinText.text = "$" + spdInitCost.ToString("");


        //coinMulti
        coinsLevel = PlayerPrefs.GetInt("CoinsLevel", 1);
        coinsLvlTxt.text = "Lvl" + coinsLevel.ToString("");

        coinsInitCost = PlayerPrefs.GetInt("CoinsValue", 100);
        coinCoinText.text = "$" + coinsInitCost.ToString("");
    }

    void Start()
    {
        CheckUpgradesAvailability();
    }

    public void IncreaseCoin(int coins)
    {
        coinsInWallet += Mathf.CeilToInt(coins * ((coinsLevel * 0.1f) + 1));
        PlayerPrefs.SetInt("totalGold", coinsInWallet);
        coinsText.text = "$" + coinsInWallet.ToString("");
        PlayerPrefs.Save();
    }

    private void CheckCoin()
    {
        if (coinCoinText.IsActive())
        {
            int coinCost = int.Parse(coinCoinText.text.Substring(1));
            if (coinCost >= coinsInWallet)
            {
                coinShape.GetComponent<Button>().interactable = false;
                coinShape.settings.fillColor = f1_Unavl;
                coinShape.settings.fillColor2 = f2_Unavl;
                coinCbShape.settings.fillColor = cb_Unavl;
            }
            else
            {
                coinShape.GetComponent<Button>().interactable = true;
                coinShape.settings.fillColor = f1_Coin;
                coinShape.settings.fillColor2 = f2_Coin;
                coinCbShape.settings.fillColor = cb_Coin;
            }
        }
    }

    private void CheckSpeed()
    {
        if (spdCoinText.IsActive())
        {
            int speedCost = int.Parse(spdCoinText.text.Substring(1));
            if (speedCost >= coinsInWallet)
            {
                speedShape.GetComponent<Button>().interactable = false;
                speedShape.settings.fillColor = f1_Unavl;
                speedShape.settings.fillColor2 = f2_Unavl;
                speedCbShape.settings.fillColor = cb_Unavl;
            }
            else
            {
                speedShape.GetComponent<Button>().interactable = true;
                speedShape.settings.fillColor = f1_Speed;
                speedShape.settings.fillColor2 = f2_Speed;
                speedCbShape.settings.fillColor = cb_Speed;
            }
        }
    }

    private void CheckPower()
    {
        if (powCoinText.IsActive())
        {
            int powerCost = int.Parse(powCoinText.text.Substring(1));
            if (powerCost >= coinsInWallet)
            {
                powerShape.GetComponent<Button>().interactable = false;
                powerShape.settings.fillColor = f1_Unavl;
                powerShape.settings.fillColor2 = f2_Unavl;
                powerCbShape.settings.fillColor = cb_Unavl;
            }
            else
            {
                powerShape.GetComponent<Button>().interactable = true;
                powerShape.settings.fillColor = f1_power;
                powerShape.settings.fillColor2 = f2_power;
                powerCbShape.settings.fillColor = cb_power;
            }
        }
    }

    public bool CheckUpgrade(Text text)
    {
        int cost = int.Parse(text.text.Substring(1));
        coinsInWallet = PlayerPrefs.GetInt("Coins", 0);
        if (coinsInWallet >= cost)
            return true;
        else return false;
    }

    // Power Upgrade

    public void UpgradePower()
    {
        if (CheckUpgrade(powCoinText))
        {
            //power to be modified
            
            pwrLevel += 1;
            PlayerPrefs.SetInt("PowerLevel", pwrLevel);
            pwrLvlTxt.text = "Lvl" + pwrLevel.ToString("");
            
            pwrInitCost *= pwrMulti;
            PlayerPrefs.SetInt("PowerValue", pwrInitCost);
            powCoinText.text = "$" + pwrInitCost.ToString("");
        }
        else
        {
            csoonAnim.gameObject.SetActive(false);
            csoonAnim.gameObject.SetActive(true);
            powerAnim.Play("UpgErr", -1, 0.0f);
        }
    }

    // Speed Upgrade

    public void UpgradeSpeed()
    {
        if (CheckUpgrade(spdCoinText))
        {
            //speed to be modified
            
            spdLevel += 1;
            PlayerPrefs.SetInt("SpeedLevel", spdLevel);
            spdLvlTxt.text = "Lvl" + spdLevel.ToString("");
            
            spdInitCost *= spdMulti;
            PlayerPrefs.SetInt("SpeedValue", spdInitCost);
            spdCoinText.text = "$" + spdInitCost.ToString("");
        }
        else
        {
            csoonAnim.gameObject.SetActive(false);
            csoonAnim.gameObject.SetActive(true);
            speedAnim.Play("UpgErr", -1, 0.0f);
        }
    }

    // Coins Upgrade
    public void UpgradeCoins()
    {
        if (CheckUpgrade(coinCoinText))
        {
            //coin to be modified

            coinsLevel += 1;
            PlayerPrefs.SetInt("CoinsLevel", coinsLevel);
            coinsLvlTxt.text = "Lvl" + coinsLevel.ToString("");
            
            coinsInitCost *= coinsMulti;
            PlayerPrefs.SetInt("CoinsValue", coinsInitCost);
            coinCoinText.text = "$" + coinsInitCost.ToString("");
        }
        else
        {
            csoonAnim.gameObject.SetActive(false);
            csoonAnim.gameObject.SetActive(true);
            coinsAnim.Play("UpgErr", -1, 0.0f);
        }
    }

    public void CheckUpgradesAvailability()
    {
        CheckPower();
        CheckSpeed();
        CheckCoin();
    }
}