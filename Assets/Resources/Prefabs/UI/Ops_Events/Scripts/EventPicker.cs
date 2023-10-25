using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EventPicker : MonoBehaviour
{
    public Button btnBuy;

    public Button btnCard;
    public Button btnCard_1;
    public Button btnCard_2;
    public Button btnCard_3;


    private Animator btnCard_AC;
    private Animator btnCard_AC_1;
    private Animator btnCard_AC_2;
    private Animator btnCard_AC_3;

    private bool canOpen = true;

    // Start is called before the first frame update
    void Start()
    {
        btnCard_AC = btnCard.GetComponent<Animator>();
        btnCard_AC_1 = btnCard_1.GetComponent<Animator>();
        btnCard_AC_2 = btnCard_2.GetComponent<Animator>();
        btnCard_AC_3 = btnCard_3.GetComponent<Animator>();



        btnBuy.onClick.AddListener(() =>
            Shuffle()
        );
        btnCard.onClick.AddListener(() =>
            OpenCard(0)
        );
        btnCard_1.onClick.AddListener(() =>
            OpenCard(1)
        );
        btnCard_2.onClick.AddListener(() =>
            OpenCard(2)
        );
        btnCard_3.onClick.AddListener(() =>
            OpenCard(3)
        );
    }

    void Shuffle()
    {
        btnBuy.gameObject.SetActive(false);
        btnCard_AC.SetTrigger("Shuffle");
        btnCard_AC_1.SetTrigger("Shuffle");
        btnCard_AC_2.SetTrigger("Shuffle");
        btnCard_AC_3.SetTrigger("Shuffle");

    }

    void OpenCard(int card)
    {
        if (canOpen)
        {
            switch (card)
            {
                case 0:
                    btnCard_AC.SetTrigger("Open");
                    break;
                case 1:
                    btnCard_AC_1.SetTrigger("Open");
                    break;
                case 2:
                    btnCard_AC_2.SetTrigger("Open");
                    break;
                case 3:
                    btnCard_AC_3.SetTrigger("Open");
                    break;

            }
            canOpen = false;
        }
        
    }

}
