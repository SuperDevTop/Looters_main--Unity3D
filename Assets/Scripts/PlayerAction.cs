using UnityEngine;
using UnityEngine.UI;
public class PlayerAction : MonoBehaviour
{
    #region variables 
    public GameObject MODEL;
    public GameObject[] LOOTERS;
    public Transform LOOTER_POS;
    public GameManager GAME_MANAGER;
    public GameObject UI;
    Vector3 _start, _end;
    float _high, _flySpeed;   
    GameObject _player;
    float _time = 0;
    GameObject _bumper;     
    GameObject _from, _to; 
    bool _isStart;
    bool _isAgain;
    bool _isFall;
    bool _isBump;    
    float _bumpersDistance;
    float _limitDeep;
    #endregion
      
    void Start()
    {
        _player = this.gameObject;
        // if (!GAME_MANAGER) GAME_MANAGER = GameObject.Find("GameManager").GetComponent<GameManager>();
                 
        _flySpeed = GAME_MANAGER.GetFlySpeed();
        _high = GAME_MANAGER.GetFlyHigh();
        _bumpersDistance = GAME_MANAGER.GetBumpersDistance();
        _limitDeep = GAME_MANAGER.GetLimitDeep();
        _isAgain = false;
        _isBump = false;
        _isFall = false;
    }

    public void DisplayName(string playerName)
    {
        UI.GetComponent<Text>().text = playerName;
    }
    public Animation GetAnimation()
    {
        return MODEL.GetComponent<Animation>();
    }

    protected float ChangeHigh(float x, float height)
    {
        return -4 * height * x * x + 4 * height * x;
    }
    
    protected Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        var mid = Vector3.Lerp(start, end, t);
        return new Vector3(mid.x, ChangeHigh(t, height) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }
    
    void Update()
    {
        if (!_isStart) return;
        _time += Time.deltaTime;
        Vector3 pos;
        if (_from == _to) _high = 0;
        else _high = GAME_MANAGER.GetFlyHigh();
        pos = Parabola(_start, _end, _high, _time * _flySpeed);
        transform.position = pos;

        if (!_isFall)
        {
            if ( pos.x == _end.x && pos.z == _end.z) //pos.y <= _end.y &&
            {
                GAME_MANAGER.AnimationPlay(_player, "Idle");
                transform.position = _end;
                if( MODEL.transform.position.y > 0 )
                {
                    MODEL.transform.position = new Vector3(MODEL.transform.position.x, 0, MODEL.transform.position.z);
                }
                print(transform.position);
                if (_bumper)
                { 
                    float dis = _bumpersDistance; 
                    
                    Vector3 pos1 = _end;
                    Vector3 pos2 = _bumper.transform.position; 
                  
                    pos1.x -= dis;
                    pos1.z -= dis;
                    //this.transform.position = pos1;

                    pos2.x += dis;
                    pos2.z += dis;
                    //coll.transform.position = pos2;

                    Vector3 pos3;
                    pos3.x = pos1.x / 2 + pos2.x / 2;
                    pos3.z = pos1.z / 2 + pos2.z / 2;
                    pos3.y = pos1.y;

                    this.transform.LookAt(pos3, this.transform.up);
                    Vector3 rot = this.transform.localEulerAngles;
                    rot.x = 0;
                    rot.z = 0;
                    this.transform.localEulerAngles = rot;
                    _bumper.transform.LookAt(pos3, _bumper.transform.up);
                    rot = _bumper.transform.localEulerAngles;
                    rot.x = 0;
                    rot.z = 0;
                    _bumper.transform.localEulerAngles = rot;

                    //float dis_ = Vector3.Distance(pos1, pos2);
                    //print(_name + "+" + _collider + ": " + dis_);
                }
                _isStart = false;
                //
                if (!_isAgain)
                {
                    print(_player.name + ", " + _from.name + ", " + _to.name + ", " + _isBump);
                    GAME_MANAGER.OnArrivedTargetPlatform(_player, _from, _to, _isBump);
                }
                else
                {
                    GAME_MANAGER.OnArrivedStayareaPlatform(_player);
                    GAME_MANAGER.SetEnable(true);

                }
            }
        }
        else if (_isFall)
        {
            if (pos.y < _limitDeep && pos.x == _end.x && pos.z == _end.z)
            {
                //print(_limitDeep); //TODO:
                transform.position = GAME_MANAGER.GetPos(_player, _to, false);
                GAME_MANAGER.AnimationPlay(_player, "Idle+Flex2");
                _isFall = false;
                _isStart = false;
                GAME_MANAGER.SetEnable(true);
            }
        }
    }

