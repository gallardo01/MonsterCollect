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


    void Start()
    {
        btnChange.onClick.AddListener(() => swapToHero());
    }

    void swapToHero()
    {
        tabHero.SetActive(true);
        tabInventory.SetActive(false);
        bar.SetActive(false);
    }

}
