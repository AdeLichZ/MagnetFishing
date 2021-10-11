using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Clicker : MonoBehaviour
{
    #region Singleton Init

    private static Clicker _instance;
    private static bool isInitialized = false; // A bit faster singleton

    void Awake() // Init in order
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);
    }

    public static Clicker Instance // Init not in order
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
        _instance = FindObjectOfType<Clicker>();
        if (_instance != null)
        {
            _instance.Initialize();
            isInitialized = true;
        }
    }
    #endregion
    [SerializeField] private Image barImage;

    public const int FILL_BAR = 100;
    [SerializeField] private float manaAmount = 30;
    private float manaRegenAmount = 15;

    public static event Action ItemReceived;
    public static event Action ItemLost;
    private void Initialize()
    {
        enabled = true;
    }
    void Update()
    {
        manaAmount -= manaRegenAmount * Time.deltaTime;
        manaAmount = Mathf.Clamp(manaAmount, 0f, FILL_BAR);
        barImage.fillAmount = GetManaNormalized();
        if(manaAmount > 95)
        {
            ItemReceived?.Invoke();
        }
        if(manaAmount <= 0)
        {
            ItemLost?.Invoke();
        }
    }
    public void TryGainMana(int amount)
    {
        manaAmount += amount;
    }
    private float GetManaNormalized()
    {
        return manaAmount / FILL_BAR;
    }
}
