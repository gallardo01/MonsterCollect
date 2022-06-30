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
    public GameObject imgAvatar;
    public GameObject bar;
    public GameObject tabInventory;

    public TextMeshProUGUI txtHeroName;

    public int curHeroID = 10;

    // Start is called before the first frame update
    void Start()
    {
        initUIHero();
        onClickCard(HeroesDatabase.Instance.fetchHeroesData(curHeroID));
        btnSelect.onClick.AddListener(() => selectHero());
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
        backToInventory();
    }

    void backToInventory()
    {
        tabInventory.SetActive(true);
        gameObject.SetActive(false);
        bar.SetActive(true);
    }

}


