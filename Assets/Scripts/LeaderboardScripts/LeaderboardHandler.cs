using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderboardHandler : MonoBehaviour
{
    #region Properties
    [Header("Attributes")]
    [SerializeField] private float positionOffset = 0f;
    [SerializeField] private float nameBoardHeight = 0f;
    [SerializeField] private int rankBelowPlayer = 0;
    [SerializeField] private int rankAbovePlayer = 0;
    [SerializeField] private List<Sprite> flagSprites = new List<Sprite>();

    [Header("Components Reference")]
    [SerializeField] private LeaderBoardScrollEffectHandler leaderBoardScrollEffectHandler = null;
    [SerializeField] private GameObject nameBoardAI = null;
    [SerializeField] private Transform nameBoardHolderTransform = null;
    [SerializeField] private NameboardHandler playerNameboardHandler = null;
    [SerializeField] private RandomNameGenerator randomNameGenerator = null;

    private List<string> fakeUserNames = new List<string>();
    private List<NameboardHandler> nameboardHandlers = new List<NameboardHandler>();
    #endregion

    #region MonoBehaviour Functions
    private void Start()
    {
        //Testing
        ScoreTracker.Instance.PlayerScore = 100;
        playerNameboardHandler.SetupRank(ScoreTracker.Instance.PlayerScore);

        InitialSetup();
    }
    #endregion

    #region Private Core Functions
    private void InitialSetup()
    {
        fakeUserNames = randomNameGenerator.GenerateNames(10);
        SetupLeaderBoard();
    }
    #endregion

    #region Public Core Functions
    public void SetupLeaderBoard()
    {
        foreach (string s in fakeUserNames)
        {
            RectTransform rt = Instantiate(nameBoardAI, Vector3.zero, Quaternion.identity, nameBoardHolderTransform).GetComponent<RectTransform>();
            rt.localPosition = Vector2.zero;
            rt.localPosition = new Vector3(0, positionOffset, 0);
            if (rt.TryGetComponent<NameboardHandler>(out NameboardHandler nameboardHandler))
            {
                nameboardHandler.SetupName(s);
                nameboardHandler.SetupFlag(flagSprites[Random.Range(0, flagSprites.Count)]);
                nameboardHandlers.Add(nameboardHandler);
            }
            positionOffset += nameBoardHeight;
        }

        int index = 0;
        int fakeRank = ScoreTracker.Instance.PlayerScore + rankBelowPlayer;
        for (int i = 0; i < rankBelowPlayer; i++)
        {
            nameboardHandlers[index].SetupRank(fakeRank);
            index++;
            fakeRank--;
        }
        index = nameboardHandlers.Count - 1;
        fakeRank = ScoreTracker.Instance.PlayerScore - rankAbovePlayer - 1;
        for (int i = 0; i < rankAbovePlayer; i++)
        {
            fakeRank++;
            nameboardHandlers[index].SetupRank(fakeRank);
            nameboardHandlers[index].GetComponent<RectTransform>().localPosition = new Vector3(0, nameboardHandlers[index].GetComponent<RectTransform>().localPosition.y + 60, 0);
            index--;
        }
        //Test
        Invoke("InvokeScrollEffect", 2f);
    }
    #endregion

    #region Invoke Functions
    private void InvokeScrollEffect()
    {
        leaderBoardScrollEffectHandler.enabled = true;
        leaderBoardScrollEffectHandler.LeaderBoardScrollOffset -= (rankBelowPlayer * 60);

        foreach(NameboardHandler nh in nameboardHandlers)
        {
            nh.gameObject.GetComponent<AINameBoardTweenHandler>().Tween();
        }
    }
    #endregion
}
