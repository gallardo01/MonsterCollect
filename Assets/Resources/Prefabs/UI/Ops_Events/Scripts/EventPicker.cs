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

    public Button btnReset;
    public GameObject txtPick;

    private Animator btnCard_AC;
    private Animator btnCard_AC_1;
    private Animator btnCard_AC_2;
    private Animator btnCard_AC_3;

    private Animator mainAC;

    private bool canOpen = false;

    // Start is called before the first frame update
    void Start()
    {
        btnCard_AC = btnCard.GetComponent<Animator>();
        btnCard_AC_1 = btnCard_1.GetComponent<Animator>();
        btnCard_AC_2 = btnCard_2.GetComponent<Animator>();
        btnCard_AC_3 = btnCard_3.GetComponent<Animator>();

        mainAC = this.gameObject.GetComponent<Animator>();

        btnBuy.onClick.AddListener(() =>
            Shuffle()
        );
        btnReset.onClick.AddListener(() =>
            ResetCard()
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
        //btnBuy.gameObject.SetActive(false);
        btnCard_AC.SetTrigger("Shuffle");
        btnCard_AC_1.SetTrigger("Shuffle");
        btnCard_AC_2.SetTrigger("Shuffle");
        btnCard_AC_3.SetTrigger("Shuffle");
        mainAC.SetTrigger("Shuffle");
        //txtPick.SetActive(true);
        canOpen = true;
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

            //btnReset.gameObject.SetActive(true);
            //txtPick.SetActive(false);
            mainAC.SetTrigger("Open");
        }

    }

    public void ResetCard()
    {
        btnCard_AC.SetTrigger("Reset");
        btnCard_AC_1.SetTrigger("Reset");
        btnCard_AC_2.SetTrigger("Reset");
        btnCard_AC_3.SetTrigger("Reset");
        mainAC.SetTrigger("Reset");
        //btnReset.gameObject.SetActive(false);
        //btnBuy.gameObject.SetActive(true);
        //txtPick.SetActive(false);
        canOpen = false;

    }
}
