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
    private GameObject PopupPanel;
    public ItemInflate selectingItem;
    private List<GameObject> listItem = new List<GameObject>();

    string[] rarity = new string[4] { "Common", "Great", "Rare", "Epic" };
    string[] Albility = new string[6] { "Attack", "HP", "Armor", "Speed", "Exp Bonus", "Gold Bonus" };
    //int[] fuseReturnByRarity = new int[4] { 2, 3, 4, 5 };
    int[] iconSort = new int[6] { 4, 3, 0, 1, 2, 5 };
    Color[] rarityColor = new Color[4] { Color.white, Color.green, Color.blue, Color.magenta };
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


    void UpgradeItem(int itemShopID, int stoneRequired, int coinRequired)
    {
        if (ItemDatabase.Instance.canReduceItemSlotEvol(8, stoneRequired) == true && UserDatabase.Instance.reduceMoney(coinRequired, 0))
        {
            ItemDatabase.Instance.upgradeItem(itemShopID);
            ItemDatabase.Instance.reduceItemSlotById(8, stoneRequired);
        }
        //Init();
        ItemInventory upgradeItem = ItemDatabase.Instance.fetchInventoryById(8);
        if (upgradeItem.Slot > 0)
            listItem[upgradeItemID].transform.Find("Slot/Count").GetComponent<TextMeshProUGUI>().text = upgradeItem.Slot.ToString();
        else
            listItem[upgradeItemID].transform.Find("Slot/Count").gameObject.SetActive(false);
        ItemInventory data = ItemDatabase.Instance.fetchInventoryByShopId(itemShopID);
        //SetInfoPanelData(data);
        InitAlbilities();
    }
    void InitEquipment(ItemInventory item, int itemSlot)
    {
        string[] Rarity = { "", "Common", "Great", "Rare", "Epic", "Mythic", "Legendary" };
        //Equipment[itemSlot].transform.Find("Contain/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Contents/Item/" + item.Id.ToString());
        //Equipment[itemSlot].GetComponent<Image>().sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + Rarity[item.Rarity]);
        //Equipment[itemSlot].transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lvl " + item.Level.ToString();
        InitAlbilities();

    }
    void InitAlbilities()
    {

        baseAlbility = new int[6] { 0, 0, 0, 0, 0, 0 };
        bonusAlbility = new int[6] { 0, 0, 0, 0, 0, 0 };
        foreach (var item in ItemDatabase.Instance.getAllData())
        {
            if (item.IsUse < 0)
            {
                if (item.Stats_1 > 0) { baseAlbility[((item.Stats_1 / 100) - 1)] += item.Stats_1 % 100; bonusAlbility[(item.Stats_1 / 100) - 1] += item.Level - 1; }
                if (item.Stats_2 > 0) { baseAlbility[((item.Stats_2 / 100) - 1)] += item.Stats_2 % 100; bonusAlbility[(item.Stats_2 / 100) - 1] += item.Level - 1; }
                if (item.Stats_3 > 0) { baseAlbility[((item.Stats_3 / 100) - 1)] += item.Stats_2 % 100; bonusAlbility[(item.Stats_3 / 100) - 1] += item.Level - 1; }
            }
        }
        for (int i = 0; i < 6; ++i)
        {
            int num = baseAlbility[i] + bonusAlbility[i];
            albilitiesText[i].GetComponent<TextMeshProUGUI>().text = num.ToString();
        }
    }
    public Sprite GetSprite(int itemID)
    {
        string temp = "ItemAtlas_" + itemID.ToString();
        Sprite outputSprite = null;
        foreach (Sprite s in itemsSprite)
        {
            if (s.name == temp)
            {
                outputSprite = s;
            }
        }
        return outputSprite;
    }
}
