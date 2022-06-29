using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{
    public Image imgHero;
    public Image[] stars;
    public TextMeshProUGUI heroName;
    public Button button;
    public GameObject locker;


    private HeroesData heroesData;


    void Start()
    {
        button.onClick.AddListener(() => onClickElemonData());
    }
    private void onClickElemonData()
    {
        UIHero.Instance.onClickCard(heroesData);
    }

    public void initData(HeroesData data)
    {
        heroesData = data;

        string name = data.Name;
        heroName.text = name;
        imgHero.sprite = Resources.Load<Sprite>("UI/Icons/Monster/" + data.Id.ToString());
        if (data.Unlock == 0)
        {
            locker.SetActive(true);
            imgHero.color = new Color(152f / 255f, 152f / 255f, 152f / 255f);
        }
        else
        {
            locker.SetActive(false);
            imgHero.color = Color.white;

        }



    }

}
