using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CollideWithItem : MonoBehaviour
{
    #region Singleton Init

    private static CollideWithItem _instance;
    private static bool isInitialized = false; // A bit faster singleton

    void Awake() // Init in order
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);
    }

    public static CollideWithItem Instance // Init not in order
    {
        get
        {
            if (_instance == null)
                Init();
            return _instance;
        }
        private set { _instance = value; }
    }

    static void Init() // Init script
    {
        _instance = FindObjectOfType<CollideWithItem>();
        if (_instance != null)
        {
            _instance.Initialize();
            isInitialized = true;
        }
    }
    #endregion
    [SerializeField] GameObject clickBar;
    [SerializeField] Text itemLostTxt;
    [SerializeField] Text itemDropedTxt;
    [SerializeField] Text itemSoldTxt;

    public static event Action tradeFinished;
    private void Initialize()
    {
        enabled = true;
    }
    private void Start()
    {
        itemDropedTxt.enabled = false;
        itemSoldTxt.enabled = false;
        itemLostTxt.enabled = false;
        Clicker.ItemLost += Clicker_ItemLost;
        Clicker.ItemReceived += Clicker_ItemReceived;
    }

    private void Clicker_ItemReceived()
    {
        
        clickBar.SetActive(false);

    }

    private void Clicker_ItemLost()
    {
        clickBar.SetActive(false);
       
        StartCoroutine(EnableText(itemLostTxt));
        MainData.Instance.gameState = MainData.GameState.PlayingGame;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            MainData.Instance.gameState = MainData.GameState.HoldInput;
            clickBar.SetActive(true);
            var item = other.GetComponent<Item>();
            item.ChaseQuality();
            Destroy(other.gameObject);
            if (item.canTrade)
            {
                Debug.Log("Can trade");
            }
            else
            {
                Debug.Log("It's a junk");
            }
        }
    }
    IEnumerator EnableText(Text text)
    {
        text.enabled = true;
        yield return new WaitForSeconds(3f);
        text.enabled = false;
    }
    private void OnDestroy()
    {
        Clicker.ItemLost -= Clicker_ItemLost;
        Clicker.ItemReceived -= Clicker_ItemReceived;
    }
    public void TradeItem()
    {
        StartCoroutine(EnableText(itemSoldTxt));
        MainData.Instance.gameState = MainData.GameState.PlayingGame;
        tradeFinished?.Invoke();
        MainData.Instance.gameState = MainData.GameState.PlayingGame;
    }
    public void ThrowOutItem()
    {
        StartCoroutine(EnableText(itemDropedTxt));
        MainData.Instance.gameState = MainData.GameState.PlayingGame;
        tradeFinished?.Invoke();
        MainData.Instance.gameState = MainData.GameState.PlayingGame;
    }
}
