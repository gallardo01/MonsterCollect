using DG.Tweening;
using DigitalRuby.SoundManagerNamespace;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class UIController : Singleton<UIController>
{
    public RectTransform shop;
    public RectTransform heroes;
    public RectTransform mainMenu;
    public RectTransform upgrade;
    public RectTransform events;

    public RectTransform bar;

    public Button shopBtn;
    public Button heroesBtn;
    public Button mainMenuBtn;
    public Button upgradeBtn;
    public Button eventBtn;

    public TextMeshProUGUI txtGold;
    public TextMeshProUGUI txtDiamond;

    public GameObject[] hightlight;
    public GameObject[] menuText;

    private int currentSite = 3;

    public bool enableSwipe;

    private bool isInit = true;

    private void Awake()
    {
    }
    void Start()
    {
        Application.targetFrameRate = 100;
        enableSwipe = true;
        initCurrency();
        shopBtn.onClick.AddListener(() => shopButton());
        heroesBtn.onClick.AddListener(() => heoresButton());
        mainMenuBtn.onClick.AddListener(() => mainMenuButton());
        upgradeBtn.onClick.AddListener(() => upgradeButton());
        eventBtn.onClick.AddListener(() => eventsButton());
        SoundManagerDemo.Instance.StopAudio(1);
        SoundManagerDemo.Instance.StopAudio(13);
        SoundManagerDemo.Instance.playMusic(8);
        startGame();

        AdsController.Instance.ShowRewardedAd();
    }
    private void initCurrency()
    {
        UserData database = UserDatabase.Instance.getUserData();
        txtDiamond.text = database.Diamond.ToString();
        txtGold.text = database.Gold.ToString();
    }
    public void InitUI()
    {
        UserData database = UserDatabase.Instance.getUserData();

        StartCoroutine(MoneyChange(database.Gold));
        StartCoroutine(DiamonChange(database.Diamond));
    }
    IEnumerator DiamonChange(int diamond)
    {
        int firstValue = int.Parse(txtDiamond.text);
        int valChange = diamond - firstValue;
        if (valChange != 0)
        {
            for (int i = 0; i < 20; i++)
            {
                firstValue += (int)(valChange * 0.05f);
                txtDiamond.text = firstValue.ToString();
                yield return new WaitForSeconds(0.02f);
            }
        }
        txtDiamond.text = diamond.ToString();
    }
    IEnumerator MoneyChange(int gold)
    {
        int firstValue = int.Parse(txtGold.text);
        int valChange = gold - firstValue;
        if (valChange != 0)
        {
            for (int i = 0; i < 20; i++)
            {
                firstValue += (int)(valChange * 0.05f);
                txtGold.text = firstValue.ToString();
                yield return new WaitForSeconds(0.02f);
            }
        }
        txtGold.text = gold.ToString();
    }
    private void shopButton()
    {
        // active bar & swipe
        enableSwipe = true;
        SoundManagerDemo.Instance.playOneShot(9);
        currentSite = 1;
        shop.DOAnchorPos(new Vector2(0, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(4000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(6000, 0), 0.25f);
        events.DOAnchorPos(new Vector2(8000, 0), 0.25f);
        setupHightLight();
        activeLayer();
    }

    private void heoresButton()
    {
        currentSite = 2;
        SoundManagerDemo.Instance.playOneShot(9);
        shop.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(0, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(4000, 0), 0.25f);
        events.DOAnchorPos(new Vector2(6000, 0), 0.25f);
        setupHightLight();
        activeLayer();
    }

    private void startGame()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        currentSite = 3;
        shop.DOAnchorPos(new Vector2(-4000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        events.DOAnchorPos(new Vector2(4000, 0), 0.25f);
        setupHightLight();
        shop.gameObject.SetActive(false);
        upgrade.gameObject.SetActive(false);
        events.gameObject.SetActive(false);
    }
    private void mainMenuButton()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        currentSite = 3;
        shop.DOAnchorPos(new Vector2(-4000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        events.DOAnchorPos(new Vector2(4000, 0), 0.25f);
        setupHightLight();
        activeLayer();
    }

    private void upgradeButton()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        currentSite = 4;
        shop.DOAnchorPos(new Vector2(-6000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(-4000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(0, 0), 0.25f);
        events.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        setupHightLight();
        activeLayer();
    }

    private void eventsButton()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        currentSite = 5;
        shop.DOAnchorPos(new Vector2(-8000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(-6000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(-4000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        events.DOAnchorPos(new Vector2(0, 0), 0.25f);
        setupHightLight();
        activeLayer();
    }
    private void activeLayer()
    {
        shop.gameObject.SetActive(currentSite == 1);
        //heroes.gameObject.SetActive(currentSite == 2);
        mainMenu.gameObject.SetActive(currentSite == 3);
        upgrade.gameObject.SetActive(currentSite == 4);
        events.gameObject.SetActive(currentSite == 5);
    }

    private void setupHightLight()
    {
        for (int i = 0; i < 5; i++)
        {
            menuText[i].SetActive(false);
            hightlight[i].SetActive(false);
        }
        menuText[currentSite - 1].SetActive(true);
        hightlight[currentSite - 1].SetActive(true);
    }

    public void detectSwipe(int direction)
    {
        if (enableSwipe)
        {
            // 1 = left, 2 = right
            if (direction == 1)
            {
                if (currentSite > 1)
                {
                    currentSite--;
                }
            }
            else
            {
                if (currentSite < 4)
                {
                    currentSite++;
                }
            }
            if (currentSite == 1)
            {
                shopButton();
            }
            else if (currentSite == 2)
            {
                heoresButton();
            }
            else if (currentSite == 3)
            {
                mainMenuButton();
            }
            else if (currentSite == 4)
            {
                upgradeButton();
            }
            else
            {
                eventsButton();
            }
        }

    }
}
