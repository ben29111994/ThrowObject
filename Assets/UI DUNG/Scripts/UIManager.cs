using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    [Header("References")]
    public GameObject coinUI;
    public GameObject homeUI;
    public GameObject matchUI;
    public GameObject playUI;
    public GameObject winUI;
    public GameObject resultUI;

    public Text coinText;
    public Sprite[] flagsSpr;
    public string[] listName;

    [Header("Player")]
    public Sprite flagPlayer;

    [Header("Enemy")]
    public Sprite flagEnemy;
    public string nameEnemy;

    [Header("TopUI")]
    public RectTransform[] topUI;

    [Header("Effect")]
    public ParticleSystem[] fireWork;

    public int Coin
    {
        get
        {
            return PlayerPrefs.GetInt("Coin");
        }
        set
        {
            PlayerPrefs.SetInt("Coin", value);
            coinText.text = value.ToString();
            coinText.transform.DOScale(Vector2.one * 1.2f, 0.02f).SetLoops(2, LoopType.Yoyo);
        }
    }

    private void Start()
    {
        Coin += 0;
        FixedTopUI();
        Show_Home_UI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H)) Show_Home_UI();
        if (Input.GetKeyDown(KeyCode.M)) Show_Match_UI();
        if (Input.GetKeyDown(KeyCode.W)) Show_Win_UI(3);
        if (Input.GetKeyDown(KeyCode.L)) Show_Lose_UI(1);
        if (Input.GetKeyDown(KeyCode.F)) FireWorkEffect();
    }

    public void Show_Home_UI()
    {
        coinUI.SetActive(true);
        homeUI.SetActive(true);

        matchUI.SetActive(false);
        playUI.SetActive(false);
        winUI.SetActive(false);
        resultUI.SetActive(false);
    }

    public void Show_Match_UI()
    {
        matchUI.SetActive(true);

        coinUI.SetActive(false);
        homeUI.SetActive(false);
        playUI.SetActive(false);
        winUI.SetActive(false);
        resultUI.SetActive(false);
    }

    public void Show_Play_UI()
    {
        playUI.SetActive(true);

        matchUI.SetActive(false);
        coinUI.SetActive(false);
        homeUI.SetActive(false);
        winUI.SetActive(false);
        resultUI.SetActive(false);
    }

    public void Show_Win_UI(int star)
    {
        winUI.GetComponent<WinUI>().ShowWin(star,true);

        matchUI.SetActive(false);
        coinUI.SetActive(false);
        homeUI.SetActive(false);
        resultUI.SetActive(false);
    }

    public void Show_Lose_UI(int star)
    {
        winUI.GetComponent<WinUI>().ShowWin(star,false);

        matchUI.SetActive(false);
        coinUI.SetActive(false);
        homeUI.SetActive(false);
        resultUI.SetActive(false);
    }

    public void Show_Result_UI()
    {
        resultUI.SetActive(true);
        coinUI.SetActive(true);

        playUI.SetActive(false);
        matchUI.SetActive(false);
        homeUI.SetActive(false);
        winUI.SetActive(false);
    }

    private void FixedTopUI()
    {
        float ratio = Camera.main.aspect;

        if (ratio >= 0.74) // 3:4
        {
           
        }
        else if (ratio >= 0.56) // 9:16
        {
           
        }
        else if (ratio >= 0.45) // 9:19
        {
            for(int i = 0; i < topUI.Length; i++)
            {
                topUI[i].anchoredPosition -= Vector2.up * 100.0f;
            }
        }
    }

    public void FireWorkEffect()
    {
        if (fireWork[0].isPlaying) return;

        fireWork[0].Play();
        fireWork[1].Play();
    }
}
