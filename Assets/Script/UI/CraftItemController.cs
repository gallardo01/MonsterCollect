using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CraftItemController : Singleton<CraftItemController>
{
    [SerializeField] Button[] itemButton;
    [SerializeField] GameObject[] itemShow;
    [SerializeField] GameObject[] particleItem;

    [SerializeField] GameObject[] itemAnimation;
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
    private bool canAddItem = true;
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
        itemAnimation[0].SetActive(true);
        itemAnimation[0].transform.DOMove(new Vector3(result.transform.position.x, result.transform.position.y, 0), 0.5f, false);
        itemAnimation[1].SetActive(true);
        itemAnimation[1].transform.DOMove(new Vector3(result.transform.position.x, result.transform.position.y, 0), 0.5f, false);
        itemAnimation[2].SetActive(true);
        itemAnimation[2].transform.DOMove(new Vector3(result.transform.position.x, result.transform.position.y, 0), 0.5f, false);
        itemShow[0].SetActive(false);
        itemShow[1].SetActive(false);
        itemShow[2].SetActive(false);

        yield return new WaitForSeconds(0.5f);
        itemAnimation[0].SetActive(false);
        itemAnimation[1].SetActive(false);
        itemAnimation[2].SetActive(false);

        for (int i = 0; i < 3; i++)
        {
            ItemDatabase.Instance.removeItemEquipment(listItem[i].ShopId);
        }
        ItemDatabase.Instance.reduceItemSlotById(9, 1);
        ItemInventory givenItem = ItemDatabase.Instance.getItemObject(UnityEngine.Random.Range(10, 34), 1, getRarityItem());
        ItemDatabase.Instance.addNewItemByObject(givenItem);

        particleItem[0].SetActive(true);
        yield return new WaitForSeconds(2f);
        itemShow[3].SetActive(true);
        itemShow[3].GetComponent<ItemInflate>().InitData(givenItem);
        particleItem[1].SetActive(true);

        yield return new WaitForSeconds(2f);
        particleItem[0].SetActive(false);
        particleItem[1].SetActive(false);
        InventoryController.Instance.onClickItem(givenItem);
        yield return new WaitForSeconds(0.3f);
        itemShow[3].SetActive(false);
        InventoryController.Instance.initEquipment();
        InventoryController.Instance.initLayout();
        overlayAllObj.SetActive(false);
        initFunction();
    }
    private int getRarityItem()
    {
        int rate = UnityEngine.Random.Range(0, 300);
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
    public void addItemOnList(ItemInventory item, Transform pos)
    {
        if (canAddItem)
        {
            int index = findNearestAvailable();
            if (index >= 0)
            {
                canAddItem = false;
                itemAnimation[index].SetActive(true);
                itemAnimation[index].GetComponent<ItemInflate>().InitData(item);
                itemAnimation[index].transform.position = pos.position;
                itemAnimation[index].transform.DOMove(new Vector3(itemShow[index].transform.position.x, itemShow[index].transform.position.y, 0), 0.5f, false);
                StartCoroutine(deactiveItem(index, item));
            }
        }
    }
    IEnumerator deactiveItem(int index, ItemInventory item)
    {
        ItemDatabase.Instance.craftItem(item.ShopId);
        InventoryController.Instance.initEquipment();
        InventoryController.Instance.initLayout();
        yield return new WaitForSeconds(0.5f);
        itemAnimation[index].SetActive(false);
        itemShow[index].SetActive(true);
        itemShow[index].GetComponent<ItemInflate>().InitData(item);
        isItemOnList[index] = true;
        listItem[index] = item;
        activeButton();
        updateTextRate();
        canAddItem = true;
    }
    private void clickItem(int index)
    {
        if (isItemOnList[index] == true)
        {
            isItemOnList[index] = false;
            if (listItem[index] != null)
            {
                ItemDatabase.Instance.unCraftItem(listItem[index].ShopId);
            }
            itemShow[index].SetActive(false);
            InventoryController.Instance.initEquipment();
            InventoryController.Instance.initLayout();
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
                    rateItem[listItem[i].Rarity] += 75;
                    rateItem[listItem[i].Rarity + 1] += 25;
                }
                else
                {
                    rateItem[listItem[i].Rarity] += 100;
                }
            }
        }
    }

}
