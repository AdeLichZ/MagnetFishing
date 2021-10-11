using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Singleton Init

    private static PlayerController _instance;
    private static bool isInitialized = false; // A bit faster singleton

    void Awake() // Init in order
    {
        if (_instance == null)
            Init();
        else if (_instance != this)
            Destroy(gameObject);
    }

    public static PlayerController Instance // Init not in order
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
        _instance = FindObjectOfType<PlayerController>();
        if (_instance != null)
        {
            _instance.Initialize();
            isInitialized = true;
        }
    }
    #endregion
    [SerializeField] GameObject ItemTradePanel;
    private Touch touch;

    [Header("MagnetControl Settings")]
    [SerializeField] GameObject magnet;
    [SerializeField] private float speedModifier;
    public float boundX;
    public float boundZ;
    private void Start()
    {
        ItemTradePanel.SetActive(false);
        magnet.SetActive(false);
        Clicker.ItemReceived += Clicker_ItemReceived;
        CollideWithItem.tradeFinished += CollideWithItem_tradeFinished;
    }

    private void CollideWithItem_tradeFinished()
    {
        ItemTradePanel.SetActive(false);
    }

    private void Initialize()
    {
        enabled = true;
    }
    private void Update()
    {
        if (MainData.Instance.gameState == MainData.GameState.PlayingGame || MainData.Instance.gameState == MainData.GameState.PlayingAnimation)
        {
            magnet.SetActive(true);
            MoveMagnet();
        }
    }
    private void Clicker_ItemReceived()
    {
        ItemTradePanel.SetActive(true);
    }
    private void MoveMagnet()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            float touchX = magnet.transform.position.x + touch.deltaPosition.x * speedModifier * Time.deltaTime;
            float touchZ = magnet.transform.position.z + touch.deltaPosition.y * speedModifier * Time.deltaTime;

            touchX = Mathf.Clamp(touchX, -boundX, boundX);
            touchZ = Mathf.Clamp(touchZ, -boundZ, boundZ);
            if (touch.phase == TouchPhase.Moved)
            {
                magnet.transform.position = new Vector3(touchX, magnet.transform.position.y, touchZ);
            }
        }
    }
}
