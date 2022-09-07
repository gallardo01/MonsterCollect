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


	void Start()
	{
		enableSwipe = true;
		InitUI();
		mainMenuButton();
		bar.sizeDelta = new Vector2(0, 300);
		shopBtn.onClick.AddListener(() => shopButton());
		heroesBtn.onClick.AddListener(() => heoresButton());
		mainMenuBtn.onClick.AddListener(() => mainMenuButton());
		upgradeBtn.onClick.AddListener(() => upgradeButton());

		addGoldBtn.onClick.AddListener(() => shopButton());
		addDiamondBtn.onClick.AddListener(() => shopButton());
	}

    public void InitUI()
    {
		UserData database = UserDatabase.Instance.getUserData();
		txtGold.text = database.Gold.ToString();
		txtDiamond.text = database.Diamond.ToString();
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
		var expandSize = 80;
		var deactivesize = (bar.rect.width - expandSize) / 4;
		var activesize = deactivesize + expandSize;
		for (int i = 1; i <= 4; i++)
        {
			hightlight[i].SetActive(false);
			menuText[i].SetActive(false);
			((RectTransform)bar.GetChild(i - 1).transform).sizeDelta = new Vector2(deactivesize,300);
			var trans = (RectTransform)menuSprite[i].gameObject.transform;
			var minsize = trans.rect.width > trans.rect.height ? trans.rect.height : trans.rect.width;
			trans.sizeDelta = new Vector2(minsize+20,minsize);
			
			menuSprite[i].gameObject.transform.localPosition = new Vector3(0, 0, 0);
		}
		((RectTransform)bar.GetChild(currentSite - 1).transform).sizeDelta = new Vector2(activesize, 310);
		menuSprite[currentSite].gameObject.transform.localPosition = new Vector3(0, 40, 0);
		hightlight[currentSite].SetActive(true);
		menuText[currentSite].SetActive(true);
	}

	public void detectSwipe(int direction)
    {
		if(enableSwipe)
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
