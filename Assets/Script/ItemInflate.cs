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
    public TextMeshProUGUI level;


    private ItemInventory itemData;
    public void InitData(ItemInventory iteminfo)
    {
        itemData = iteminfo;
        item.sprite = InventoryController.Instance.getSpriteIndex(iteminfo.Id);
        rarity.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + iteminfo.Rarity.ToString());
        type.sprite = Resources.Load<Sprite>("UI/Inventory/PlaceHolder/" + iteminfo.Type.ToString());
        if (iteminfo.Type == 0 || iteminfo.Type == 10)
        {
            slot.text = iteminfo.Slot.ToString();
            level.text = "";
        }
        else
        {
            slot.text = "";
            level.text = "Lv." + iteminfo.Level.ToString();
        }
    }

    //// Start is called before the first frame update
    void Start()
    {

        button.onClick.AddListener(() => onClickItem(this.transform));
    }
    private void onClickItem(Transform pos)
    {
        gameObject.GetComponent<Animator>().Play("Click_Animation");
        if (InventoryController.Instance.getCraftItem() == true && itemData.Type % 10 > 0)
        {
            CraftItemController.Instance.addItemOnList(itemData, pos);
        }
        else
        {
            if (itemData != null)
            {
                InventoryController.Instance.onClickItem(itemData);
            }
        }
    }

    public void setTextSlot(int inven, int require)
    {
        slot.text = inven + "/" + require;
        if (inven >= require)
        {
            slot.color = Color.white;
        } else
        {
            slot.color = Color.red;
        }
    }
}
