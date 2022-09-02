
using System.Collections.Generic;
using UnityEngine;

public enum Platform
{
    noPlatform = -1,
    CoinPlatform = 0,
    SafezonePlatform = 1,
    StayareaPlatform = 2
}

public class GameManager : MonoBehaviour
{
    #region variables
    public GameObject[] PLAYERS;
    //public GameObject[] CHARACTORS;
    public GameObject[] COINS;
    public GameObject[] SAFE_ZONES;
    public GameObject[] PLATFORMS;
    public GameObject[] FALL_POSITIONS;

    public int[] PRICE_ARRAY;
    string _log = "";
    int _lvl = 1;
    public int LEVEL;

    public GUIStyle style;
    public string BOT_NAME_1 = "Ai_1";
    public string BOT_NAME_2 = "Ai_2";
    public string BOT_NAME_3 = "Ai_3";

    const string BOT1 = "Enemy1";
    const string BOT2 = "Enemy2";
    const string BOT3 = "Enemy3";
    const string USER = "Player";

    const string COIN_TAG = "Coin";
    const string SAFEZONE_TAG = "Safezone";
    const string PLATFORM_TAG = "Platform";

    GameObject[] _players;
    Animation[] _chars;

    GameObject[] _safezones;
    GameObject[] _stayAreas;
    int[] _order;
    List<Vector3> _abs_stay_positions;
    List<Vector3> _abs_coin_positions;
    List<Vector3> _abs_safe_positions;

    List<int> _allMoneyArr;
    List<int> _moneyArr;

    bool _exit = false;
    int _signals = 0;
    int _round = 1;
    int _round_limit = 5;
    bool _debug = false;
    bool _isBack;
    bool _isEnable;
    bool _isLog;
    float _time;
    GameObject[] _froms;
    GameObject[] _tos;
    List<int[]> _lootersArr;
    bool[] _isBumps;
    string[] _animations;
    int _bestIndex;
    float _flyHigh;
    float _flySpeed = 1.0f;
    float _bumpersDistance = 2.5f;
    float _limitDeep = -50.0f;
    #endregion

    private void Init(int level)
    {
        _round = 1;
        _lvl = level;
        _abs_stay_positions = new List<Vector3>();
        _abs_coin_positions = new List<Vector3>();
        _abs_safe_positions = new List<Vector3>();
        _froms = new GameObject[level + 1];
        _tos = new GameObject[level + 1];
        _animations = new string[level + 1];

        _isBumps = new bool[level + 1];
        for (int i = 0; i < level + 1; i++)
            _isBumps[i] = false;
        _lootersArr = new List<int[]>();
        _signals = 0;
        _bestIndex = -1;
        _time = 0;
        _isBack = false;

        _players = new GameObject[level + 1];
        _chars = new Animation[level + 1];
        _safezones = new GameObject[level + 1];
        _stayAreas = new GameObject[level + 1];
        switch (level)
        {
            case 1:
                _players[0] = PLAYERS[1];
                _players[1] = PLAYERS[3];
                _chars[0] = _players[0].GetComponent<PlayerAction>().GetAnimation();
                _chars[1] = _players[1].GetComponent<PlayerAction>().GetAnimation();
                _safezones[0] = SAFE_ZONES[1];
                _safezones[1] = SAFE_ZONES[3];
                _stayAreas[0] = PLATFORMS[1];
                _stayAreas[1] = PLATFORMS[3];
                RemoveGameObject(PLAYERS[0]);
                RemoveGameObject(PLAYERS[2]);
                RemoveGameObject(PLATFORMS[0]);
                RemoveGameObject(PLATFORMS[2]);
                RemoveGameObject(SAFE_ZONES[0]);
                RemoveGameObject(SAFE_ZONES[2]);
                // RemoveGameObject("safe_zone1");
                // RemoveGameObject("safe_zone3");
                break;
            case 2:
                _players[0] = PLAYERS[0];
                _players[1] = PLAYERS[2];
                _players[2] = PLAYERS[3];
                _chars[0] = _players[0].GetComponent<PlayerAction>().GetAnimation();
                _chars[1] = _players[1].GetComponent<PlayerAction>().GetAnimation();
                _chars[2] = _players[2].GetComponent<PlayerAction>().GetAnimation();
                _safezones[0] = SAFE_ZONES[0];
                _safezones[1] = SAFE_ZONES[2];
                _safezones[2] = SAFE_ZONES[3];
                _stayAreas[0] = PLATFORMS[0];
                _stayAreas[1] = PLATFORMS[2];
                _stayAreas[2] = PLATFORMS[3];

                RemoveGameObject(PLAYERS[1]);
                RemoveGameObject(PLATFORMS[1]);
                RemoveGameObject(SAFE_ZONES[1]);
                // RemoveGameObject("safe_zone2");
                break;
            default:
                _players[0] = PLAYERS[0];
                _players[1] = PLAYERS[1];
                _players[2] = PLAYERS[2];
                _players[3] = PLAYERS[3];

                _chars[0] = _players[0].GetComponent<PlayerAction>().GetAnimation();
                _chars[1] = _players[1].GetComponent<PlayerAction>().GetAnimation();
                _chars[2] = _players[2].GetComponent<PlayerAction>().GetAnimation();
                _chars[3] = _players[3].GetComponent<PlayerAction>().GetAnimation();

                _safezones[0] = SAFE_ZONES[0];
                _safezones[1] = SAFE_ZONES[1];
                _safezones[2] = SAFE_ZONES[2];
                _safezones[3] = SAFE_ZONES[3];

                _stayAreas[0] = PLATFORMS[0];
                _stayAreas[1] = PLATFORMS[1];
                _stayAreas[2] = PLATFORMS[2];
                _stayAreas[3] = PLATFORMS[3]; 
                break;
        }
         
        for(int i = 0; i <= level; i++)
        {
            GameObject player = _players[i];
            Transform trans = player.transform;
            _abs_stay_positions.Add(trans.position);
            player.GetComponent<PlayerAction>().DisplayName(GetBotName(i));
        }
        for (int i = 0; i < 4; i++)
        {
            GameObject coin = COINS[i];
            Transform trans = coin.transform;
            Vector3 pos = trans.position;
            //TODO: high value must equals initial pos.y
            pos.y = _abs_stay_positions[0].y;
            _abs_coin_positions.Add(pos);
        }
        for (int i = 0; i <= level; i++)
        {
            GameObject safezone = _safezones[i];
            Transform trans = safezone.transform;
            Vector3 pos = trans.position;
            //TODO: high value must equals initial pos.y
            pos.y = _abs_stay_positions[0].y;
            _abs_safe_positions.Add(pos);
        }
        SetCoinLooters();//init 
        CheckScore();
    }

