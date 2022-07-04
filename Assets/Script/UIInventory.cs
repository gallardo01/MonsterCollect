using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventory : MonoBehaviour
{
    public Button btnChange;
    public GameObject tabHero;
    public GameObject bar;
    public GameObject tabInventory;
    public GameObject imgAvatar;

    public int curHeroId = 1;


    void Start()
    {
        initData(curHeroId);
        btnChange.onClick.AddListener(() => swapToHero());
    }

    void initData(int curHeroId)
    {
        HeroesData data = HeroesDatabase.Instance.getCurrentHero(curHeroId);

        foreach (Transform child in imgAvatar.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        GameObject monster = Instantiate(Resources.Load("Prefabs/Heroes/no." + data.Id.ToString()) as GameObject, imgAvatar.transform);
        monster.transform.localPosition = new Vector3(0, 0, 0);
        monster.transform.localScale = new Vector3(100, 100, 100);
        monster.GetComponent<DragonBones.UnityArmatureComponent>().animation.Play("idle");

    }

    void swapToHero()
    {
        tabHero.SetActive(true);
        tabInventory.SetActive(false);
        bar.SetActive(false);
    }

}
