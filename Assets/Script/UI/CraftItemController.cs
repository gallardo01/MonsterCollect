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
    [SerializeField] GameObject overlayAllObj;
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

        startCraft.onClick.AddListener(() => startCraftAction());
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
        for(int i = 0; i < 3; i++) { isItemOnList[i] = false; itemShow[i].SetActive(false);}
        activeButton();
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
        overlayAllObj.SetActive(false);
    }
    private void startCraftAction()
    {
        startCraft.interactable = false;
        overlayAllObj.SetActive(true);
        StartCoroutine(animationCraft());
     
    }
    IEnumerator animationCraft()
    {
        particleItem[0].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        particleItem[1].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        particleItem[2].SetActive(true);
        yield return new WaitForSeconds(0.5f);
        particleItem[0].SetActive(false);
        itemShow[0].SetActive(false);
        yield return new WaitForSeconds(0.5f);
        particleItem[1].SetActive(false);
        itemShow[1].SetActive(false); 
        yield return new WaitForSeconds(0.5f);
        particleItem[2].SetActive(false);
        itemShow[2].SetActive(false);
        for (int i = 0; i < 3; i++)
        {
            ItemDatabase.Instance.removeItemEquipment(listItem[i].ShopId);
        }
        ItemDatabase.Instance.reduceItemSlotById(9, 1);
        ItemInventory givenItem = ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, getRarityItem());
        particleItem[3].SetActive(true);
        ItemDatabase.Instance.addNewItemByObject(givenItem);
        yield return new WaitForSeconds(0.5f);
        particleItem[3].SetActive(false);
        itemShow[3].SetActive(true);
        itemShow[3].GetComponent<ItemInflate>().InitData(givenItem);
        yield return new WaitForSeconds(0.5f);
        itemShow[3].SetActive(false);
        InventoryController.Instance.initEquipment();
        overlayAllObj.SetActive(false);
        activeButton();
    }
    private int getRarityItem()
    {
        int rate = Random.Range(0, 300);
        int totalRate = 0;
        for (int i = 1; i < 6; i++)
        {
            totalRate += rateItem[i];
            if(rate < totalRate)
            {
                return i;
            }
        }
        return 1;
    }
    private void activeButton()
    {
        if(findNearestAvailable() == -1 && ItemDatabase.Instance.fetchInventoryById(9).Slot > 0)
        {
            startCraft.interactable = true;
        } else
        {
            startCraft.interactable = false;
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
            activeButton();
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
