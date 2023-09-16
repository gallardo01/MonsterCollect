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
    public ParticleSystem obj;
    private ItemInventory itemData;

    //// Start is called before the first frame update
    void Start()
    {

        button.onClick.AddListener(() => onClickItem(this.transform));
    }
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
    public void initDataUnlock(ItemInventory iteminfo)
    {
        itemData = iteminfo;
        item.sprite = InventoryController.Instance.getSpriteIndex(iteminfo.Id);
        rarity.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + iteminfo.Rarity.ToString());
        type.sprite = Resources.Load<Sprite>("UI/Inventory/PlaceHolder/" + iteminfo.Type.ToString());
        slot.text = "Unlock";
        level.text = "";
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
    public void playAnim()
    {
        gameObject.GetComponent<Animator>().Play("Shake");
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
    public void setupRarityObj(int rarity)
    {
        gameObject.GetComponent<Animator>().Play("Shake");
        if (rarity == 1) { obj.startColor = Color.white; }
        if (rarity == 2) { obj.startColor = Color.green; }
        if (rarity == 3) { obj.startColor = Color.blue; };
        if (rarity == 4) { obj.startColor = Color.yellow; }
        if (rarity == 5) { obj.startColor = Color.red; }
    }
    public void setupCurrencyItem(int gold, int diamond)
    {
        if(gold > 0)
        {
            item.sprite = Resources.Load<Sprite>("UI/Sprites/Gold");
            rarity.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/1");
            type.sprite = Resources.Load<Sprite>("UI/Inventory/PlaceHolder/0");
            slot.text = gold.ToString();
            level.text = "";
        } else if(diamond > 0)
        {
            item.sprite = Resources.Load<Sprite>("UI/Sprites/Diamond");
            rarity.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/1");
            type.sprite = Resources.Load<Sprite>("UI/Inventory/PlaceHolder/0");
            slot.text = diamond.ToString();
            level.text = "";
        }
    }
}


