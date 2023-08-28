using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.SoundManagerNamespace;

public class UIInventory : Singleton<UIInventory>
{
    public Button btnChange;
    public GameObject tabHero;
    public GameObject bar;
    public GameObject tabInventory;
    public GameObject imgAvatar;

    private int curHeroId = 0;


    void Start()
    {
        if (!PlayerPrefs.HasKey("HeroesPick"))
        {
            PlayerPrefs.SetInt("HeroesPick", 10);
            curHeroId = PlayerPrefs.GetInt("HeroesPick");
        }
        else
        {
            curHeroId = PlayerPrefs.GetInt("HeroesPick");
        }
        initData(curHeroId);
        btnChange.onClick.AddListener(() => swapToHero());
    }

    public void LoadData()
    {
        initData(curHeroId);
    }

    public void initData(int curHeroId)
    {
        MyHeroes data = HeroesDatabase.Instance.fetchMyHeroes(curHeroId);
        foreach (Transform child in imgAvatar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + data.Id.ToString()) as GameObject, imgAvatar.transform);
        monster.transform.localPosition = new Vector3(0, 0, 0);
        monster.transform.localScale = new Vector3(monster.transform.localScale.x * 100, monster.transform.localScale.y * 100, monster.transform.localScale.z * 100);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");
    }

    void swapToHero()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        UIController.Instance.enableSwipe = false;
        tabHero.SetActive(true);
        tabInventory.SetActive(false);
        bar.SetActive(false);
    }

}