    private void Start()
    {
        //TODO: set value fly Speed and fly Highest value of charactors
        //TODO: seperate distance when is collided,
        //TODO: limit deep when is falled in water
        _flySpeed = 1.0f;
        _flyHigh = 6.0f;
        //_collidersDistance = 1.50f;
        _limitDeep = -50.0f;
        _isEnable = true;
        try
        {
            LEVEL = PlayerPrefs.GetInt("level");
        }
        catch { LEVEL = 1; }
        _lvl = LEVEL;
        if (LEVEL == 0)
        {
            LEVEL = 1;
            _lvl = 1;
        }
        if (LEVEL > 3)
            _lvl = 3;

        Init(_lvl);
    }

    #region Init Get 
    public float GetFlySpeed()
    {
        return _flySpeed;
    }

    public float GetFlyHigh()
    {
        return _flyHigh;
    }

    public string GetCoinTag()
    {
        return COIN_TAG;
    }

    public float GetBumpersDistance()
    {
        return _bumpersDistance;
    }

    public float GetLimitDeep()
    {
        return _limitDeep;
    }

    public int[] GetPrices()
    {
        return PRICE_ARRAY;
    }

    #endregion

    public string GetBotName(int n)
    {
        switch (n)
        {
            case 0: return BOT_NAME_1;
            case 1: return BOT_NAME_2;
            case 2: return BOT_NAME_3;
            default: return "YOU";
        }
    }
    public string GetPlayerName(int n)
    {
        switch (n)
        {
            case 1: return BOT1;
            case 2: return BOT2;
            case 3: return BOT3;
            case 4: return USER;
            default: return null;
        }
    }

    // protected void RemoveGameObject(string what)
    // {
    //     RemoveGameObject(GameObject.Find(what));
    // }

    protected void RemoveGameObject(GameObject what)
    {
        DestroyImmediate(what);
    }

    public void SetEnable(bool enable)
    {
        _isEnable = enable;
        if (_isEnable)
        {
            CheckScore();
            _isLog = true;
        }
        else
        {
            _isLog = false;
        }
    }

    protected int GetRandomInt(int min, int max)
    {
        return Random.Range(min, max);
    }

