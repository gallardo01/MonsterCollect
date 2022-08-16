using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventory : Singleton<UIInventory>
{
    public Button btnChange;
    public GameObject tabHero;
    public GameObject bar;
    public GameObject tabInventory;
    public GameObject imgAvatar;

    public TextMeshProUGUI txtAlibity_1;
    public TextMeshProUGUI txtAlibity_2;
    public TextMeshProUGUI txtAlibity_3;
    public TextMeshProUGUI txtAlibity_4;
    public TextMeshProUGUI txtAlibity_5;
    public TextMeshProUGUI txtAlibity_6;

    private int curHeroId = 10;


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

    public void initData(int curHeroId)
    {
        HeroesData data = HeroesDatabase.Instance.fetchHeroesData(curHeroId);

        foreach (Transform child in imgAvatar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + data.Id.ToString()) as GameObject, imgAvatar.transform);
        monster.transform.localPosition = new Vector3(0, 0, 0);
        monster.transform.localScale = new Vector3(monster.transform.localScale.x * 100, monster.transform.localScale.y * 100, monster.transform.localScale.z * 100);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");

        HeroesData dataHero = HeroesDatabase.Instance.fetchHeroesData(curHeroId);

        txtAlibity_1.text = "Atk: " + dataHero.Atk.ToString();
        txtAlibity_2.text = "HP: " + dataHero.Hp.ToString();
        txtAlibity_3.text = "Arm: " + dataHero.Armour.ToString();
        txtAlibity_4.text = "Spd: " + dataHero.Speed.ToString();
        txtAlibity_5.text = "Exp: " + dataHero.XpGain.ToString();
        txtAlibity_6.text = "Gold: " + dataHero.GoldGain.ToString();
    }

    void swapToHero()
    {
        UIController.Instance.enableSwipe = false;
        tabHero.SetActive(true);
        tabInventory.SetActive(false);
        bar.SetActive(false);
    }

}
