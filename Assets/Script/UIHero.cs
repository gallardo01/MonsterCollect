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
    public Image imgAvatar;

    public TextMeshProUGUI txtHeroName;


    int curHero;

    // Start is called before the first frame update
    void Start()
    {
        initUIHero();

    }

    private void initUIHero()
    {
        for (int i = 1; i <= 12; i++)
        {
            listHero[i].GetComponent<CharacterCard>().initData(HeroesDatabase.Instance.getCurrentHero(i));
        }
    }



}
