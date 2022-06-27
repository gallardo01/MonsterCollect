using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIHero : MonoBehaviour
{
    public Button[] listHero;
    public Button btnSelect;
    public Image imgAvatar;

    public TextMeshProUGUI txtHeroName;


    int curHero;

    // Start is called before the first frame update
    void Start()
    {
        curHero = 1;
        //imgAvatar.sprite = listHero[curHero-1].image.sprite;
        //txtHeroName.text = "pokemon so " + (curHero);

        for (int i = 0; i < 12; i++)
        {
            int n = i;

            listHero[n].onClick.AddListener(() => selectHero(n));
        }

        btnSelect.onClick.AddListener(() => saveHero());
    }

    void selectHero(int index)
    {
        //int temp = 0;
        //switch (index)
        //{
        //    case 0: temp = 1; break;
        //    case 1: temp = 4; break;
        //    case 2: temp = 7; break;
        //    case 3: temp = 10; break;
        //    case 4: temp = 13; break;
        //    case 5: temp = 14; break;
        //    case 6: temp = 16; break;
        //    case 7: temp = 17; break;
        //    case 8: temp = 19; break;
        //    case 9: temp = 22; break;
        //    case 10: temp = 25; break;
        //    case 11: temp = 28; break;
        //    default: temp = 1; break;
        //}
        //imgAvatar.sprite = Resources.Load<Sprite>("UI/Icons/Monster/"+ temp);
        imgAvatar.sprite = listHero[index].image.sprite;
        txtHeroName.text = "pokemon so " + (index + 1);

        curHero = index +1;
    }

    void saveHero()
    {
        Debug.Log("da chon pkm so " + curHero);
    }

}
