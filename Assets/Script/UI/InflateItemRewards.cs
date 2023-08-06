using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using static UnityEditor.Progress;

public class InflateItemRewards : MonoBehaviour
{
    public Image backerItem;
    public Image iconItem;
    public TextMeshProUGUI slotItem;
    private Sprite[] itemSprite;

    // Start is called before the first frame update

    private void Awake()
    {
    }
    void Start()
    {
        
    }

    public void inflateItem(ItemInventory item)
    {
        itemSprite = Resources.LoadAll<Sprite>("Contents/Item/Item");
        backerItem.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + item.Rarity);
        int index;
        if (item.Id < 100)
        {
            index = item.Id - 1;
        }
        else
        {
            index = item.Id - 68;
        }
        iconItem.sprite = itemSprite[index];
        slotItem.text = item.Slot.ToString();
    }

    public void inflateGoldItem(int slot)
    {
        iconItem.sprite = Resources.Load<Sprite>("UI/Sprites/Gold");
        slotItem.text = slot.ToString();
    }
}
