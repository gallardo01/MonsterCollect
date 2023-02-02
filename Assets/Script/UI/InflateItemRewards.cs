﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InflateItemRewards : MonoBehaviour
{
    public Image backerItem;
    public Image iconItem;
    public TextMeshProUGUI slotItem;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void inflateItem(ItemInventory item)
    {
        backerItem.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + item.Rarity);
        iconItem.sprite = Resources.Load<Sprite>("Contents/Item/" + item.Id);
        slotItem.text = item.Slot.ToString();
    }

    public void inflateGoldItem(int slot)
    {
        slotItem.text = slot.ToString();
    }
}
