
using UnityEngine;
public class CoinBase : MonoBehaviour
{
    public GameManager GAME_MANAGER;
    int[] _looters;
    GameObject _to;
    int[] _prices;

    private void Awake()
    {
        this._looters = new int[4] { 0, 0, 0, 0 };
    }
   
    void Start()
    {
        _to = this.gameObject;
       
       // if(!GAME_MANAGER) GAME_MANAGER = GameObject.Find("GameManager").GetComponent<GameManager>();
        _prices = GAME_MANAGER.GetPrices();
    }  

    public void SetLooters(int[] looters)
    {
        this._looters = looters;
    }

    public int[] GetLooters()
    {
        return this._looters;
    }
    
    public int GetMoney()
    {        
        int res = 0;
        for (int i = 0; i < 4; i++) 
            res += _looters[i] * _prices[i]; 
        return res; 
    }
    
    void OnMouseDown()
    {
        GAME_MANAGER.OnMouseEvent (_to);
    }
}
