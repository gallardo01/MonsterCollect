using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using Mono.Cecil;

public class UIHero : Singleton<UIHero>
{
    public GameObject[] listHero;
    public Button btnSelect;
    public Button btnBuy;
    public TextMeshProUGUI txtPrice;

    public Button btnEvolve;
    public Button btnBack;
    public GameObject imgAvatar;

    public RectTransform pnEvolve;

    public GameObject bar;
    public GameObject tabInventory;

    public TextMeshProUGUI txtHeroName;
    public TextMeshProUGUI txtHeroSkillDetail;
    public Image skillImage;

    public TextMeshProUGUI txtAlibity_1;
    public TextMeshProUGUI txtAlibity_2;
    public TextMeshProUGUI txtAlibity_3;
    public TextMeshProUGUI txtAlibity_4;
    public TextMeshProUGUI txtAlibity_5;
    public TextMeshProUGUI txtAlibity_6;

    private int curHeroID;
    private int cacheId;

    public Button panelBtnClose;
    public Button panelBtnEvolve;
    public GameObject scrollview_evol;
    public List<GameObject> listHeroEvolve;
    public List<GameObject> listHeroAvatar;
    public List<GameObject> listHeroBackGlow;
    public GameObject EvolRequire;

    public GameObject[] evolItem;
    public TextMeshProUGUI goldRequire;

    public TextMeshProUGUI txtCurLevel;

    public List<TextMeshProUGUI> txtAlibityBefore;
    public List<TextMeshProUGUI> txtLevelOnScrollVew;

    int currentEvol = 0;
    Animator evolAnimator;

    public List<TextMeshProUGUI> txtLevelCard;
    public List<TextMeshProUGUI> txtLevelProgessCard;
    public List<Slider> slLevelCard;

    private Sprite[] heroesSprite;
    private Sprite[] imageSprites;
    private MyHeroes myHeroes;
    private int requireStone = 0;
    private bool isEvolve = true;
    // Start is called before the first frame update
    void Start()
    {
        heroesSprite = Resources.LoadAll<Sprite>("UI/Icons/Monster");
        imageSprites = Resources.LoadAll<Sprite>("Contents/Skill");
        if (!PlayerPrefs.HasKey("HeroesPick"))
        {
            PlayerPrefs.SetInt("HeroesPick", 10);
            curHeroID = PlayerPrefs.GetInt("HeroesPick");
            curHeroID = HeroesDatabase.Instance.getCurrentHero(curHeroID).Id;
        }
        else
        {
            curHeroID = PlayerPrefs.GetInt("HeroesPick");
        }
        initUIHero();
        onClickCard(HeroesDatabase.Instance.fetchMyHeroes(curHeroID));


        btnSelect.onClick.AddListener(() => selectHero());
        btnBuy.onClick.AddListener(() => buyHero());
        btnBack.onClick.AddListener(() => backToInventory());
        btnEvolve.onClick.AddListener(() => openEvolvePanel());
        panelBtnClose.onClick.AddListener(() => closeEvolvePanel());
        panelBtnEvolve.onClick.AddListener(() => evolutionAndLevelUpHero());

        evolAnimator = pnEvolve.GetComponent<Animator>();

        //maskBtnBuyGold.SetActive(true);
        //maskBtnBuyDiamond.SetActive(true);
    }
    public Sprite getSpriteHeroes(int id)
    {
        int index = HeroesDatabase.Instance.fetchHeroesIndex(id);
        return heroesSprite[index];
    }
    public void updateCacheSelection(int id)
    {
        cacheId = id;
    }

    private void initUIHero()
    {

        for (int i = 1; i <= 12; i++)
        {
            listHero[i - 1].GetComponent<CharacterCard>().initData(HeroesDatabase.Instance.getCurrentHero(i));
            if (i == curHeroID / 10)
            {
                listHero[i - 1].GetComponent<CharacterCard>().choosedHero(true);
            }
        }
    }

