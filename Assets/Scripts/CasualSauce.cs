using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Lofelt.NiceVibrations;

public class CasualSauce : MonoBehaviour
{
    public static CasualSauce sauce;

    [Header("==== BACIS ====")] [Space(15)]
    public int frameRate = 60;

    [Range(0, 5)] public float skyRotSpeed = 2.0f;

    [Header("==== UI ====")] [Space(15)] public Image pointer;
    public GameObject pointHolder;
    public GameObject startUI;
    public GameObject ingameUI;
    public GameObject loaderUI;
    public GameObject winUI;
    public GameObject loseUI;

    [Header("==== WORLD ====")] [Space(15)]
    public int level;
    public Text worldScore;
    public List<Image> lvlImg;
    public Color done, doing, toBeDone;
    public float activeHeight = 130;
    public float nonActiveHeight = 90;
    [Header("==== Appreciate ====")] [Space(15)]
    public Image appreciateImage;

    public Sprite[] appreciateSprites;
    public bool flashOnAppreciate = true;
    public Animator appreciateAnim;
    public bool flashOnMiniAppr = true;
    public Animator apprMiniAnim;
    public GameObject confettiMain, conf1, conf2, conf3, conf4;
    public Animator camAnim;
    public Animator flasherAnim;

    [Header("==== Effects ====")] [Space(15)]
    public bool flashOnSlowMoOn = true;

    public bool flashOnSlowMoOff = true;

    public GameObject miniGames;

    // Game Win - U
    // Game Lose - L
    // Player Gains - O
    // Player Hits - P
    // SlowMo - k

    void Awake()
    {
        Application.targetFrameRate = frameRate;

        if (sauce != null && sauce != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            sauce = this;
        }

        level = PlayerPrefs.GetInt("level", 1);
        loaderUI.SetActive(true);
        startUI.gameObject.GetComponent<Animator>().Play("PanelInv", -1, 0.0f);

        StartCoroutine(StartDelayUI());
        WorldUI();
    }
     public void WorldUI()
    {
        worldScore.text = "World ";
        if (level < 10)
            worldScore.text += "0";
        worldScore.text += level;
        for (int i = 0; i < lvlImg.Count; i++)
        {
            if (i == ((level - 1) % 9))
            {
                lvlImg[i].color = doing;
                lvlImg[i].transform.GetComponent<RectTransform>().sizeDelta =
                    new Vector2(lvlImg[i].transform.GetComponent<RectTransform>().sizeDelta.x, activeHeight);
            }
            else
            {
                if (i < ((level - 1) % 9))
                {
                    Debug.Log("test2");
                    lvlImg[i].color = done;
                    lvlImg[i].transform.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(lvlImg[i].transform.GetComponent<RectTransform>().sizeDelta.x, nonActiveHeight);
                }
                else
                {
                    lvlImg[i].color = toBeDone;
                    lvlImg[i].transform.GetComponent<RectTransform>().sizeDelta =
                        new Vector2(lvlImg[i].transform.GetComponent<RectTransform>().sizeDelta.x, nonActiveHeight);
                }
            }
        }
    }

    IEnumerator StartDelayUI()
    {
        yield return new WaitForSeconds(2.0f);
        loaderUI.SetActive(false);
        startUI.gameObject.GetComponent<Animator>().Play("PanelIn", -1, 0.0f);
    }

    void Update()
    {
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * skyRotSpeed);

