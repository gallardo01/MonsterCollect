using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;
 
public class InventoryController : Singleton<InventoryController>
{
    public GameObject ItemPanel;
    public GameObject SlotItem;

    private List<GameObject> listItem = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        List<ItemInventory> itemArr = ItemDatabase.Instance.getAllData();
        foreach (Transform child in ItemPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < itemArr.Count ;i++)
        {
            ItemInventory item = itemArr[i];
            GameObject _slotitem = Instantiate(SlotItem, ItemPanel.transform);
            _slotitem.transform.localPosition = new Vector3(0, 0, 0);
            _slotitem.GetComponent<ItemInflate>().InitData(item);
            listItem.Add(_slotitem);
        }

    }
    public void onClickItem(ItemInventory data)
    {
        foreach(var item in listItem)
        {
            item.GetComponent<ItemInflate>().setFocus(false);
        }
    }



}
