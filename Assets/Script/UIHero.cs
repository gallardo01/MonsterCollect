﻿using System.Collections;
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
    public GameObject imgAvatar;

    public GameObject bar;
    public GameObject tabInventory;

    public TextMeshProUGUI txtHeroName;

    public TextMeshProUGUI txtAlibity_1;
    public TextMeshProUGUI txtAlibity_2;
    public TextMeshProUGUI txtAlibity_3;
    public TextMeshProUGUI txtAlibity_4;
    public TextMeshProUGUI txtAlibity_5;
    public TextMeshProUGUI txtAlibity_6;

    private int curHeroID;
    public int cacheId;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("HeroesPick"))
        {
            PlayerPrefs.SetInt("HeroesPick", 10);
        }
        else
        {
            curHeroID = PlayerPrefs.GetInt("HeroesPick");
        }
        initUIHero();
        onClickCard(HeroesDatabase.Instance.fetchHeroesData(curHeroID));
        btnSelect.onClick.AddListener(() => selectHero());
        btnBuy.onClick.AddListener(() => buyHero());
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
        txtHeroName.text = data.Name;

        txtAlibity_1.text = "Atk: " + data.Atk.ToString();
        txtAlibity_2.text = "HP: " + data.Hp.ToString();
        txtAlibity_3.text = "Arm: " + data.Armour.ToString();
        txtAlibity_4.text = "Spd: " + data.Speed.ToString();
        txtAlibity_5.text = "Exp: " + data.XpGain.ToString();
        txtAlibity_6.text = "Gold: " + data.GoldGain.ToString();

        curHeroID = data.Id;
        cacheId = data.Id;
        handleButton(data);

        foreach (Transform child in imgAvatar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + data.Id.ToString()) as GameObject, imgAvatar.transform);
        //monster.transform.parent = imgAvatar.transform;
        //monster.transform.parent = monster.transform;
        monster.transform.localPosition = new Vector3(0, 0, 0);
        monster.transform.localScale = new Vector3(300, 300, 300);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
    }

    void selectHero()
    {
        PlayerPrefs.SetInt("HeroesPick", cacheId);
        backToInventory();
    }


    void backToInventory()
    {
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
        }
        else
        {
            btnBuy.gameObject.SetActive(true);
            btnSelect.gameObject.SetActive(false);
        }
    }

    void buyHero()
    {
        if (HeroesDatabase.Instance.buyHeroes(cacheId))
        {
            HeroesData data = HeroesDatabase.Instance.fetchHeroesData(cacheId);
            initUIHero();
            handleButton(data);
        } else
        {
            // khong du tien
        }
    }

}