    public void JumpForLooter(GameObject from, GameObject to, bool isBump, GameObject bumper)
    {
        bool isDefence = from == to;
        if (isDefence)
        {
            //string playerName = GAME_MANAGER.GetPlayerByStayarea(from);
            GAME_MANAGER.ChangeParent_Stayarea2PlayerForMove(from, _player);
            GAME_MANAGER.AnimationPlay(_player, "PushungSlow");
        }
        else GAME_MANAGER.AnimationPlay(_player, "JumpFalling");
         
        _time = 0;
        _from = from;
        _to = to;
        _isBump = isBump;
        _bumper = bumper;
        _start = GAME_MANAGER.GetPos(_player, from, false);

        if (isDefence)
            _end = GAME_MANAGER.GetSafePos(to);
        else _end = GAME_MANAGER.GetPos(_player, to, isBump);
        if (isBump)
        { 
            if (bumper.transform.position.x < this.transform.position.x)
            {
                transform.eulerAngles = new Vector3(0, 90, 0);
                bumper.transform.eulerAngles = new Vector3(0, -90, 0);
            }
            else
            {
                transform.eulerAngles = new Vector3(0, -90, 0);
                bumper.transform.eulerAngles = new Vector3(0, 90, 0);
            }
        }
        if (isDefence)
        {
            float angle = 0.0f;
            if (_player.name == GAME_MANAGER.GetPlayerName(1)) angle = 90.0f;
            else if (_player.name == GAME_MANAGER.GetPlayerName(2)) angle = 0.0f;
            else if (_player.name == GAME_MANAGER.GetPlayerName(3)) angle = 270.0f;
            else angle = 180.0f;
            transform.eulerAngles = new Vector3(0.0f, angle, 0.0f);
        }
        _isAgain = false;
        _isStart = true;
        GAME_MANAGER.SetEnable(false);
    }

    public void JumpForBack(GameObject from, GameObject to, int[] looters_)
    {
        _time = 0;
        _from = from;
        _to = to;
        _start = GAME_MANAGER.GetPos(_player, from, false);
        _end = GAME_MANAGER.GetPos(_player, to, false);
        _end.y = 0.0f;
        GameObject who = GAME_MANAGER.Stayarea2Player(to);
        GameObject safe = GAME_MANAGER.Stayarea2Safezone(to);

        if (from == to)
        {
            GAME_MANAGER.ChangeParent_Player2Safezone(who, safe, GAME_MANAGER.GetCoinTag());
        }
        else
        {
            Platform platform = GAME_MANAGER.GetPlatfrom(to);
            if (platform == Platform.StayareaPlatform)
            {
                GAME_MANAGER.AnimationPlay(_player, "CarryBoxDance");
                GAME_MANAGER.ChangePosStealed(who);
            }

            for (int i = 0; i < 4; i++)
            {
                int cnt = looters_[i];
                for (int j = 0; j < cnt; j++)
                {
                    Vector3 v3 = LOOTER_POS.position;
                    v3.x = LOOTER_POS.position.x + UnityEngine.Random.Range(-0.5f, 0.5f);
                    v3.z = LOOTER_POS.position.z + UnityEngine.Random.Range(-0.5f, 0.5f);
                    GameObject clone = GameObject.Instantiate(LOOTERS[i], v3, Quaternion.identity, this.transform) as GameObject;
                    //TODO: change got coin scale
                    float scale = 3 + j * 1.0f;
                    scale = 1 + j * 1.0f;
                    clone.transform.localScale = new Vector3(scale, scale, scale);
                    GAME_MANAGER.AnimationPlay(_player, "CarryBoxDance");
                }
            }
        }

        _isAgain = true;
        _isStart = true;
        GAME_MANAGER.SetEnable(false);
    }

    public void FallForBack(GameObject from, GameObject to)
    {
        _time = 0;
        _from = from;
        _to = to;
        _start = GAME_MANAGER.GetPos(_player, from, false);
        _end = GAME_MANAGER.GetFallPos(from, to);
        //print(_name + ", " + from + ", " + to);
        _isFall = true;
        _isStart = true;
        GAME_MANAGER.SetEnable(false);
    }

}
