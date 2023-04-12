using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventoryFilter : MonoBehaviour
{
    public Button filterByItem;
    public Button filterByEquipment;

    public int filterStatus; //0 = not filter, 1 == Item, 2== equipment
    // Start is called before the first frame update
    void Start()
    {
        filterStatus = 0;
        filterByItem.onClick.AddListener(FilterByItemClicked);
        filterByEquipment.onClick.AddListener(FilterByEquipmentClicked);
    }
    public void FilterByItemClicked()
    {
        
        if(filterStatus == 1)
        {
            filterStatus = 0;
            filterByItem.GetComponent<Image>().color = Color.white;
            filterByEquipment.GetComponent<Image>().color = Color.white;
           
        }
        else
        {
            filterStatus = 1;
            filterByItem.GetComponent<Image>().color = Color.gray;
            filterByEquipment.GetComponent<Image>().color = Color.white;

        }
        //InventoryController.Instance.InitInventory(filterStatus);
    }
    public void FilterByEquipmentClicked()
    {
        if (filterStatus == 2)
        {
            filterStatus = 0;
            filterByEquipment.GetComponent<Image>().color = Color.white;
            filterByItem.GetComponent<Image>().color = Color.white;

        }
        else
        {
            filterStatus = 2;
            filterByEquipment.GetComponent<Image>().color = Color.gray;
            filterByItem.GetComponent<Image>().color = Color.white;
           
        }
        //InventoryController.Instance.InitInventory(filterStatus);
    }
}
