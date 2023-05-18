using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterCard : MonoBehaviour
{
    public Image imgHero;
    public Image[] stars;
    public Image backGlow;
    public TextMeshProUGUI heroName;
    public Button button;
    public GameObject locker;
    public GameObject selected;
    public GameObject choosed;
    public Image typeHero;
    public Slider sliderObj;
    public TextMeshProUGUI shardText;

    private MyHeroes heroesData;


    void Start()
    {
        button.onClick.AddListener(() => onClickHeroData());
    }
    private void onClickHeroData()
    {
        UIHero.Instance.onClickCard(heroesData);
        //selected.SetActive(true);
    }

    public void chooseHeroes(bool active)
    {
        choosed.SetActive(active);
    }
    public void selectHeroes(bool active)
    {
        selected.SetActive(active);
    }
    public void initData(MyHeroes data)
    {
        heroesData = data;
        heroName.text = data.Name;
        if (data.Type == 1)
        {
            gameObject.GetComponent<Image>().color = Color.red;
            backGlow.color = Color.white;
        } else if(data.Type == 2)
        {
            gameObject.GetComponent<Image>().color = Color.yellow;
            backGlow.color = Color.white;
        }
        else if(data.Type == 3)
        {
            gameObject.GetComponent<Image>().color = Color.blue;
            backGlow.color = Color.white;
        }
        else
        {
            gameObject.GetComponent<Image>().color = Color.green;
            backGlow.color = Color.white;
        }
        imgHero.sprite = UIHero.Instance.getSpriteHeroes(data.Id);
        typeHero.sprite = Resources.Load<Sprite>("UI/Icons/Type/" + data.Type.ToString());
        if (data.Level > 0)
        {
            locker.SetActive(false);
        }
        else
        {
            locker.SetActive(true);
        }

        if (data.Level > 0)
        {
            int shardRequire = HeroesDatabase.Instance.getEvolveStone(data.Id);
            int shardInv = ItemDatabase.Instance.fetchInventoryById(100 + data.Id / 10).Slot;

            sliderObj.value = (float)shardInv / shardRequire;
            shardText.text = shardInv.ToString() + "/" + shardRequire;
        }
        else
        {
            sliderObj.value = 0f;
            shardText.text = ItemDatabase.Instance.fetchInventoryById(100 + data.Id / 10).Slot.ToString() + "/0";
        }
    }

}