    protected int[] GetRandomFourInts(int min, int max)
    {
        int[] ns = new int[4];
        ns[0] = GetRandomInt(min, max);
        while (true)
        {
            int n = GetRandomInt(min, max);
            if (n != ns[0])
            {
                ns[1] = n;
                break;
            }
        }
        while (true)
        {
            int n = GetRandomInt(min, max);
            if (n != ns[0] && n != ns[1])
            {
                ns[2] = n;
                break;
            }
        }
        while (true)
        {
            int n = GetRandomInt(min, max);
            if (n != ns[0] && n != ns[1] && n != ns[2])
            {
                ns[3] = n;
                break;
            }
        }
        return ns;
    }

    protected void SetCoinLooters()
    {
        COINS[0].GetComponent<CoinBase>().SetLooters(new int[4] { 1, 0, 0, 0 });
        COINS[1].GetComponent<CoinBase>().SetLooters(new int[4] { 0, 1, 0, 0 });
        COINS[2].GetComponent<CoinBase>().SetLooters(new int[4] { 0, 0, 1, 0 });
        COINS[3].GetComponent<CoinBase>().SetLooters(new int[4] { 0, 0, 0, 1 });

        int[] rs = GetRandomFourInts(1, 5);
        for (int i = 1; i <= 4; i++)
        {
            int r = rs[i - 1];
            Vector3 pos = _abs_coin_positions[r - 1];
            pos.y = 0.56f;

            COINS[i - 1].transform.position = pos;
        }
    }

    public int Count()
    {
        _round++;
        if (_round > _round_limit)
        {
            _round = _round_limit;
            _exit = true;
        }
        return _round;
    }

    #region Coin Move
    public List<GameObject> GetChildrens(GameObject parent)
    {
        List<GameObject> objs = new List<GameObject>();
        int cnt = parent.transform.childCount;
        for (int i = 0; i < cnt; i++)
        {
            Transform tf = parent.transform.GetChild(i);
            GameObject obj = tf.gameObject;
            objs.Add(obj);
        }
        return objs;
    }

    public void ChangePosStealed(GameObject who)
    {
        List<GameObject> childs = GetChildrens(who);
        int cnt = childs.Count;
        for (int i = 0; i < cnt; i++)
        {
            GameObject child = childs[i];
            Vector3 v3 = child.transform.position;
            //TODO: x, z is changed little
            v3.x = v3.x + UnityEngine.Random.Range(-0.2f, 0.2f);
            v3.z = v3.z + UnityEngine.Random.Range(-0.2f, 0.2f);
            v3.y = who.GetComponent<PlayerAction>().LOOTER_POS.position.y;
            child.transform.position = v3;
        }
    }

