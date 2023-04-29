using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using Random = UnityEngine.Random;

public class InventoryController : Singleton<InventoryController>
{
    public GameObject Hero;
    public GameObject ItemPanel;
    public GameObject MaterialPanel;
    public GameObject ShardPanel;

    public GameObject SlotItem;
    public GameObject InfoPanel;
    public GameObject[] Equipment;
    public GameObject PopupPrefab;
    public GameObject ParentPanel;
    public GameObject[] albilitiesText;
    public ItemInflate selectingItem;
    private List<GameObject> listItem = new List<GameObject>();

    int upgradeItemID = -1;

    public int[] baseAlbility = new int[6] { 0, 0, 0, 0, 0, 0 };
    public int[] bonusAlbility = new int[6] { 0, 0, 0, 0, 0, 0 };

    private Sprite[] itemsSprite;

    void Awake()
    {
        itemsSprite = Resources.LoadAll<Sprite>("Contents/Item/ItemAtlas");
    }
    
    void Start()
    {
        Init();
    }
    void Init()
    {
        InfoPanel.SetActive(false);
        //fix inventory to center
        ItemPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2((ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5, (ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5);
        initEquipment();
        initMaterial();
        initShard();
    }
    public void initEquipment()
    {
        List<ItemInventory> itemArr = ItemDatabase.Instance.getEquipmentData();

        List<GameObject> child = new List<GameObject>();
        for (int i = 0; i < ItemPanel.transform.childCount; i++)
        {
            if (i < itemArr.Count)
            {
                child.Add(ItemPanel.transform.GetChild(i).gameObject);
            }
            else
            {
                Destroy(ItemPanel.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < itemArr.Count; i++)
        {
            if(i < child.Count)
            {
                child[i].GetComponent<ItemInflate>().InitData(itemArr[i]);
            } else
            {
                GameObject _slotitem = Instantiate(SlotItem, ItemPanel.transform);
                _slotitem.transform.localPosition = new Vector3(0, 0, 0);
                _slotitem.GetComponent<ItemInflate>().InitData(itemArr[i]);
            }
        }
    }
    public void initMaterial()
    {
        List<ItemInventory> itemArr = ItemDatabase.Instance.getMaterialData();
        List<GameObject> child = new List<GameObject>();
        for (int i = 0; i < MaterialPanel.transform.childCount; i++)
        {
            if (i < itemArr.Count)
            {
                child.Add(MaterialPanel.transform.GetChild(i).gameObject);
            }
            else
            {
                Destroy(MaterialPanel.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < itemArr.Count; i++)
        {
            if (i < child.Count)
            {
                child[i].GetComponent<ItemInflate>().InitData(itemArr[i]);
            }
            else
            {
                GameObject _slotitem = Instantiate(SlotItem, MaterialPanel.transform);
                _slotitem.transform.localPosition = new Vector3(0, 0, 0);
                _slotitem.GetComponent<ItemInflate>().InitData(itemArr[i]);
            }
        }
    }
    public void initShard()
    {
        List<ItemInventory> itemArr = ItemDatabase.Instance.getShardData();
        List<GameObject> child = new List<GameObject>();
        for (int i = 0; i < ShardPanel.transform.childCount; i++)
        {
            if (i < itemArr.Count)
            {
                child.Add(ShardPanel.transform.GetChild(i).gameObject);
            }
            else
            {
                Destroy(ShardPanel.transform.GetChild(i).gameObject);
            }
        }
        for (int i = 0; i < itemArr.Count; i++)
        {
            if (i < child.Count)
            {
                child[i].GetComponent<ItemInflate>().InitData(itemArr[i]);
            }
            else
            {
                GameObject _slotitem = Instantiate(SlotItem, ShardPanel.transform);
                _slotitem.transform.localPosition = new Vector3(0, 0, 0);
                _slotitem.GetComponent<ItemInflate>().InitData(itemArr[i]);
            }
        }
    }

}