    public void onClickCard(MyHeroes data)
    {
        for (int i = 1; i <= 12; i++)
        {
            listHero[i - 1].GetComponent<CharacterCard>().selectedHeroes(false);
        }
        listHero[data.Id/10 -1].GetComponent<CharacterCard>().selectedHeroes(true);

        cacheId = data.Id;
        txtHeroName.text = data.Name;
        myHeroes = data;

        if (data.Level == 0)
        {
            txtPrice.text = "<sprite=5> " + StaticInfo.costHeroes[cacheId / 10].ToString();
        }
        txtHeroSkillDetail.text = data.Skill.ToString();

        txtAlibity_1.text = data.Atk.ToString();
        txtAlibity_2.text = data.Hp.ToString();
        txtAlibity_3.text = data.Armour.ToString();
        txtAlibity_4.text = data.Speed.ToString();
        txtAlibity_5.text = data.Crit.ToString();
        txtAlibity_6.text = data.Move.ToString();

        skillImage.sprite = imageSprites[cacheId/10 - 1];
        handleButton(data);

        foreach (Transform child in imgAvatar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + data.Id.ToString()) as GameObject, imgAvatar.transform);
        monster.transform.localPosition = new Vector3(0, 0, 0);
        monster.transform.localScale = new Vector3(monster.transform.localScale.x * 300, monster.transform.localScale.y * 300, monster.transform.localScale.z * 300);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
    }

    public void selectHero()
    {
        PlayerPrefs.SetInt("HeroesPick", cacheId);
        curHeroID = cacheId;
        btnSelect.gameObject.SetActive(false);
        for (int i = 1; i <= 12; i++)
        {
            if (i == curHeroID/10)
            {
                listHero[i - 1].GetComponent<CharacterCard>().choosedHero(true);
            }
            else
            {
                listHero[i - 1].GetComponent<CharacterCard>().choosedHero(false);
            }
        }
    }


    public void backToInventory()
    {
        PlayerPrefs.SetInt("HeroesPick", curHeroID);
        onClickCard(HeroesDatabase.Instance.fetchMyHeroes(curHeroID));

        UIController.Instance.enableSwipe = true;
        tabInventory.SetActive(true);
        bar.SetActive(true);
        UIInventory.Instance.initData(curHeroID);
        InventoryController.Instance.initItemData();

        gameObject.SetActive(false);
    }

    void handleButton(MyHeroes data)
    {
        if (data.Level > 0)
        {
            if (PlayerPrefs.GetInt("HeroesPick") == cacheId)
            {
                btnSelect.gameObject.SetActive(false);

            }
            else
            {
                btnSelect.gameObject.SetActive(true);
            }
            btnBuy.gameObject.SetActive(false);
            //btnSelect.gameObject.SetActive(true);
            btnEvolve.gameObject.SetActive(true);
        }
        else
        {
            btnBuy.gameObject.SetActive(true);
            btnSelect.gameObject.SetActive(false);
            btnEvolve.gameObject.SetActive(false);
        }
    }

    void buyHero()
    {
        if (HeroesDatabase.Instance.buyHeroes(cacheId))
        {
            MyHeroes data = HeroesDatabase.Instance.fetchMyHeroes(cacheId);
            initUIHero();
            handleButton(data);
        }
        else
        {
            // khong du tien
        }
    }

    void openEvolvePanel()
    {
        curHeroID = cacheId;
        imgAvatar.SetActive(false);
        pnEvolve.DOAnchorPos(new Vector2(0, 0), 0.25f);
        initDataEvolve();
    }

    void initDataEvolve()
    {
        panelBtnEvolve.gameObject.SetActive(true);
        List<MyHeroes> listhero = HeroesDatabase.Instance.fetchAllEvolveHero(curHeroID);
        currentEvol = curHeroID % 10 + 1;

        for (int i = 0; i < 5; i++)
        {
            listHeroEvolve[i].SetActive(false);
        }
        for (int i = 0; i < listhero.Count; i++)
        {
            listHeroEvolve[i].SetActive(true);
            listHeroBackGlow[i].SetActive(false);
            foreach (Transform child in listHeroAvatar[i].transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + listhero[i].Id.ToString()) as GameObject, listHeroAvatar[i].transform);
            monster.transform.localPosition = new Vector3(0, 0, 0);
            monster.transform.localScale = new Vector3(monster.transform.localScale.x * 300, monster.transform.localScale.y * 300, monster.transform.localScale.z * 300);
            monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
        }

        listHeroBackGlow[currentEvol - 1].SetActive(true);

        scrollview_evol.GetComponent<RectTransform>().anchoredPosition = new Vector2(StaticInfo.evolLocation[currentEvol - 1], 0);

        // Item can tien hoa
        ItemInventory stoneEvolve;
        requireStone = HeroesDatabase.Instance.getEvolveStone(curHeroID);
        int invenStone = ItemDatabase.Instance.fetchInventoryById(listhero[0].Type).Slot;
        stoneEvolve = ItemDatabase.Instance.getItemObject(listhero[0].Type, requireStone, 1);
        evolItem[0].GetComponent<ItemInflate>().InitData(stoneEvolve);
        evolItem[0].GetComponent<ItemInflate>().setTextSlot(invenStone, requireStone);

        ItemInventory shardEvolve;
        int invenShard = ItemDatabase.Instance.fetchInventoryById(listhero[0].Id / 10 + 100).Slot;
        shardEvolve = ItemDatabase.Instance.getItemObject(listhero[0].Id / 10 + 100, requireStone, 1);
        evolItem[1].GetComponent<ItemInflate>().InitData(shardEvolve);
        evolItem[1].GetComponent<ItemInflate>().setTextSlot(invenShard, requireStone);

        int goldRequireEvolve = 5000;
        goldRequire.text = goldRequireEvolve.ToString();
        if (goldRequireEvolve > UserDatabase.Instance.getUserData().Gold) { goldRequire.color = Color.red; };

        panelBtnEvolve.interactable = checkRequired();

        MyHeroes data_before = HeroesDatabase.Instance.fetchMyHeroes(curHeroID);
        txtAlibityBefore[0].text = data_before.Atk.ToString();
        txtAlibityBefore[1].text = data_before.Hp.ToString();
        txtAlibityBefore[2].text = data_before.Armour.ToString();
        txtAlibityBefore[3].text = data_before.Speed.ToString();
        txtAlibityBefore[4].text = data_before.Crit.ToString();
        txtAlibityBefore[5].text = data_before.Move.ToString();

    }

    void evolutionAndLevelUpHero()
    {
        if (isEvolve)
        {
            if (checkRequired())
            {
                isEvolve = false;
                StartCoroutine(enableButton());
                //StartCoroutine(AlibityChange());

                ItemDatabase.Instance.reduceItemSlotById(myHeroes.Type, requireStone);
                ItemDatabase.Instance.reduceItemSlotById(myHeroes.Id / 10 + 100, requireStone);
                UserDatabase.Instance.reduceMoney(5000, 0);
                InventoryController.Instance.initItemData();
                InventoryController.Instance.initMaterial();
                curHeroID++;
                HeroesDatabase.Instance.unlockHero(curHeroID);
                
                PlayerPrefs.SetInt("HeroesPick", curHeroID);
                StartCoroutine(runAnimEvolveAndLevelUp());
            }
            else
            {
                Debug.Log("khong du do");
            }
        }
    }

    IEnumerator enableButton()
    {
        yield return new WaitForSeconds(5f);
        isEvolve = true;
    }
    bool checkRequired()
    {
        int invenStone = ItemDatabase.Instance.fetchInventoryById(myHeroes.Type).Slot;
        int invenShard = ItemDatabase.Instance.fetchInventoryById(myHeroes.Id / 10 + 100).Slot;
        int goldRequireEvolve = 5000;

        if (invenStone < requireStone) { return false; }
        if (invenShard < requireStone) { return false; }
        if (UserDatabase.Instance.getUserData().Gold < goldRequireEvolve)
        {
            return false;
        }
        if(!HeroesDatabase.Instance.canEvolve(curHeroID))
        {
            return false;
        }
        return true;
    }

    public void closeEvolvePanel()
    {
        pnEvolve.DOAnchorPos(new Vector2(0, 3000), 0.25f);
        StartCoroutine(waitToActiveAvatar());
    }

    IEnumerator waitToActiveAvatar()
    {
        yield return new WaitForSeconds(0.15f);
        imgAvatar.SetActive(true);
        onClickCard(HeroesDatabase.Instance.fetchMyHeroes(curHeroID));
    }

    IEnumerator runAnimEvolveAndLevelUp()
    {
        evolAnimator.SetTrigger("Evolve");

        panelBtnEvolve.gameObject.SetActive(false);
        // anim tang chi so
        txtAlibityBefore[0].GetComponent<Animator>().SetTrigger("LevelUp");
        txtAlibityBefore[1].GetComponent<Animator>().SetTrigger("LevelUp");
        txtAlibityBefore[2].GetComponent<Animator>().SetTrigger("LevelUp");
        txtAlibityBefore[3].GetComponent<Animator>().SetTrigger("LevelUp");
        txtAlibityBefore[4].GetComponent<Animator>().SetTrigger("LevelUp");
        txtAlibityBefore[5].GetComponent<Animator>().SetTrigger("LevelUp");

        yield return new WaitForSeconds(1f);

        initDataEvolve();
        initUIHero();
    }
}