        if (Input.GetMouseButton(0))
        {
            pointer.enabled = true;
            pointHolder.gameObject.transform.position = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            pointer.enabled = false;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            WinGame();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            LostGame();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadCurrentScene();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            FlashSlowMo(2.0f, 0.5f);
        }
    }

    public void WinGame() // When player Wins at Finish Line
    {
        AppreciateNow();
    }

    public void AppreciateNow()
    {
        TurnOffConfettis();

        camAnim.Play("covr", -1, 0.0f);

        appreciateImage.sprite = appreciateSprites[Random.Range(0, appreciateSprites.Length)];

        appreciateAnim.gameObject.SetActive(false);
        appreciateAnim.gameObject.SetActive(true);
        appreciateAnim.Play("Appreciate", -1, 0.0f); // Because default animation is played.

        StartCoroutine(PopConfettis());
    }

    IEnumerator PopConfettis()
    {
        if (flashOnAppreciate)
            flasherAnim.Play("Flash", -1, 0.0f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Warning);
        yield return new WaitForSeconds(0.1f);
        confettiMain.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        conf1.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        conf2.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        conf3.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        conf4.SetActive(true);

        yield return new WaitForSeconds(2.0f);
        TurnOffConfettis();

        yield return new WaitForSeconds(2.0f);
        ingameUI.SetActive(false);
        winUI.SetActive(true);
    }

    public void TurnOffConfettis() // useful for multiple playbacks
    {
        confettiMain.SetActive(false);
        conf1.SetActive(false);
        conf2.SetActive(false);
        conf3.SetActive(false);
        conf4.SetActive(false);
    }

    public void LostGame() // When player Lose at Finish Line
    {
        if (flashOnSlowMoOn)
            flasherAnim.Play("FlashHit", -1, 0.0f);
        camAnim.Play("covr", -1, 0.0f);

        StartCoroutine(LostGameEnum());
    }

    IEnumerator LostGameEnum()
    {
        yield return new WaitForSeconds(2.0f);
        ingameUI.SetActive(false);
        loseUI.SetActive(true);
    }

    public void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ReloadCurrentScene()
    {
        if ((level - 1) % 3 == 0)
        {
            winUI.SetActive(false);
            startUI.SetActive(false);
            int ranGame = Random.Range(0, miniGames.transform.childCount);
            miniGames.transform.GetChild(ranGame).gameObject.SetActive(true);
        }
        else
            ReloadScene();
    }

    public void MiniAppreciate() // When player reaches checkpoint or small victory.
    {
        if (flashOnAppreciate)
            flasherAnim.Play("Flash", -1, 0.0f);

        apprMiniAnim.gameObject
            .SetActive(false); // since animations are played on start, simply making sure it's off before it's turned on.
        apprMiniAnim.gameObject.SetActive(true);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);

        StartCoroutine(MiniApprEnum());
    }

    IEnumerator MiniApprEnum()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        yield return new WaitForSeconds(0.1f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);

        yield return new WaitForSeconds(1.0f);
        apprMiniAnim.gameObject.SetActive(false);
    }

    public void CoinPickup()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
    }

    public void PlayerHit() // When the player gets hits by Enemy or Obstacles.
    {
        camAnim.Play("cse", -1, 0.0f);

        StartCoroutine(PlayerHitEnum());
    }

    IEnumerator PlayerHitEnum()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.Selection);
        yield return new WaitForSeconds(0.1f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);
    }

    public void PlayerGains() // When the player gains some Boost or Health or Positive thing.
    {
        camAnim.Play("csm", -1, 0.0f);

        StartCoroutine(PlayerGainsEnum());
    }

    IEnumerator PlayerGainsEnum()
    {
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
        yield return new WaitForSeconds(0.1f);
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.RigidImpact);
    }

    public void FlashSlowMo(float duration, float intensity)
    {
        if (flashOnSlowMoOn)
            flasherAnim.Play("Flash", -1, 0.0f);
        camAnim.Play("cbob", -1, 0.0f);
        Time.timeScale = intensity;

        StartCoroutine(FlashSlowMoEnum(duration));
    }

    IEnumerator FlashSlowMoEnum(float dur)
    {
        yield return new WaitForSecondsRealtime(dur);
        if (flashOnSlowMoOff)
            flasherAnim.Play("Flash", -1, 0.0f);
        camAnim.Play("cbob", -1, 0.0f);
        Time.timeScale = 1.0f;
    }
    
}