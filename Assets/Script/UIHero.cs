using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIHero : Singleton<UIHero>
{
    public GameObject[] listHero;
    public Button btnSelect;
    public Button btnBuy;
    public Button btnEvolve;
    public Button btnBack;
    public GameObject imgAvatar;

    public RectTransform pnEvolve;

    public GameObject bar;
    public GameObject tabInventory;

    public TextMeshProUGUI txtHeroName;
    public TextMeshProUGUI txtHeroSkillDetail;

    public TextMeshProUGUI txtAlibity_1;
    public TextMeshProUGUI txtAlibity_2;
    public TextMeshProUGUI txtAlibity_3;
    public TextMeshProUGUI txtAlibity_4;
    public TextMeshProUGUI txtAlibity_5;
    public TextMeshProUGUI txtAlibity_6;

    private int curHeroID;
    public int cacheId;

    public List<GameObject> selected;

    public Button panelBtnClose;
    public Button panelBtnEvolve;
    public GameObject scrollview_evol;
    public List<GameObject> listHeroEvolve;
    public List<GameObject> listHeroAvatar;
    public List<GameObject> listHeroBackGlow;
    public GameObject EvolRequire;
    public List<TextMeshProUGUI> textEvolRequire;

    public TextMeshProUGUI txtCurrentLevel;
    public TextMeshProUGUI txtNextLevel;
    public List<TextMeshProUGUI> txtAlibityBefore;
    public List<TextMeshProUGUI> txtAlibityAfter;
    public GameObject groupPanelAlibityAfter;


    int currentEvol = 0;



    Animator evolAnimator;
    bool can_evolve;


    public GameObject maskBtnBuyGold;
    public GameObject maskBtnBuyDiamond;


    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("HeroesPick"))
        {
            PlayerPrefs.SetInt("HeroesPick", 10);
            curHeroID = PlayerPrefs.GetInt("HeroesPick");

        }
        else
        {
            curHeroID = PlayerPrefs.GetInt("HeroesPick");
        }
        initUIHero();
        onClickCard(HeroesDatabase.Instance.fetchHeroesData(curHeroID));


        btnSelect.onClick.AddListener(() => selectHero());
        btnBuy.onClick.AddListener(() => buyHero());
        btnBack.onClick.AddListener(() => backToInventory());
        btnEvolve.onClick.AddListener(() => openEvolvePanel());
        panelBtnClose.onClick.AddListener(() => closeEvolvePanel());
        panelBtnEvolve.onClick.AddListener(() => evolutionHero());

        evolAnimator = pnEvolve.GetComponent<Animator>();

        maskBtnBuyGold.SetActive(true);
        maskBtnBuyDiamond.SetActive(true);
    }

    public void updateCacheSelection(int id)
    {
        cacheId = id;
    }

    private void initUIHero()
    {
        for (int i = 1; i <= 12; i++)
        {
            listHero[i].GetComponent<CharacterCard>().initData(HeroesDatabase.Instance.getCurrentHero(i));
        }
    }

    public void onClickCard(HeroesData data)
    {
        //curHeroID = data.Id;
        cacheId = data.Id;

        for (int i = 1; i <= 12; i++)
        {
            selected[i].SetActive(false);
        }
        selected[cacheId/10].SetActive(true);


        txtHeroName.text = data.Name;
        //txtHeroSkillDetail.text = StaticInfo.skillDetail[(cacheId / 10) - 1];
        txtHeroSkillDetail.text = data.Skill.ToString();

        txtAlibity_1.text = data.Atk.ToString();
        txtAlibity_2.text = data.Hp.ToString();
        txtAlibity_3.text = data.Armour.ToString();
        txtAlibity_4.text = data.Speed.ToString();
        txtAlibity_5.text = data.Crit.ToString();
        txtAlibity_6.text = data.Spell.ToString();

        
        handleButton(data);

        foreach (Transform child in imgAvatar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + data.Id.ToString()) as GameObject, imgAvatar.transform);
        //monster.transform.parent = imgAvatar.transform;
        //monster.transform.parent = monster.transform;
        monster.transform.localPosition = new Vector3(0, 0, 0);
        monster.transform.localScale = new Vector3(monster.transform.localScale.x * 300, monster.transform.localScale.y * 300, monster.transform.localScale.z * 300);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
    }

    public void selectHero()
    {
        PlayerPrefs.SetInt("HeroesPick", cacheId);
        curHeroID = cacheId;    
        //backToInventory();
    }


    public void backToInventory()
    {
        maskBtnBuyGold.SetActive(false);
        maskBtnBuyDiamond.SetActive(false);

        PlayerPrefs.SetInt("HeroesPick", curHeroID);
        onClickCard(HeroesDatabase.Instance.fetchHeroesData(curHeroID));

        UIController.Instance.enableSwipe = true;
        tabInventory.SetActive(true);
        bar.SetActive(true);
        UIInventory.Instance.initData(curHeroID);
        gameObject.SetActive(false);
    }

    void handleButton(HeroesData data)
    {
        if (HeroesDatabase.Instance.isUnlock(data.Id))
        {
            btnBuy.gameObject.SetActive(false);
            btnSelect.gameObject.SetActive(true);
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
            HeroesData data = HeroesDatabase.Instance.fetchHeroesData(cacheId);
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
        can_evolve = true;

        // check unlocked
        HeroesData data = HeroesDatabase.Instance.fetchHeroesData(cacheId);

        //if (data.Unlock == 0)
        //{
        //    return;
        //}

        curHeroID = cacheId;
        imgAvatar.SetActive(false);
        pnEvolve.DOAnchorPos(new Vector2(0, 0), 0.25f);

        initDataEvolve();
    }

    void initDataEvolve()
    {
        List<HeroesData> listhero =  HeroesDatabase.Instance.fetchAllEvolveHero(curHeroID);
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
            monster.transform.localScale = new Vector3(monster.transform.localScale.x *300, monster.transform.localScale.y * 300, monster.transform.localScale.z *300);
            monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");

        }

        listHeroBackGlow[currentEvol-1].SetActive(true);

        //Debug.Log("cur evol "+ currentEvol);


        scrollview_evol.GetComponent<RectTransform>().localPosition = new Vector3(StaticInfo.evolLocation[currentEvol-1], 749, 0);


        //for (int i = 0; i < 3; i++)
        //{
        //    textEvolRequire[i].SetText(ItemDatabase.Instance.fetchInventoryById(i + 5).Slot.ToString() + "/" + StaticInfo.evolveLevel[currentEvol, i].ToString());
        //}
        //textEvolRequire[3].SetText(StaticInfo.evolveLevel[currentEvol, 3].ToString());

        //for (int i = 0; i < 3; i++)
        //{
        //    if (ItemDatabase.Instance.fetchInventoryById(i + 5).Slot < StaticInfo.evolveLevel[currentEvol, i])
        //    {
        //        textEvolRequire[i].color = Color.red;
        //    }
        //}
        //UserData database = UserDatabase.Instance.getUserData();

        //if (database.Gold < StaticInfo.evolveLevel[currentEvol, 3])
        //{
        //    textEvolRequire[3].color = Color.red;
        //}



        MyHeroes data_before = HeroesDatabase.Instance.fetchMyData(curHeroID);

        txtCurrentLevel.text = "Level "+ data_before.Level.ToString();
        txtAlibityBefore[0].text = data_before.Atk.ToString();
        txtAlibityBefore[1].text = data_before.Hp.ToString();
        txtAlibityBefore[2].text = data_before.Armour.ToString();
        txtAlibityBefore[3].text = data_before.Speed.ToString();
        txtAlibityBefore[4].text = data_before.Crit.ToString();
        txtAlibityBefore[5].text = data_before.Spell.ToString();

        if (data_before.Level < 25)
        {
            groupPanelAlibityAfter.SetActive(true);
            panelBtnEvolve.gameObject.SetActive(true);
            EvolRequire.SetActive(true);

            //panel thong tin
            txtNextLevel.text = "Level " + (data_before.Level+1).ToString();


            //if ((data_before.Atk * (data_before.Level * 5 + 100) / 100) > data_before.Atk)
            //{
            //    txtAlibityAfter[0].color = Color.green;
            //}
            //if ((data_before.Hp * (data_before.Level * 5 + 100) / 100) > data_before.Hp)
            //{
            //    txtAlibityAfter[1].color = Color.green;
            //}
            //if ((data_before.Armour * (data_before.Level * 5 + 100) / 100) > data_before.Armour)
            //{
            //    txtAlibityAfter[2].color = Color.green;
            //}
            //if ((data_before.Speed * (data_before.Level * 5 + 100) / 100) > data_before.Speed)
            //{
            //    txtAlibityAfter[3].color = Color.green;
            //}
            //if ((data_before.Crit * (data_before.Level * 5 + 100) / 100) > data_before.Crit)
            //{
            //    txtAlibityAfter[4].color = Color.green;
            //}
            //if ((data_before.Spell * (data_before.Level * 5 + 100) / 100) > data_before.Spell)
            //{
            //    txtAlibityAfter[5].color = Color.green;
            //}
            for (int i = 0; i < 6; i++)
            {
                txtAlibityAfter[i].color = Color.green;
            }

            
            txtAlibityAfter[0].text = (data_before.Atk * (data_before.Level * 5 + 100) / 100).ToString();
            txtAlibityAfter[1].text = (data_before.Hp * (data_before.Level * 5 + 100) / 100).ToString();
            txtAlibityAfter[2].text = (data_before.Armour * (data_before.Level * 5 + 100) / 100).ToString();
            txtAlibityAfter[3].text = (data_before.Speed * (data_before.Level * 5 + 100) / 100).ToString();
            txtAlibityAfter[4].text = (data_before.Crit * (data_before.Level * 5 + 100) / 100).ToString();
            txtAlibityAfter[5].text = (data_before.Spell * (data_before.Level * 5 + 100) / 100).ToString();

        }
        else
        {
            groupPanelAlibityAfter.SetActive(false);
            panelBtnEvolve.gameObject.SetActive(false);
            EvolRequire.SetActive(false);
        }

    }

    void evolutionHero()
    {
     

        int currentLevel = HeroesDatabase.Instance.fetchMyData(curHeroID).Level;
        HeroesDatabase.Instance.levelUpHero(curHeroID);

        if (currentLevel == 4 || currentLevel == 9 || currentLevel == 14 || currentLevel == 19 || currentLevel == 24)
        {
            // tien hoa hinh dang moi

            if (canEvolve() && can_evolve)
            {
                //run anim
                can_evolve = false;
                //HeroesDatabase.Instance.evolveHero(curHeroID);
                StartCoroutine(runAnimEvolve());
            }
            else
            {
                Debug.Log("khong co du do");
            }
        }

        initDataEvolve();

    }

    bool canEvolve()
    {
        //if (HeroesDatabase.Instance.fetchHeroesData(curHeroID).Unlock == 0)
        //{
        //    return false;
        //}
        List<HeroesData> listhero = HeroesDatabase.Instance.fetchAllEvolveHero(curHeroID);
        if (currentEvol >= listhero.Count-1)
        {
            return false;
        }

        //for (int i = 0; i < 3; i++)
        //{
        //    //if(ItemDatabase.Instance.fetchInventoryById(i + 5).Slot < StaticInfo.evolveLevel[currentEvol, i])
        //    if(!ItemDatabase.Instance.canReduceItemSlotEvol(i+5, StaticInfo.evolveLevel[currentEvol, i]))
        //    {
        //        return false;
        //    }
        //}
        //UserData database = UserDatabase.Instance.getUserData();
        //if (database.Gold < StaticInfo.evolveLevel[currentEvol, 3])
        //{
        //    return false;
        //}
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
        onClickCard(HeroesDatabase.Instance.fetchHeroesData(curHeroID));

    }

    IEnumerator runAnimEvolve()
    {
        evolAnimator.SetTrigger("Evolve");
        yield return new WaitForSeconds(1f);

        for (int i = 0; i < 3; i++)
        {
            ItemDatabase.Instance.reduceItemSlotById(i + 5, StaticInfo.evolveLevel[currentEvol, i]);
        }
        UserDatabase.Instance.reduceMoney(StaticInfo.evolveLevel[currentEvol, 3], 0);

        HeroesDatabase.Instance.evolveHero(curHeroID);
        Debug.Log("cur Hero ID " + curHeroID);

        curHeroID++;
        PlayerPrefs.SetInt("HeroesPick", curHeroID);
        initDataEvolve();
        initUIHero();

        can_evolve = true;
    }
}