    public void ChangeParent_Stayarea2PlayerForMove(GameObject stay, GameObject who)
    {
        List<GameObject> childs = GetChildrens(stay);
        int cnt = childs.Count;
        for (int i = 0; i < cnt; i++)
        {
            GameObject child = childs[i];
            if (child.CompareTag(COIN_TAG))
            {
                Vector3 v3 = child.transform.position;
                //TODO: x, z is changed little
                v3.x = child.transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f);
                v3.z = child.transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f);
                child.transform.position = v3;
                child.transform.SetParent(who.transform, true);
            }
        }
    }

    public void ChangeParent_Stayarea2PlayerForSteal(GameObject stay, GameObject who)
    {
        List<GameObject> childs = GetChildrens(stay);
        int cnt = childs.Count;
        for (int i = 0; i < cnt; i++)
        {
            GameObject child = childs[i];
            if (child.CompareTag(COIN_TAG))
            {
                Vector3 v3 = child.transform.position;
                //TODO: x, z is changed little
                v3.x = v3.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                v3.z = v3.z + UnityEngine.Random.Range(-0.2f, 0.2f);
                child.transform.position = v3;
                child.transform.SetParent(who.transform, true);
            }
        }
    }

    public void ChangeParent_Player2Stayarea(GameObject who, GameObject to)
    {
        List<GameObject> childs = GetChildrens(who);
        int cnt = childs.Count;
        if (cnt > 1)
        {
            for (int i = 0; i < cnt; i++)
            {
                GameObject child = childs[i];
                if (child.CompareTag(COIN_TAG))
                {
                    Vector3 v3 = child.transform.position;
                    //TODO: x, z is changed little
                    switch (who.name)
                    {
                        case BOT1:
                            v3.x = child.transform.position.x + UnityEngine.Random.Range(0.3f, 1.0f);
                            v3.z = child.transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f);
                            break;
                        case BOT2:
                            v3.x = child.transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f);
                            v3.z = child.transform.position.z + UnityEngine.Random.Range(-0.3f, 0.7f);
                            break;
                        case BOT3:
                            v3.x = child.transform.position.x + UnityEngine.Random.Range(-0.3f, 0.7f);
                            v3.z = child.transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f);
                            break;
                        case USER:
                            v3.x = child.transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f);
                            v3.z = child.transform.position.z + UnityEngine.Random.Range(-1.0f, -0.3f);
                            break;
                    }

                    //TODO: for look little smaller than origin

                    v3.y = 0.56f;
                    child.transform.position = v3;
                    child.transform.SetParent(to.transform, true);
                }
            }
        }
    }

    public void ChangeParent_Player2Safezone(GameObject player, GameObject safe, string compareTag)
    {
        List<GameObject> childs = GetChildrens(player);
        int cnt = childs.Count;
        if (cnt > 1)
        {
            for (int i = 0; i < cnt; i++)
            {
                GameObject child = childs[i];
                if (child.CompareTag(compareTag))
                {
                    Vector3 v3 = safe.transform.position;
                    //TODO: x, z is changed little
                    v3.x = safe.transform.position.x + UnityEngine.Random.Range(-0.5f, 0.5f);
                    v3.z = safe.transform.position.z + UnityEngine.Random.Range(-0.5f, 0.5f);
                    //v3.y = to_.transform.position.y;
                    child.transform.position = v3;
                    child.transform.SetParent(safe.transform, true);
                }
            }
        }
    }

    #endregion

    #region animation
    public void AnimationPlay(GameObject who, string type)
    {
        Animation anim = GetAnimObjByName(who);
        if (anim.name != type)
            anim.Play("Armature|" + type);
        else
        {
            if (!anim.isPlaying)
                anim.Play("Armature|" + type);
        }
    }

    public Animation GetAnimObjByName(GameObject self)
    {
        foreach (GameObject player in _players)
        {
            if (player == self)
                return _chars[GetPlayerIndex(player)];
        }
        return null;
    }

    #endregion

    public int Looters2Money(int[] looters)
    {
        int res = 0;
        for (int i = 0; i < 4; i++)
        {
            int cnt = looters[i];
            res += cnt * PRICE_ARRAY[i];
        }
        return res;
    }

    #region Get GameObject each other
    protected int GetPlayerIndex(GameObject player)
    {
        for (int i = 0; i <= _lvl; i++)
        {
            GameObject p = _players[i];
            if (p == player)
                return i;
        }
        return -1;
    }

    protected int GetSafeZoneIndex(GameObject safezone)
    {
        for (int i = 0; i <= _lvl; i++)
        {
            GameObject sz = _safezones[i];
            if (sz == safezone)
                return i;
        }
        return -1;
    }

    public int GetStayAreaIndex(GameObject stayArea)
    { 
        for (int i = 0; i <= _lvl; i++)
        {
            GameObject sa = _stayAreas[i];
            if (sa == stayArea) return i;    
        }

        return -1;
    }

    public GameObject Player2Stayarea(GameObject player)
    {
        int index = GetPlayerIndex(player);
        return _stayAreas[index];
    }

    public GameObject Stayarea2Player(GameObject stayArea)
    {
        int index = GetStayAreaIndex(stayArea);
        return _players[index];
    }

    public GameObject Stayarea2Safezone(GameObject stay)
    {
        int index = GetStayAreaIndex(stay);
        return _safezones[index];
    }

    protected bool IsSelfStayarea(GameObject who, GameObject stayarea)
    {
        return stayarea == this.Player2Stayarea(who);
    }

    #endregion

    public Platform GetPlatfrom(GameObject platform)
    {
        if (platform.CompareTag(COIN_TAG))
            return Platform.CoinPlatform;
        else if (platform.CompareTag(PLATFORM_TAG))
            return Platform.StayareaPlatform;
        else if (platform.CompareTag(SAFEZONE_TAG))
            return Platform.SafezonePlatform;
        else return Platform.noPlatform;
    }

    public void OnArrivedStayareaPlatform(GameObject who)
    {
        GameObject stayArea = this.Player2Stayarea(who);
        this.ChangeParent_Player2Stayarea(who, stayArea);
    }

    protected void Run2Base()
    {
        if (!_isBack) return;
        _time += Time.deltaTime;
        for (int i = 0; i <= _lvl; i++)
        {
            GameObject who_ = _players[i];
            string animation = _animations[i];
            if (i == _bestIndex) animation = "Win4";
            AnimationPlay(who_, animation);
        }
        if (_time > 3.0f)
        {
            for (int i = 0; i <= _lvl; i++)
            {
                GameObject from_ = _froms[i];
                GameObject to_ = _tos[i];
                GameObject who_ = _players[i];
                bool isBump_ = _isBumps[i];
                int[] looters_ = _lootersArr[i];
                Platform platform_ = GetPlatfrom(to_);
                AnimationPlay(who_, "JumpFalling");
                if (isBump_ || platform_ != Platform.CoinPlatform)
                    looters_ = new int[4] { 0, 0, 0, 0 };

                switch (_lvl)
                {
                    case 3:
                        switch (who_.name)
                        {
                            case BOT1:
                                _players[0].transform.localEulerAngles = new Vector3(0, 180, 0);
                                break;
                            case BOT2:
                                _players[1].transform.localEulerAngles = new Vector3(0, 90, 0);
                                break;
                            case BOT3:
                                _players[2].transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                            case USER:
                                _players[3].transform.localEulerAngles = new Vector3(0, -90, 0);
                                break;
                        }
                        break;

                    case 2:
                        switch (who_.name)
                        {
                            case BOT1:
                                _players[0].transform.localEulerAngles = new Vector3(0, 180, 0);
                                break;
                            case BOT3:
                                _players[1].transform.localEulerAngles = new Vector3(0, 90, 0);
                                break;
                            case USER:
                                _players[2].transform.localEulerAngles = new Vector3(0, 0, 0);
                                break;
                        }
                        break;

                    case 1:
                        switch (who_.name)
                        {
                            case BOT2:
                                _players[0].transform.localEulerAngles = new Vector3(0, 180, 0);
                                break;
                            case USER:
                                _players[1].transform.localEulerAngles = new Vector3(0, 90, 0);
                                break;
                        }
                        break;
                }
                if (!isBump_)
                    who_.GetComponent<PlayerAction>().JumpForBack(to_, from_, looters_);
                else
                {
                    if (to_ != from_)
                        who_.GetComponent<PlayerAction>().FallForBack(to_, from_);
                    else
                        who_.GetComponent<PlayerAction>().JumpForBack(to_, from_, looters_);
                }
            }
            SetCoinLooters();

            _isBumps = new bool[_lvl + 1];
            for (int i = 0; i <= _lvl; i++)
                _isBumps[i] = false;
            _lootersArr = new List<int[]>();
            _signals = 0;
            _bestIndex = -1;
            _time = 0;
            _isBack = false;
            this.Count();
            //if (_exit) return;
        }

    }

    private void FixedUpdate()
    {
        Run2Base();
    }

    public void OnArrivedTargetPlatform(GameObject who, GameObject stayarea, GameObject to, bool isBump)
    {
        AnimationPlay(who, "JumpLanding");
        //_log = who.name + ">" + to.name + "(" + isCollid + ")\n";
        int[] looters = new int[4] { 0, 0, 0, 0 };
        bool forSafe = to == stayarea;
        //print(to + "," + stayarea);
        if (forSafe)
        {
            stayarea.GetComponent<StayBase>().SetSafeLooters();
            //this.ChangeParent_Player2Safezone(to, who, GetCoinTag());
            stayarea.GetComponent<StayBase>().AddLooters(looters, false);
        }
        else
        {
            stayarea.GetComponent<StayBase>().SetSafeLooterHistory();
        }

        Platform platform = GetPlatfrom(to);
        if (platform == Platform.StayareaPlatform && !IsSelfStayarea(who, to) && !isBump)
        {
            looters = to.GetComponent<StayBase>().GetLooters();
            to.GetComponent<StayBase>().StealedByOther(who);
            ChangeParent_Stayarea2PlayerForSteal(to, who);
            stayarea.GetComponent<StayBase>().AddLooters(looters, true);
        }

        _signals++;
        int index = GetPlayerIndex(who);
        _froms[index] = stayarea;
        _tos[index] = to;
        _isBumps[index] = isBump;

        if (_signals != _lvl + 1) return;
        int max = -1;
        _lootersArr = new List<int[]>();
        for (int i = 0; i <= _lvl; i++)
        {
            GameObject to_ = _tos[i];
            GameObject from_ = _froms[i];
            GameObject who_ = _players[i];
            bool isBump_ = _isBumps[i];
            platform = GetPlatfrom(to_);
            looters = new int[4] { 0, 0, 0, 0 };

            if (!isBump_)
            {
                switch (platform)
                {
                    case Platform.CoinPlatform:
                        looters = to_.GetComponent<CoinBase>().GetLooters();
                        from_.GetComponent<StayBase>().AddLooters(looters, false);
                        break;
                    default: break;
                }
            }
            else from_.GetComponent<StayBase>().AddLooters(looters, false);
            _lootersArr.Add(looters);
            if (GetPlatfrom(to_) == Platform.StayareaPlatform && !IsSelfStayarea(who_, to_))
                looters = from_.GetComponent<StayBase>().GetStealedLooters();
            int money = Looters2Money(looters);
            if (money > max) max = money;
            string animation = string.Empty;
            if (isBump_) animation = "Fight" + (i + 1);
            else
            {
                //TODO: set animation as got looters
                if (money == 0)
                {
                    if (to_ == from_) animation = "PushingFast";
                    else animation = "Lose3";
                }
                else if (money > 0 && money < PRICE_ARRAY[1] ) animation = "HappyIdle";
                else if (money >= PRICE_ARRAY[1] && money < PRICE_ARRAY[2])
                {
                    animation = "HappyIdle" + (i + 1);
                    if (animation == "HappyIdle1") animation = "HappyIdle2";
                }
                else if (money >= PRICE_ARRAY[2]) animation = "Win";
            }
            _animations[i] = animation;
        }
        for (int i = 0; i <= _lvl; i++)
        {
            looters = _lootersArr[i];
            int money = Looters2Money(looters);
            if (money == max && money > 0)
            {
                _bestIndex = i;
                break;
            }
        }
        _time = 0;
        _isBack = true;
    }

    #region get target position
    public Vector3 GetFallPos(GameObject from, GameObject to)
    {
        int index = GetStayAreaIndex(to);
        Vector3 pos = FALL_POSITIONS[index].transform.position;

        float range = 2.0f;
        //TODO: x, z is changed little
        float x = Random.Range(-range, range);
        float z = Random.Range(-range, range);
        pos.x += x;
        pos.z += z;
        return pos;
    }

    public Vector3 GetPos(GameObject who, GameObject target, bool isBump)
    {
        Vector3 pos;
        Platform platform = GetPlatfrom(target);
        if (platform == Platform.CoinPlatform)
        {
            pos = target.transform.position;
            pos.y = _abs_coin_positions[0].y;
            pos.y = 0.0f;
            if (!isBump)
            {
                //TODO: for move to back of coin
                float dis = 1.5f;
                pos.z += dis;
                return pos;
            }
            else
            {
                //TODO: for move to back of coin
                float dis = 1.0f;
                switch (who.name)
                {
                    case BOT1:
                        pos.x -= dis;
                        pos.z -= dis;
                        break;
                    case BOT2:
                        pos.x += dis;
                        pos.z -= dis;
                        break;
                    case BOT3:
                        pos.x -= dis;
                        pos.z += dis;
                        break;
                    case USER:
                        pos.x += dis;
                        pos.z += dis;
                        break;
                }
                return pos;
            }
        }

        else if (platform == Platform.StayareaPlatform)
        {
            int index = GetStayAreaIndex(target);
            pos = _abs_stay_positions[index];
            pos.y = 0.0f;
            if (!isBump) return pos;
            else
            {
                //TODO: for move to back of coin
                float dis = 1.0f;
                switch (who.name)
                {
                    case BOT1:
                        pos.x -= dis;
                        pos.z -= dis;
                        break;
                    case BOT2:
                        pos.x += dis;
                        pos.z -= dis;
                        break;
                    case BOT3:
                        pos.x -= dis;
                        pos.z += dis;
                        break;
                    case USER:
                        pos.x += dis;
                        pos.z += dis;
                        break;
                }
                return pos;
            }
        }
        else if (platform == Platform.SafezonePlatform)
        {
            int index = GetSafeZoneIndex(target);
            pos = _abs_safe_positions[index];
            pos.y = 0.0f;
            if (!isBump) return pos;

            else
            {
                //TODO: for move to back of coin
                float dis = 1.0f;
                switch (who.name)
                {
                    case BOT1:
                        pos.x -= dis;
                        pos.z -= dis;
                        break;
                    case BOT2:
                        pos.x += dis;
                        pos.z -= dis;
                        break;
                    case BOT3:
                        pos.x -= dis;
                        pos.z += dis;
                        break;
                    case USER:
                        pos.x += dis;
                        pos.z += dis;
                        break;
                }
                return pos;
            }

        }
        return Vector3.zero;
    }

    public Vector3 GetSafePos(GameObject target)
    {
        int index = GetStayAreaIndex(target);
        return _abs_safe_positions[index];
    }

    #endregion

    public int[] GetOrderIndex()
    {
        List<int> order = _allMoneyArr;
        int[] playerIndexs = new int[_lvl + 1];
        for (int i = 0; i <= _lvl; i++)
            playerIndexs[i] = i;
        bool itemMoved = false;
        do
        {
            itemMoved = false;
            for (int i = 0; i < order.Count - 1; i++)
            {
                if (order[i] < order[i + 1])
                {
                    int lowerValue = order[i + 1];
                    order[i + 1] = order[i];
                    order[i] = lowerValue;

                    lowerValue = playerIndexs[i + 1];
                    playerIndexs[i + 1] = playerIndexs[i];
                    playerIndexs[i] = lowerValue;

                    itemMoved = true;
                }
            }
        } while (itemMoved);
        //print( toStr(playerIndexs) + "   " + toStr(order)); 
        return playerIndexs;
    }

    public void CheckScore()
    {
        _allMoneyArr = new List<int>();
        _moneyArr = new List<int>();
        for (int i = 0; i <= _lvl; i++)
        {
            StayBase staybase = _stayAreas[i].GetComponent<StayBase>();             
            _allMoneyArr.Add(staybase.GetAllMoney());
            _moneyArr.Add(staybase.GetMoney());
            staybase.DisplayScore(GetBotName(i));
        }
        _order = GetOrderIndex();
        //myLog(order);
    }

    protected string GetCleverBot()
    {
        int r = GetRandomInt(1, 4);
        switch (r)
        {
            case 1: return BOT1;
            case 2: return BOT2;
            case 3: return BOT3;
            default: return string.Empty;
        }

    }

    protected GameObject GetBestTarget(GameObject from, bool isClever)
    {
        if (isClever)
        {
            //CheckScore();
            if (_moneyArr != null)
            {
                //TODO: who is clever bot
                int max = -9999;
                for (int i = 0; i <= _lvl; i++)
                {
                    int money = _moneyArr[i];
                    if (max < money)
                        max = money;
                }
                int index = 0;
                for (int i = 0; i <= _lvl; i++)
                {
                    int money = _moneyArr[i];
                    if (money == max)
                    {
                        index = i;
                        break;
                    }
                }
                if (max > 0)
                    return Player2Stayarea(_players[index]);
                else return GetBestTarget(from, false);
            }
            else return GetBestTarget(from, false);
        }
        else
        {
            while (true)
            {
                GameObject to = this.GetEnableTarget();
                Platform platform = GetPlatfrom(to);
                if (platform == Platform.CoinPlatform)
                    return to;
                else if (platform == Platform.SafezonePlatform)
                {
                    if (from.GetComponent<StayBase>().GetMoney() > 0)
                        return to;
                }
            }
        }

    }

    protected GameObject GetEnableTarget()
    {
        int n = GetRandomInt(1, COINS.Length + (_lvl + 1) + 1 + 0);
        if (_lvl == 1)
        {
            switch (n)
            {
                case 1: return COINS[0];
                case 2: return COINS[1];
                case 3: return COINS[2];
                case 4: return COINS[3];
                case 5: return _stayAreas[0];
                case 6: return _stayAreas[1];
                default: return null;
            }
        }
        else if (_lvl == 2)
        {
            switch (n)
            {
                case 1: return COINS[0];
                case 2: return COINS[1];
                case 3: return COINS[2];
                case 4: return COINS[3];
                case 5: return _stayAreas[0];
                case 6: return _stayAreas[1];
                case 7: return _stayAreas[2];
                default: return null;
            }
        }
        else
        {
            switch (n)
            {
                case 1: return COINS[0];
                case 2: return COINS[1];
                case 3: return COINS[2];
                case 4: return COINS[3];
                case 5: return _stayAreas[0];
                case 6: return _stayAreas[1];
                case 7: return _stayAreas[2];
                case 8: return _stayAreas[3];
                default: return null;
            }
        }
    }

    public void OnMouseEvent(GameObject to)
    {
        if (!_isEnable) return;
        switch (_lvl)
        {
            case 1:
                _froms[0] = _stayAreas[0];
                _tos[0] = this.GetBestTarget(_froms[0], false);
                _froms[1] = _stayAreas[1];
                _tos[1] = to;
                break;
            case 2:
                _froms[0] = _stayAreas[0];
                _froms[1] = _stayAreas[1];
                _froms[2] = _stayAreas[2];
                _tos[0] = this.GetBestTarget(_froms[0], false);
                _tos[1] = this.GetBestTarget(_froms[1], false);
                _tos[2] = to;
                break;
            default:
                _froms[0] = _stayAreas[0];
                _froms[1] = _stayAreas[1];
                _froms[2] = _stayAreas[2];
                _froms[3] = _stayAreas[3];
                string cleverBot = GetCleverBot();
                _tos[0] = this.GetBestTarget(_froms[0], cleverBot == BOT1);
                _tos[1] = this.GetBestTarget(_froms[1], cleverBot == BOT2);
                _tos[2] = this.GetBestTarget(_froms[2], cleverBot == BOT3);
                _tos[3] = to;
                break;
        }
        bool[] isBumps = new bool[_lvl + 1];
        for (int i = 0; i <= _lvl; i++)
            isBumps[i] = false;
        GameObject[] bumpers = new GameObject[_lvl + 1];
        for (int i = 0; i <= _lvl; i++)
        {
            string to1 = _tos[i].name;
            for (int j = 0; j <= _lvl; j++)
            {
                string to2 = _tos[j].name;
                if (i != j && to1 == to2)
                {
                    isBumps[i] = true;
                    bumpers[i] = _players[j];
                    isBumps[j] = true;
                }
            }
        }
        for (int i = 0; i <= _lvl; i++)
            _players[i].GetComponent<PlayerAction>().JumpForLooter(_froms[i], _tos[i], isBumps[i], bumpers[i]);
    }

    void OnGUI()
    {
        GUI.color = Color.yellow;
         
        int wide = Screen.width - 200, high = Screen.height - 100;
        //GUI.Label(new Rect(Screen.width / 2 - 100, 10, 200, 50), "LEVEL: " + LEVEL + " , ROUND: " + _round);

        if (_exit)
        { 
            GUI.Box(new Rect(Screen.width / 2 - wide / 2, Screen.height / 2 - high / 2, wide, high), "");
            wide = 150; high = 80;
             
            int m = 0;
            int max = -999;
            for (int i = 0; i <= _lvl; i++)
            {
                m = _stayAreas[i].GetComponent<StayBase>().GetAllMoney();
                if (max < m) max = m;
            }
            int maxIndex = 0;
            for (int i = 0; i <= _lvl; i++)
            {
                m = _stayAreas[i].GetComponent<StayBase>().GetAllMoney();
                if (max == m)
                {
                    maxIndex = i;
                    break;
                }
            } 
            bool win = _players[maxIndex].name == USER;

            if (win)
            {
                if (GUI.Button(new Rect(Screen.width / 2 - wide - 20, Screen.height / 2 - high / 2, wide, high), "Again", style))
                {
                    PlayerPrefs.SetInt("level", LEVEL);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    _exit = false;
                }

                if (GUI.Button(new Rect(Screen.width / 2 + 20, Screen.height / 2 - high / 2, wide, high)
                    , "WIN!\n\nNext Level", style))
                {
                    PlayerPrefs.SetInt("level", LEVEL + 1);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    _exit = false;
                }

            }
            else
            {
                if (GUI.Button(new Rect(Screen.width / 2 - wide / 2, Screen.height / 2 - high / 2, wide, high), "LOSE!\n\nAgain", style))
                {
                    PlayerPrefs.SetInt("level", LEVEL);
                    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
                    _exit = false;
                }
            }
        }
        //hSliderValue = GUI.HorizontalSlider(new Rect(Screen.width / 2 - 50, 50, 100, 30), hSliderValue, 1.0F, 10.0F);
        //if (GUI.Button(new Rect(35, 1, 60, 20), "LEVEL1"))
        //{
        //    PlayerPrefs.SetInt("level", 1);
        //    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        //}
        //if (GUI.Button(new Rect(100, 1, 60, 20), "LOG")) _debug ^= true;
        //if (!_debug) return;

        //int all_money = 0;
        //int safe_money = 0;
        //string str = "";
        //string s;

        //int money = COINS[0].GetComponent<CoinBase>().GetMoney();
        //str += "\ncoin1~4: " + money;
        //money = COINS[1].GetComponent<CoinBase>().GetMoney();
        //str += ", " + money;
        //money = COINS[2].GetComponent<CoinBase>().GetMoney();
        //str += ", " + money;
        //money = COINS[3].GetComponent<CoinBase>().GetMoney();
        //str += ", " + money + "\n\n";

        //if (_isLog) //
        //    for (int i = 0; i <= _lvl; i++)
        //    {
        //        int index = _order[i];
        //        index = i;
        //        StayBase obj = _stayAreas[index].GetComponent<StayBase>();
        //        all_money = obj.GetAllMoney();
        //        money = obj.GetMoney();
        //        safe_money = obj.GetSafeMoney();
        //        s = obj.GetLootersHistory();
        //        str += "--------------------" + _players[i].name + "--------------------\n" +
        //                s + "\nTotal:   " + all_money + "\n";
        //    }
        //GUI.TextField(new Rect(1, 22, 200, Screen.height - 23), _log + str);
    }
}