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

    private HeroesData heroesData;


    void Start()
    {
        button.onClick.AddListener(() => onClickElemonData());
    }
    private void onClickElemonData()
    {
        //InformationController.Instance.initData(elemonData);
    }

    public void initData(HeroesData data)
    {
        heroesData = data;

        string name = data.Name;

        heroName.text = name;
        imgHero.sprite = Resources.Load<Sprite>("UI/Icons/Monster/" + data.Id.ToString());


    }

}
