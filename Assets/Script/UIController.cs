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
            txtGold.text = database.Gold.ToString();
            txtDiamond.text = database.Diamond.ToString();
            isInit = false;
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
        int valPerFrame = (diamon - int.Parse(txtDiamond.text)) > 0 ? (diamon - int.Parse(txtDiamond.text)) : (int.Parse(txtDiamond.text) - diamon);
        while (int.Parse(txtDiamond.text) != diamon)
        {

            float waitTime = 0.05f;
            float maxWaitTime = 1f;
            var percent = waitTime / maxWaitTime;
            if (int.Parse(txtDiamond.text) < diamon)
            {
                txtDiamond.text = (int.Parse(txtDiamond.text) + valPerFrame * percent).ToString();
                if (int.Parse(txtDiamond.text) > diamon) txtDiamond.text = diamon.ToString();
            };
            if (int.Parse(txtDiamond.text) > diamon)
            {
                txtDiamond.text = (int.Parse(txtDiamond.text) - valPerFrame * percent).ToString();
                if (int.Parse(txtDiamond.text) < diamon) txtDiamond.text = diamon.ToString();
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
    IEnumerator MoneyChange(int gold)
    {
        int valPerFrame = (gold - int.Parse(txtGold.text)) > 0 ? (gold - int.Parse(txtGold.text)) : (int.Parse(txtGold.text) - gold);
        while (int.Parse(txtGold.text) != gold)
        {
            
            float waitTime = 0.05f;
            float maxWaitTime = 1f;
            var percent = waitTime/ maxWaitTime;
            if (int.Parse(txtGold.text) < gold)
            {
                txtGold.text = (int.Parse(txtGold.text) + valPerFrame * percent).ToString();
                if(int.Parse(txtGold.text) > gold) txtGold.text = gold.ToString();
            };
            if (int.Parse(txtGold.text) > gold)
            {
                txtGold.text = (int.Parse(txtGold.text) - valPerFrame * percent).ToString();
                if (int.Parse(txtGold.text) < gold) txtGold.text = gold.ToString();
            }
            yield return new WaitForSeconds(waitTime);
        }
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
        // bar.sizeDelta = new Vector2(0, 200);

        var expandSize = 80;
        var deactivesize = (bar.rect.width - expandSize) / 4;
        var activesize = deactivesize + expandSize;
        for (int i = 1; i <= 4; i++)
        {
            hightlight[i].gameObject.GetComponent<Image>().color = Color.white;
            hightlight[i].gameObject.transform.localScale = new Vector3(1,1,1);
            
            menuText[i].SetActive(false);
            ((RectTransform)bar.GetChild(i - 1).transform).sizeDelta = new Vector2(deactivesize, 300);
            var trans = (RectTransform)menuSprite[i].gameObject.transform;
            var minsize = trans.rect.width > trans.rect.height ? trans.rect.height : trans.rect.width;
            trans.sizeDelta = new Vector2(minsize , minsize);
            hightlight[i].gameObject.transform.localPosition = new Vector3(0, 0, 0);
            menuSprite[i].gameObject.transform.localPosition = new Vector3(0, 0, 0);
        }
        menuText[currentSite].SetActive(true);
        menuText[currentSite].gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0,100);
        ((RectTransform)bar.GetChild(currentSite - 1).transform).sizeDelta = new Vector2(activesize, 330);
      
        hightlight[currentSite].gameObject.transform.localPosition = new Vector3(0, 55, 0);
        menuSprite[currentSite].gameObject.transform.localPosition = new Vector3(0, 55, 0);
        hightlight[currentSite].gameObject.GetComponent<Image>().color = new Color32(61,49,45,255);
     
        
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
