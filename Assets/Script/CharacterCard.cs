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
    public GameObject selected;
    public GameObject choosed;
    public Image typeHero;
    public Slider sliderObj;
    public TextMeshProUGUI shardText;
    public TextMeshProUGUI level;

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
        imgHero.sprite = UIHero.Instance.getSpriteHeroes(data.Id);
        typeHero.sprite = Resources.Load<Sprite>("UI/Icons/Type/" + data.Type.ToString());
        if (data.Level > 0)
        {
            locker.SetActive(true);
            imgHero.color = new Color(152f / 255f, 152f / 255f, 152f / 255f);
        }
        else
        {
            locker.SetActive(false);
            imgHero.color = Color.white;
        }

        if (data.Level > 0)
        {
            int shardRequire = 2 * data.Level;
            int shardInv = ItemDatabase.Instance.fetchInventoryById(100 + data.Id / 10).Slot;

            sliderObj.value = (float)shardInv / shardRequire;
            level.text = data.Level.ToString();
            shardText.text = shardInv.ToString() + " " + shardRequire;
        }
        else
        {
            sliderObj.value = 0f;
            level.text = "0";
            shardText.text = ItemDatabase.Instance.fetchInventoryById(100 + data.Id / 10).Slot.ToString() + "/0";
        }
    }

}
