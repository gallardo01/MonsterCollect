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
    public GameObject ItemPanel;
    public GameObject SlotItem;
    public GameObject InfoPanel;
    public GameObject[] Equipment;


    private List<GameObject> listItem = new List<GameObject>();
    string[] rarity = new string[4] { "Common", "Rare", "Mythical", "Epic" };
    string[] Albility = new string[6] { "Attack", "HP", "Armor", "Speed", "Exp Bonus", "Gold Bonus" };
    int[] iconSort = new int[6] { 4, 3, 0, 1, 2, 5 };
    Color[] rarityColor = new Color[4] { Color.white, Color.green, Color.blue, Color.magenta };
    int upgradeItemID = -1;
    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    void Init()
    {
        List<ItemInventory> itemArr = ItemDatabase.Instance.getAllData();
        foreach (Transform child in ItemPanel.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        for (int i = 0; i < itemArr.Count; i++)
        {
            ItemInventory item = itemArr[i];
            if (item.Slot > 0)
            {
                GameObject _slotitem = Instantiate(SlotItem, ItemPanel.transform);
                _slotitem.transform.localPosition = new Vector3(0, 0, 0);
                _slotitem.GetComponent<ItemInflate>().InitData(item);
                
                listItem.Add(_slotitem);
                if (item.Id == 8) upgradeItemID = listItem.IndexOf(_slotitem);
            }

        }
    }
    public void onClickItem(ItemInventory data)
    {
        foreach (var item in listItem)
        {
            item.GetComponent<ItemInflate>().setFocus(false);
        }
        SetInfoPanelData(data);
    }
    void SetInfoPanelData(ItemInventory data)
    {
        var main = InfoPanel.transform;

        main.Find("Panel/Top/TopHub/Name").GetComponent<TextMeshProUGUI>().text = data.Name;
        main.Find("Panel/Top/TopHub/Name").GetComponent<TextMeshProUGUI>().color = rarityColor[data.Rarity - 1];
        main.Find("Panel/Top/TopHub/Rarity").GetComponent<TextMeshProUGUI>().text = rarity[data.Rarity - 1];
        main.Find("Panel/Top/TopHub/Rarity").GetComponent<TextMeshProUGUI>().color = rarityColor[data.Rarity - 1];
        main.Find("Panel/Top/Info/Description/Level").GetComponent<TextMeshProUGUI>().text = "Level: " + data.Level + " / 50";

        main.Find("Panel/Top/Info/Description/Des").GetComponent<TextMeshProUGUI>().text = data.Contents;
        main.Find("Panel/Top/Info/ItemSprite").GetComponent<Image>().sprite = Resources.Load<Sprite>("Contents/Icon/UI/" + data.Rarity.ToString());
        main.Find("Panel/Top/Info/ItemSprite/Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Contents/Item/" + data.Id.ToString());
        main.Find("Panel/Top/Info/ItemSprite/Type").GetComponent<Image>().sprite = Resources.Load<Sprite>("Contents/Icon/DameType/" + data.Type.ToString());
        if (data.Type == 0)
        {
            main.Find("Panel/Top/TopHub/Name").GetComponent<TextMeshProUGUI>().text = data.Name + " X" + data.Slot;
            main.Find("Panel/Top/Info/Description/Level").gameObject.SetActive(false);
            main.Find("Panel/Bottom/Upgrade").gameObject.SetActive(false);
            main.Find("Panel/Bottom/Button/ButtonFuse").gameObject.SetActive(false);
            main.Find("Panel/Bottom/Button/ButtonUpgrade").gameObject.SetActive(false);
            main.Find("Panel/Bottom/Button/ButtonEquip/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Use";
        }
        else
        {
            main.Find("Panel/Top/Info/Description/Level").gameObject.SetActive(true);
            main.Find("Panel/Bottom/Upgrade").gameObject.SetActive(true);
            main.Find("Panel/Bottom/Button/ButtonFuse").gameObject.SetActive(true);
            main.Find("Panel/Bottom/Button/ButtonUpgrade").gameObject.SetActive(true);

            main.Find("Panel/Bottom/Button/ButtonUpgrade/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "<size=120%><sprite=5></size>x" + data.Level * 1000 + " \n Upgrade";
            main.Find("Panel/Bottom/Button/ButtonEquip/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "Equip";
        }
        ItemInventory upgrade_item = ItemDatabase.Instance.fetchInventoryById(8);

        main.Find("Panel/Bottom/Upgrade/Stats/Content1").GetComponent<TextMeshProUGUI>().text = data.Stats_1 <= 0 ? "" : "<size=120%><sprite=" + iconSort[((data.Stats_1 / 100) - 1)] + "></size>" + Albility[(data.Stats_1 / 100) - 1] + " + " + data.Stats_1 % 100;
        main.Find("Panel/Bottom/Upgrade/Stats/Content2").GetComponent<TextMeshProUGUI>().text = data.Stats_2 <= 0 ? "" : "<size=120%><sprite=" + iconSort[((data.Stats_2 / 100) - 1)] + "></size>" + Albility[(data.Stats_2 / 100) - 1] + " + " + data.Stats_2 % 100;
        main.Find("Panel/Bottom/Upgrade/Stats/Content3").GetComponent<TextMeshProUGUI>().text = data.Stats_3 <= 0 ? "" : "<size=120%><sprite=" + iconSort[((data.Stats_3 / 100) - 1)] + "></size>" + Albility[(data.Stats_3 / 100) - 1] + " + " + data.Stats_3 % 100;

        main.Find("Panel/Bottom/Button/ButtonFuse").GetComponent<Button>().onClick.AddListener(() => fuseItem(data.Id));
        if (data.Level >= 10)
        {
            main.Find("Panel/Bottom/Upgrade/Material/Content").GetComponent<TextMeshProUGUI>().text = "Level Max";
            main.Find("Panel/Bottom/Button/ButtonUpgrade").gameObject.SetActive(false);
        }
        else
        {
            main.Find("Panel/Bottom/Upgrade/Material/Content").GetComponent<TextMeshProUGUI>().text = "X" + upgrade_item.Slot + "/" + data.Level;
            main.Find("Panel/Bottom/Button/ButtonUpgrade").GetComponent<Button>().onClick.AddListener(() => UpgradeItem(data.ShopId, data.Level, data.Level * 1000));
        }
        main.Find("Panel/Bottom/Button/ButtonEquip").GetComponent<Button>().onClick.AddListener(() => { if (data.IsUse == 1) return; InitEquipment(data, data.Type - 1); InfoPanel.SetActive(false); });

        InfoPanel.SetActive(true);

    }
    void fuseItem(int itemID)
    {

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
        SetInfoPanelData(data);
    }
    void InitEquipment(ItemInventory item, int itemSlot)
    {
        Equipment[itemSlot].transform.Find("Image").GetComponent<Image>().sprite = Resources.Load<Sprite>("Contents/Item/" + item.Id.ToString());
        Equipment[itemSlot].GetComponent<Image>().sprite = Resources.Load<Sprite>("Contents/Icon/UI/" + item.Rarity.ToString());
        Equipment[itemSlot].transform.Find("Level").GetComponent<TextMeshProUGUI>().text = "Lvl " + item.Level.ToString();

    }
}
