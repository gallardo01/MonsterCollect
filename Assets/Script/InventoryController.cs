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
    public TextMeshProUGUI[] abilitiesText;

    public GameObject itemDetailPanel;
    public GameObject consumeDetailPanel;

    [SerializeField] Button filterItem;
    [SerializeField] TextMeshProUGUI textFilter;

    [SerializeField] Button craftButton;
    [SerializeField] GameObject craftItemObj;
    [SerializeField] GameObject hideScreenWhenCraft;
    private bool filterBool = true;
    private bool isCraftItem = false;
    private Sprite[] itemsSprite;

    void Awake()
    {
        itemsSprite = Resources.LoadAll<Sprite>("Contents/Item/Item");
        ItemPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2((ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5, (ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5);
        MaterialPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2((ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5, (ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5);
        ShardPanel.GetComponent<GridLayoutGroup>().cellSize = new Vector2((ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5, (ItemPanel.GetComponent<RectTransform>().rect.size.x - 190) / 5);

    }

    void Start()
    {
        Init();
        filterItem.onClick.AddListener(() => filterItems());
        craftButton.onClick.AddListener(() => craftButtonAction());
    }
    public Sprite getSpriteIndex(int index)
    {
        int indexSprite;
        if(index < 100)
        {
            indexSprite = index - 1;
        } else
        {
            indexSprite = index - 68;
        }
        return itemsSprite[indexSprite];
    }
    private void craftButtonAction()
    {
        if (isCraftItem == false)
        {
            craftItemObj.SetActive(true);
            craftItemObj.GetComponent<CraftItemController>().initFunction();
            hideScreenWhenCraft.SetActive(false);
        }
        else
        {
            ItemDatabase.Instance.unCraftAllItem();
            craftItemObj.SetActive(false);
            hideScreenWhenCraft.SetActive(true);
            initEquipment();
        }
        isCraftItem = !isCraftItem;
    }
    public bool getCraftItem()
    {
        return isCraftItem;
    }
    private void filterItems()
    {
        filterBool = !filterBool;
        if(filterBool == true)
        {
            textFilter.text = "By Level";
        } else
        {
            textFilter.text = "By Type";
        }
        initEquipment();
    }
    public void Init()
    {
        InfoPanel.SetActive(false);
        //fix inventory to center
        initItemData();
    }
    public void initItemData()
    {
        initUsedItem();
        initEquipment();
        initMaterial();
        initShard();
        initStats();
    }
    public void initUsedItem()
    {
        for(int i = 0; i < 6; i++)
        {
            Equipment[i].SetActive(false);
        }
        List<ItemInventory> items = ItemDatabase.Instance.fetchUsedItem();
        for (int i = 0; i < items.Count; i++)
        {
            Equipment[items[i].Type - 1].SetActive(true);
            Equipment[items[i].Type - 1].GetComponent<ItemInflate>().InitData(items[i]);
        }
    }
    public void initEquipment()
    {
        List<ItemInventory> itemArr = ItemDatabase.Instance.getEquipmentData(filterBool);

        List<GameObject> child = new List<GameObject>();
        for (int i = 0; i < ItemPanel.transform.childCount; i++)
        {
            child.Add(ItemPanel.transform.GetChild(i).gameObject);

            if (i >= itemArr.Count)
            {
                ItemPanel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < itemArr.Count; i++)
        {
            if(i < child.Count)
            {
                child[i].SetActive(true);
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
    public void initStats()
    {
        UserInformation user = RealTimeDatabase.Instance.getUserInformation();
        abilitiesText[0].text = user.Atk.ToString();
        abilitiesText[1].text = user.Hp.ToString();
        abilitiesText[2].text = user.Armour.ToString();
        abilitiesText[3].text = user.Move.ToString();
        abilitiesText[4].text = user.Crit.ToString();
        abilitiesText[5].text = user.AttackSpeed.ToString();
    }
    public void onClickItem(ItemInventory item)
    {
        StartCoroutine(clickItem(item));
    }
    IEnumerator clickItem(ItemInventory item)
    {
        yield return new WaitForSeconds(0.3f);
        Hero.SetActive(false);
        if (item.Type > 0 && item.Type < 10)
        {
            itemDetailPanel.SetActive(true);
            itemDetailPanel.GetComponent<InflateShowItemController>().initItem(item);
        } else
        {
            consumeDetailPanel.SetActive(true);
            consumeDetailPanel.GetComponent<InflateShowItemController>().consumeItem(item);
        }
    }
    public void enableHeroes()
    {
        initItemData();
        Hero.SetActive(true);
    }
}
