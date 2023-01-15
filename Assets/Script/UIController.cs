using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
public class UIController : Singleton<UIController>
{
    public RectTransform shop;
    public RectTransform heroes;
    public RectTransform mainMenu;
    public RectTransform upgrade;

    public RectTransform bar;

    public Button shopBtn;
    public Button heroesBtn;
    public Button mainMenuBtn;
    public Button upgradeBtn;

    public Button addGoldBtn;
    public Button addDiamondBtn;

    public TextMeshProUGUI txtGold;
    public TextMeshProUGUI txtDiamond;

    public GameObject[] hightlight;
    public GameObject[] menuSprite;
    public GameObject[] menuText;

    private int currentSite = 3;

    public bool enableSwipe;

    private bool isInit = true;

    void Start()
    {
        enableSwipe = true;

        InitUI();
        shopBtn.onClick.AddListener(() => shopButton());
        heroesBtn.onClick.AddListener(() => heoresButton());
        mainMenuBtn.onClick.AddListener(() => mainMenuButton());
        upgradeBtn.onClick.AddListener(() => upgradeButton());

        addGoldBtn.onClick.AddListener(() => shopButton());
        addDiamondBtn.onClick.AddListener(() => shopButton());
        mainMenuButton();
        menuText[currentSite].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,25);

    }

    public void InitUI()
    {
        UserData database = UserDatabase.Instance.getUserData();
        if (isInit)
        {
            //txtGold.text = database.Gold.ToString();
            //txtDiamond.text = database.Diamond.ToString();
            //isInit = false;
        }
        else
        {
            if (int.Parse(txtGold.text) != database.Gold)
                StartCoroutine(MoneyChange(database.Gold));
            if (int.Parse(txtDiamond.text) != database.Diamond)
                StartCoroutine(DiamonChange(database.Diamond));
        }
    }
    IEnumerator DiamonChange(int diamon)
    {
        int firstValue = int.Parse(txtDiamond.text);
        int valChange = Mathf.Abs(firstValue - diamon);

        for (int i = 0; i < 10; i++)
        {
            firstValue -= (int)(valChange * 0.05f);
            txtDiamond.text = firstValue.ToString();
            yield return new WaitForSeconds(0.05f);
        }
        txtDiamond.text = diamon.ToString();
    }
    IEnumerator MoneyChange(int gold)
    {
        int firstValue = int.Parse(txtGold.text);
        int valChange = Mathf.Abs(firstValue - gold);

        for (int i = 0; i < 10; i++)
        {
            firstValue -= (int)(valChange * 0.05f);
            txtGold.text = firstValue.ToString();
            yield return new WaitForSeconds(0.05f);
        }
        txtGold.text = gold.ToString();
    }
    private void shopButton()
    {
        // active bar & swipe
        enableSwipe = true;
        //bar.gameObject.SetActive(true);
        //UIHero.Instance.closeEvolvePanel();
        //UIHero.Instance.backToInventory();

        currentSite = 1;
        shop.DOAnchorPos(new Vector2(0, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(4000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(6000, 0), 0.25f);
        setupHightLight();
    }

    private void heoresButton()
    {
        currentSite = 2;
        shop.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(0, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(4000, 0), 0.25f);
        setupHightLight();
    }

    private void mainMenuButton()
    {
        currentSite = 3;
        shop.DOAnchorPos(new Vector2(-4000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(0, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(2000, 0), 0.25f);
        setupHightLight();
    }

    private void upgradeButton()
    {
        currentSite = 4;
        shop.DOAnchorPos(new Vector2(-6000, 0), 0.25f);
        heroes.DOAnchorPos(new Vector2(-4000, 0), 0.25f);
        mainMenu.DOAnchorPos(new Vector2(-2000, 0), 0.25f);
        upgrade.DOAnchorPos(new Vector2(0, 0), 0.25f);
        setupHightLight();
    }

    private void setupHightLight()
    {
        for (int i = 1; i <= 4; i++)
        {
            hightlight[i].SetActive(false);
        }
        hightlight[currentSite].SetActive(true);     
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
            else
            {
                upgradeButton();
            }
        }

    }
}
