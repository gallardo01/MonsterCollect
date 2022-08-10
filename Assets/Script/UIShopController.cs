using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIShopController : MonoBehaviour
{
    public GameObject TargetedOfferPanel;
    public GameObject ChestsPanel;
    public GameObject HeroesPanel;
    public GameObject GemsPanel;
    public GameObject CoinsPanel;
    void Start()
    {
        InitTargetedOffer();
        InitChests();
    }
    void InitTargetedOffer()
    {
        //init data
        TargetedOfferPanel.transform.Find("Top/Badge/Text").GetComponent<TextMeshProUGUI>().text = "4x Value";
        TargetedOfferPanel.transform.Find("Panel/Description").GetComponent<TextMeshProUGUI>().text = "Super value! Once Only! Super Pack! Contains:";
        TargetedOfferPanel.transform.Find("Panel/BaseValue").GetComponent<TextMeshProUGUI>().text = "49,000đ";
        TargetedOfferPanel.transform.Find("Panel/Value").GetComponent<TextMeshProUGUI>().text = "29,000đ";

        var item1_image = TargetedOfferPanel.transform.Find("Panel/Content/Item1/Image").GetComponent<Image>().sprite;
        var item2_image = TargetedOfferPanel.transform.Find("Panel/Content/Item1 (1)/Image").GetComponent<Image>().sprite;
        var item3_image = TargetedOfferPanel.transform.Find("Panel/Content/Item1 (2)/Image").GetComponent<Image>().sprite;
        var item4_image = TargetedOfferPanel.transform.Find("Panel/Content/Item1 (3)/Image").GetComponent<Image>().sprite;

        //setup data

    }
    void InitChests()
    {
        ChestsPanel.transform.Find("Content/Top/Golden/Button/OpenKeyButton/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "<size=120%><sprite=5></size> 0/2";
    }
}
