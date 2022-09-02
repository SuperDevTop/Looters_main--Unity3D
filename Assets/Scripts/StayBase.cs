using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StayBase : MonoBehaviour
{
    public GameManager GAME_MANAGER;
    public GameObject UI;
    int[] _safeLooters;
    int[] _looters;
    GameObject _to;
    int[] _prices ;
    List<int[]> _gotLooterHistroy;
    List<int[]> _stealedLooterHistory;
    List<int[]> _safeLooterHistory;
    List<int[]> _looterHistory;

    private void Awake()
    {
        _looters = new int[4] { 0, 0, 0, 0 };
        _safeLooters = new int[4] { 0, 0, 0, 0 };
        _prices = new int[4] { 0, 0, 0, 0 };
        _stealedLooterHistory = new List<int[]>();
        _gotLooterHistroy = new List<int[]>();
        _safeLooterHistory = new List<int[]>();
        _looterHistory = new List<int[]>();
        //if (!GAME_MANAGER) GAME_MANAGER = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
   
    public void DisplayScore(string playerName)
    {
        try
        {     
            UI.GetComponent<Text>().text = playerName + ": " + GetAllMoney() + " $";
        }
        catch { }
    }

    void Start()
    {
        _to = this.gameObject; 
         _prices = GAME_MANAGER.GetPrices();
        //DisplayScore();
    } 

    public int[] GetStealedLooters()
    {
        return this._stealedLooterHistory[this._stealedLooterHistory.Count - 1];
    }
    
    public void StealedByOther(GameObject who)
    {
        //_safeLooterHistory.Add(_looters);
        this._looters = new int[4] { 0,0,0,0 };
        //DisplayScore();
    }

    public void AddLooters(int[] looters, bool stealed)
    { 
        for(int i = 0; i < 4; i++) 
            this._looters[i] += looters[i];
        _looterHistory.Add(looters);
        if (stealed)
        {
            if(_gotLooterHistroy != null)
                _gotLooterHistroy.Add(new int[4] { 0, 0, 0, 0 });
            if (_stealedLooterHistory != null)
                _stealedLooterHistory.Add(looters);
        }
        else
        {
            if (_stealedLooterHistory != null)
                _stealedLooterHistory.Add(new int[4] { 0, 0, 0, 0 });
            if (_gotLooterHistroy != null)
                _gotLooterHistroy.Add(looters);
        }
        //DisplayScore();
    } 
    
    public int[] GetLooters()
    {
        return this._looters;
    }
    
    public int[] GetSafeLooters()
    {
        return this._safeLooters;
    }
    
    public string GetLootersHistory()
    {
        string res = "Get:     ";
        if (_gotLooterHistroy == null) return string.Empty;
        int len = _gotLooterHistroy.Count;
        for (int i = 0; i < len; i++)
        {
            int[] looters = _gotLooterHistroy[i];
            int money = GAME_MANAGER.Looters2Money(looters);
            res += money + ", ";
        }
        res += "\nSteal:   ";
        len = _stealedLooterHistory.Count;
        for (int i = 0; i < len; i++)
        {
            int[] looters = _stealedLooterHistory[i];
            int money = GAME_MANAGER.Looters2Money(looters);
            res += money + ", ";
        }
        res += "\nSafe:    ";
        len = _safeLooterHistory.Count;
        for (int i = 0; i < len; i++)
        {
            int[] looters = _safeLooterHistory[i];
            int money = GAME_MANAGER.Looters2Money(looters);
            //print(money);
            res += money + ", ";
        }

        res += "\nMoney: ";
        len = _looterHistory.Count;
        for (int i = 0; i < len; i++)
        {
            int[] looters = _looterHistory[i];
            int money = GAME_MANAGER.Looters2Money(looters);
            //print(money);
            res += money + ", ";
        }


        

        return res;
    }
   
    public int GetSafeMoney()
    {        
        int res = 0;
        for (int i = 0; i < 4; i++) 
            res += _safeLooters[i] * _prices[i]; 
        return res;
    }

    public int GetMoney()
    {       
        int res = 0;          
        for (int i = 0; i < 4; i++) 
            res += _looters[i] * _prices[i]; 
        return res;
    }

    public int GetAllMoney()
    { 
        return GetMoney() + GetSafeMoney();
    }
    
    public void SetSafeLooters()
    {  
        for (int i = 0; i < 4; i++)
        {
            _safeLooters[i] += _looters[i];
            _looters[i] = 0;
        }
        _safeLooterHistory.Add(_safeLooters);
        //DisplayScore();
    }

    public void SetSafeLooterHistory()
    { 
        _safeLooterHistory.Add(new int[4] { 0,0,0,0 });  
    }

    void OnMouseDown()
    {
        GAME_MANAGER.OnMouseEvent (_to); 
    }
}
