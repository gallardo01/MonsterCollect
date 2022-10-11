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
    public Image heroShard;

    public TextMeshProUGUI txtCurrentLevel;
    public TextMeshProUGUI txtNextLevel;
    public List<TextMeshProUGUI> txtAlibityBefore;
    public List<TextMeshProUGUI> txtAlibityAfter;
    public GameObject groupPanelAlibityAfter;

    public List<TextMeshProUGUI> txtLevelOnScrollVew;

    int currentEvol = 0;



    Animator evolAnimator;


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
        panelBtnEvolve.onClick.AddListener(() => evolutionAndLevelUpHero());

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

        int currentLevel = HeroesDatabase.Instance.fetchMyData(curHeroID).Level;


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

        scrollview_evol.GetComponent<RectTransform>().localPosition = new Vector3(StaticInfo.evolLocation[currentEvol - 1], 749, 0);

        ////////
        ///


        int index_shard = HeroesDatabase.Instance.fetchMyData(curHeroID).Id / 10 + 100;

        heroShard.sprite = Resources.Load<Sprite>("Contents/Item/" + index_shard.ToString());

        textEvolRequire[0].SetText(ItemDatabase.Instance.fetchInventoryById(index_shard).Slot.ToString() + "/" + (currentLevel + 1).ToString());
        textEvolRequire[1].SetText(ItemDatabase.Instance.fetchInventoryById(1).Slot.ToString() + "/" + (currentLevel/3 + 3).ToString());
        textEvolRequire[2].SetText(ItemDatabase.Instance.fetchInventoryById(2).Slot.ToString() + "/" + (currentLevel/5 + 2).ToString());
        textEvolRequire[3].SetText(ItemDatabase.Instance.fetchInventoryById(3).Slot.ToString() + "/" + (currentLevel / 10 + 1).ToString());
        textEvolRequire[4].SetText((currentLevel * 200).ToString());

        //for (int i = 0; i < 3; i++)
        //{
        //    if (ItemDatabase.Instance.fetchInventoryById(i + 5).Slot < StaticInfo.evolveLevel[currentEvol, i])
        //    {
        //        textEvolRequire[i].color = Color.red;
        //    }
        //}

        if (ItemDatabase.Instance.fetchInventoryById(index_shard).Slot < (currentLevel + 1))
        {
            textEvolRequire[0].color = Color.red;
        }
        if (ItemDatabase.Instance.fetchInventoryById(1).Slot < (currentLevel / 3 + 3))
        {
            textEvolRequire[1].color = Color.red;
        }
        if (ItemDatabase.Instance.fetchInventoryById(2).Slot < (currentLevel / 5 + 2))
        {
            textEvolRequire[2].color = Color.red;
        }
        if (ItemDatabase.Instance.fetchInventoryById(3).Slot < (currentLevel / 10 + 1))
        {
            textEvolRequire[3].color = Color.red;
        }

        UserData database = UserDatabase.Instance.getUserData();
        if (database.Gold < currentLevel * 200)
        {
            textEvolRequire[4].color = Color.red;
        }

        //set text level
        if (currentLevel < 5)
        {
            txtLevelOnScrollVew[0].text = "Level " + currentLevel;
        }
        else
        {
            txtLevelOnScrollVew[0].text = "Level 1"; 
        }

        if (currentLevel >= 5 && currentLevel <10)
        {
            txtLevelOnScrollVew[1].text = "Level " + currentLevel;
        }
        else
        {
            txtLevelOnScrollVew[1].text = "Level 5";
        }
        if (currentLevel >= 10 && currentLevel < 15)
        {
            txtLevelOnScrollVew[2].text = "Level " + currentLevel;
        }
        else
        {
            txtLevelOnScrollVew[2].text = "Level 10";
        }
        if (currentLevel >= 15 && currentLevel < 20)
        {
            txtLevelOnScrollVew[3].text = "Level " + currentLevel;
        }
        else
        {
            txtLevelOnScrollVew[3].text = "Level 15";
        }
        if (currentLevel >= 20 && currentLevel < 25)
        {
            txtLevelOnScrollVew[4].text = "Level " + currentLevel;
        }
        else
        {
            txtLevelOnScrollVew[4].text = "Level 20";
        }

        if (true)
        {
            txtLevelOnScrollVew[listhero.Count - 1].text = "Level " + currentLevel;
        }

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

            if ((currentLevel == 4 || currentLevel == 9 || currentLevel == 14 || currentLevel == 19 || currentLevel == 24) && canEvolve())
            {
                HeroesData data_before_evole = HeroesDatabase.Instance.fetchHeroesData(curHeroID+1);

                txtAlibityAfter[0].text = (data_before_evole.Atk * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[1].text = (data_before_evole.Hp * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[2].text = (data_before_evole.Armour * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[3].text = (data_before_evole.Speed * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[4].text = (data_before_evole.Crit * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[5].text = (data_before_evole.Spell * ((data_before.Level) * 5 + 100) / 100).ToString();

                Debug.Log("elvo: "+data_before.Level);
                Debug.Log("elvo: " + curHeroID);
            }
            else
            {
                HeroesData data_before_evole_1 = HeroesDatabase.Instance.fetchHeroesData(curHeroID);

                txtAlibityAfter[0].text = (data_before_evole_1.Atk * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[1].text = (data_before_evole_1.Hp * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[2].text = (data_before_evole_1.Armour * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[3].text = (data_before_evole_1.Speed * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[4].text = (data_before_evole_1.Crit * ((data_before.Level) * 5 + 100) / 100).ToString();
                txtAlibityAfter[5].text = (data_before_evole_1.Spell * ((data_before.Level) * 5 + 100) / 100).ToString();

                Debug.Log("nor: " + data_before.Level);
                Debug.Log("nor: " + curHeroID); 
            }
        }
        else
        {
            groupPanelAlibityAfter.SetActive(false);
            panelBtnEvolve.gameObject.SetActive(false);
            EvolRequire.SetActive(false);
        }

    }

    void evolutionAndLevelUpHero()
    {
        //check requiement
        int currentLevel = HeroesDatabase.Instance.fetchMyData(curHeroID).Level;
        if (checkRequired(currentLevel))
        {
            HeroesDatabase.Instance.levelUpHero(curHeroID);

            if ((currentLevel == 4 || currentLevel == 9 || currentLevel == 14 || currentLevel == 19 || currentLevel == 24) && canEvolve())
            {
                // tien hoa hinh dang moi

                //run anim
                //HeroesDatabase.Instance.evolveHero(curHeroID);
                HeroesDatabase.Instance.evolveHero(curHeroID);
                curHeroID++;
                PlayerPrefs.SetInt("HeroesPick", curHeroID);

            }
            StartCoroutine(runAnimEvolveAndLevelUp());
        }
        else
        {
            Debug.Log("khong du do");
        }

        
    }

    bool canEvolve()
    {
        //if (HeroesDatabase.Instance.fetchHeroesData(curHeroID).Unlock == 0)
        //{
        //    return false;
        //}
        List<HeroesData> listhero = HeroesDatabase.Instance.fetchAllEvolveHero(curHeroID/10 *10);
 
        if (currentEvol >= listhero.Count)
        {
            return false;
        }

        return true;
    }
    
    bool checkRequired(int level)
    {
        int index_shard = HeroesDatabase.Instance.fetchMyData(curHeroID).Id / 10 + 100;
        //Heros shard
        if (!ItemDatabase.Instance.canReduceItemSlotEvol(index_shard, level + 1))
        {
            return false;
        }
        // moon stone
        if (!ItemDatabase.Instance.canReduceItemSlotEvol(1, level/3 + 3))
        {
            return false;
        }
        // sun stone
        if (!ItemDatabase.Instance.canReduceItemSlotEvol(2, level/5 + 2))
        {
            return false;
        }
        // element stone
        if (!ItemDatabase.Instance.canReduceItemSlotEvol(3, level/10 + 1))
        {
            return false;
        }
        UserData database = UserDatabase.Instance.getUserData();
        if (database.Gold < level *200)
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
        onClickCard(HeroesDatabase.Instance.fetchHeroesData(curHeroID));

    }

    IEnumerator runAnimEvolveAndLevelUp()
    {
        evolAnimator.SetTrigger("Evolve");
        yield return new WaitForSeconds(1f);

        //for (int i = 0; i < 3; i++)
        //{
        //    ItemDatabase.Instance.reduceItemSlotById(i + 5, StaticInfo.evolveLevel[currentEvol, i]);
        //}
        //UserDatabase.Instance.reduceMoney(StaticInfo.evolveLevel[currentEvol, 3], 0);


       
        initDataEvolve();
        initUIHero();
    }
}



