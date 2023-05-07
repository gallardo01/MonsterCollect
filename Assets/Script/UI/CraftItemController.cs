using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftItemController : Singleton<CraftItemController>
{
    [SerializeField] Button[] itemButton;
    [SerializeField] GameObject[] itemShow;
    [SerializeField] GameObject[] particleItem;
    [SerializeField] TextMeshProUGUI textCraftShard;
    [SerializeField] Button startCraft;
    [SerializeField] GameObject result;
    [SerializeField] TextMeshProUGUI rateText;

    private ItemInventory[] listItem = {null, null, null};
    private bool[] isItemOnList = { false, false, false };
    private int[] rateItem = { 0, 0, 0, 0, 0, 0 };
    private string[] colorItem = { "", "white", "green", "blue", "yellow", "red" };
    private string[] rarityItem = {"", "Common", "Great", "Rare", "Legendary", "Epic"};
    private void Start()
    {
        itemButton[0].onClick.AddListener(() => clickItem(0));
        itemButton[1].onClick.AddListener(() => clickItem(1));
        itemButton[2].onClick.AddListener(() => clickItem(2));
        initFunction();
    }
    public void initFunction()
    {
        if (ItemDatabase.Instance.fetchInventoryById(9).Slot > 0)
        {
            textCraftShard.text = ItemDatabase.Instance.fetchInventoryById(9).Slot + "/1";
        } else
        {
            textCraftShard.text = "<color=red>" + ItemDatabase.Instance.fetchInventoryById(9).Slot + "</color>" + "/1";
        }
        rateText.text = "";
    }
    private void updateTextRate()
    {
        calculateRateItem();
        string text = "";
        int countItem = 0;
        for(int i = 0; i < 3; i++)
        {
            if (isItemOnList[i] == true) { countItem++; }
        }
        for(int i = 1; i < 6; i++)
        {
            if (rateItem[i] > 0)
            {
                text += $"<color={colorItem[i]}>{rateItem[i]*100/(100*countItem)}% {rarityItem[i]} </color>";
            }
        }
        rateText.text = text;
    }
    private void startCraftAction()
    {
        for(int i = 0; i < 3; i++)
        {
            ItemDatabase.Instance.removeItemEquipment(listItem[i].ShopId);
        }

    }
    private void activeButton()
    {
        if(findNearestAvailable() == -1 && ItemDatabase.Instance.fetchInventoryById(9).Slot > 0)
        {
            startCraft.enabled = true;
        } else
        {
            startCraft.enabled = false;
        }
    }
    public void addItemOnList(ItemInventory item)
    {
        int index = findNearestAvailable();
        if(index >= 0)
        {
            isItemOnList[index] = true;
            listItem[index] = item;
            itemShow[index].SetActive(true);
            itemShow[index].GetComponent<ItemInflate>().InitData(item);
            ItemDatabase.Instance.craftItem(item.ShopId);
            InventoryController.Instance.initEquipment();
            updateTextRate();
        }
    }
    private void clickItem(int index)
    {
        Debug.Log("click");
        if (isItemOnList[index] == true)
        {
            isItemOnList[index] = false;
            if (listItem[index] != null)
            {
                ItemDatabase.Instance.unCraftItem(listItem[index].ShopId);
            }
            itemShow[index].SetActive(false);
            InventoryController.Instance.initEquipment();
            activeButton();
            updateTextRate();
        }
    }
    private int findNearestAvailable()
    {
        for(int i = 0; i < 3; i++)
        {
            if (isItemOnList[i] == false)
            {
                return i;
            }
        }
        return -1;
    }
    private void calculateRateItem()
    {
        for (int i = 0; i < 6; i++) { rateItem[i] = 0; }
        for(int i = 0; i < 3; i++)
        {
            if (isItemOnList[i] == true)
            {
                if (listItem[i].Rarity < 5)
                {
                    rateItem[listItem[i].Rarity] += 80;
                    rateItem[listItem[i].Rarity + 1] += 20;
                }
                else
                {
                    rateItem[listItem[i].Rarity] += 100;
                }
            }
        }
    }

}
