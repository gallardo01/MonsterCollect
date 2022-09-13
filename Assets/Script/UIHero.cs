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

    public Button panelBtnClose;
    public Button panelBtnEvolve;
    public GameObject scrollview_evol;
    public List<GameObject> listHeroEvolve;
    public List<GameObject> listHeroAvatar;
    public List<GameObject> listHeroBackGlow;
    public GameObject EvolRequire;
    public List<TextMeshProUGUI> textEvolRequire;
    int currentEvol = 0;

    Animator evolAnimator;



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

        txtHeroName.text = data.Name;
        txtHeroSkillDetail.text = StaticInfo.skillDetail[(cacheId / 10) - 1];

        txtAlibity_1.text = data.Atk.ToString();
        txtAlibity_2.text = data.Hp.ToString();
        txtAlibity_3.text = data.Armour.ToString();
        txtAlibity_4.text = data.Speed.ToString();
        txtAlibity_5.text = data.XpGain.ToString();
        txtAlibity_6.text = data.GoldGain.ToString();

        
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
        if (data.Unlock == 1)
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

        if (data.Unlock == 0)
        {
            return;
        }

        curHeroID = cacheId;
        imgAvatar.SetActive(false);
        pnEvolve.DOAnchorPos(new Vector2(0, 0), 0.25f);

        initDataEvolve();
    }

    void initDataEvolve()
    {
        List<HeroesData> listhero =  HeroesDatabase.Instance.fetchAllEvolveHero(curHeroID);

        //if (currentEvol >= listhero.Count - 1)
        //{
        //    EvolRequire.SetActive(false);
        //}
        //else
        //{
        //    EvolRequire.SetActive(true);
        //}



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
        for (int i = listhero.Count-1; i >=0 ; i--)
        {
            if (listhero[i].Unlock == 1)
            {
                listHeroBackGlow[i].SetActive(true);
                currentEvol = i;
                break;
            }
        }

        Debug.Log("cur evol "+ currentEvol);
        scrollview_evol.GetComponent<RectTransform>().localPosition = new Vector3(StaticInfo.evolLocation[currentEvol], 749, 0);

        for (int i = 0; i < 3; i++)
        {
            textEvolRequire[i].SetText(ItemDatabase.Instance.fetchInventoryById(i + 5).Slot.ToString() + "/" + StaticInfo.evolveLevel[currentEvol, i].ToString());
        }
        textEvolRequire[3].SetText(StaticInfo.evolveLevel[currentEvol,3].ToString());

        if (canEvolve())
        {
            panelBtnEvolve.gameObject.SetActive(true);
        }
        else
        {
            panelBtnEvolve.gameObject.SetActive(false);

        }


    }

    void evolutionHero()
    {
        if (canEvolve())
        {
            //run anim
            StartCoroutine(runAnimEvolve());

            
        }
        else
        {
            Debug.Log("khong co du do");

        }
    }

    bool canEvolve()
    {
        if (HeroesDatabase.Instance.fetchHeroesData(curHeroID).Unlock == 0)
        {
            return false;
        }
        List<HeroesData> listhero = HeroesDatabase.Instance.fetchAllEvolveHero(curHeroID);
        if (currentEvol >= listhero.Count-1)
        {
            return false;
        }

        for (int i = 0; i < 3; i++)
        {
            //if(ItemDatabase.Instance.fetchInventoryById(i + 5).Slot < StaticInfo.evolveLevel[currentEvol, i])
            if(!ItemDatabase.Instance.canReduceItemSlotEvol(i+5, StaticInfo.evolveLevel[currentEvol, i]))
            {
                return false;
            }
        }
        UserData database = UserDatabase.Instance.getUserData();
        if (database.Gold < StaticInfo.evolveLevel[currentEvol, 3])
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

    IEnumerator runAnimEvolve()
    {
        evolAnimator.SetTrigger("Evolve");
        yield return new WaitForSeconds(2f);

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
    }
}



