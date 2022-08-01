using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ItemInflate : MonoBehaviour
{

    public Button button;
    public Image rarity;
    public Image item;
    public Image type;
    public TextMeshProUGUI slot;
    public GameObject focus;

    private ItemInventory itemData;
    public void InitData(ItemInventory iteminfo)
    {
        itemData = iteminfo;
        item.sprite = Resources.Load<Sprite>("Contents/Item/" + iteminfo.Id.ToString());
        rarity.sprite = Resources.Load<Sprite>("Contents/Icon/UI/" + iteminfo.Rarity.ToString());
        type.sprite = Resources.Load<Sprite>("Contents/Icon/DameType/" + iteminfo.Type.ToString());
        if (iteminfo.Type == 0)
        {
            slot.text = iteminfo.Slot.ToString();
        }
        else
        {
            slot.text = "";
        }
        
    }

    //// Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(() => onClickItem());
    }
    private void onClickItem()
    {
        if (itemData != null)
        {
            InventoryController.Instance.onClickItem(itemData);
            InventoryController.Instance.selectingItem = this;
            setFocus(true);
        }
    }
    public void setFocus(bool enable)
    {
        focus.SetActive(enable);
    }
    public bool isFocus()
    {
        return focus.active;
    }
}
