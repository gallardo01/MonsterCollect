using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LitJson;
using System.Text;
using System.IO;
using System;
using Random = UnityEngine.Random;
using DragonBones;

public class ItemDatabase : Singleton<ItemDatabase>
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private List<ItemData> database = new List<ItemData>();
    private List<ItemInventory> inventoryData = new List<ItemInventory>();
    private JsonData itemData;
    private JsonData myCurrentJsonData;

    // Start is called before the first frame update
    void Start()
    {

    }


    public void LoadData()
    {
        string fileName = "Item.txt";
        LoadResourceTextfileItemData(fileName);

        var items = SyncService.Instance.GetInventory();

        if (items == null || items.Count == 0)
        {
            Debug.Log("Loading inventory from file...");
            string myFileName = "MyItem.txt";
            LoadResourceTextfileCurrentData(myFileName);

            if (PlayerPrefs.HasKey("Init") == false)
            {
                PlayerPrefs.SetInt("Init", 1);
                for (int i = 1; i <= 37; i++)
                {
                    addNewItem(i, 1, Random.Range(1, 6));
                }
                for (int i = 101; i <= 112; i++)
                {
                    addNewItem(i, 300);
                }
            }

            SyncService.Instance.PushInventory(inventoryData);
        } else
        {
            Debug.Log("Using inventory from cloud...");
            inventoryData = items;
        }
    }

    private void LoadResourceTextfileItemData(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        itemData = JsonMapper.ToObject(targetFile.text);
        ConstructItemDatabase();
    }
    private void LoadResourceTextfileCurrentData(string path)
    {
        string tempPath = Application.persistentDataPath + "/c/b/c" + "/MyItem.txt";
        //Load saved Json
        if (!File.Exists(tempPath))
        {
            return;
        }
        byte[] jsonByte = null;
        try
        {
            jsonByte = File.ReadAllBytes(tempPath);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Error: " + e.Message);
        }
        //Convert to json string
        string jsonData = Encoding.ASCII.GetString(jsonByte);
        myCurrentJsonData = JsonMapper.ToObject(jsonData);
        ConstructItemCurrentDataDatabase();
    }
    private void ConstructItemCurrentDataDatabase()
    {
        for (int i = 0; i < myCurrentJsonData.Count; i++)
        {
            ItemInventory newItem = new ItemInventory();
            newItem.Id = (int)myCurrentJsonData[i]["Id"];
            newItem.Name = myCurrentJsonData[i]["Name"].ToString();
            newItem.Contents = myCurrentJsonData[i]["Contents"].ToString();
            newItem.Rarity = (int)myCurrentJsonData[i]["Rarity"];
            newItem.Type = (int)myCurrentJsonData[i]["Type"];

            newItem.Slot = (int)myCurrentJsonData[i]["Slot"];
            newItem.ShopId = (int)myCurrentJsonData[i]["ShopId"];
            newItem.IsUse = (int)myCurrentJsonData[i]["IsUse"];
            newItem.Level = (int)myCurrentJsonData[i]["Level"];

            newItem.Stats_1 = (int)myCurrentJsonData[i]["Stats_1"];
            newItem.Stats_2 = (int)myCurrentJsonData[i]["Stats_2"];
            newItem.Stats_3 = (int)myCurrentJsonData[i]["Stats_3"];
            newItem.Stats_4 = (int)myCurrentJsonData[i]["Stats_4"];
            newItem.Stats_5 = (int)myCurrentJsonData[i]["Stats_5"];

            inventoryData.Add(newItem);
        }
    }
    private void ConstructItemDatabase()
    {
        for (int i = 0; i < itemData.Count; i++)
        {
            ItemData newItem = new ItemData();
            newItem.Id = (int)itemData[i]["Id"];
            newItem.Name = itemData[i]["Name"].ToString();
            newItem.Contents = itemData[i]["Contents"].ToString();
            newItem.Rarity = (int)itemData[i]["Rarity"];
            newItem.Type = (int)itemData[i]["Type"];

            database.Add(newItem);
        }
    }

    public void Save()
    {
        string jsonData = JsonConvert.SerializeObject(inventoryData.ToArray(), Formatting.Indented);

        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "MyItem.txt";

        //Convert To Json then to bytes

        byte[] jsonByte = Encoding.ASCII.GetBytes(jsonData);

        //Create Directory if it does not exist
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        //Debug.Log(path);
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }

        try
        {
            File.WriteAllBytes(filePath, jsonByte);
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed To PlayerInfo Data to: " + tempPath.Replace("/", "\\"));
            Debug.LogWarning("Error: " + e.Message);
        }

        SyncService.Instance.PushInventory(inventoryData);
    }

    public ItemData fetchItemById(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].Id == id)
            {
                return database[i];
            }
        }
        return null;
    }
    public ItemInventory fetchInventoryById(int id)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].Id == id)
            {
                return inventoryData[i];
            }
        }
        return null;
    }
    public ItemInventory fetchInventoryByShopId(int id)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].ShopId == id)
            {
                return inventoryData[i];
            }
        }
        return null;
    }
    public int fetchInventoryByIndex(int id)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].Id == id)
            {
                return i;
            }
        }
        return -1;

    }
    public int fetchInventoryByShopIdIndex(int id)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].ShopId == id)
            {
                return i;
            }
        }
        return -1;
    }
    public List<ItemInventory> fetchUsedItem()
    {
        List<ItemInventory> usedItem = new List<ItemInventory>();
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].IsUse < 0)
            {
                usedItem.Add(inventoryData[i]);
            }
        }
        return usedItem;
    }
    public UserInformation totalEquipmentStats()
    {
        UserInformation itemInformation = new UserInformation();
        List<ItemInventory> usedItem = fetchUsedItem();
        int bonusEquipmentStats = 100 + UserDatabase.Instance.getUserData().Equipment;
        int[] stats = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < usedItem.Count; i++)
        {
            stats[usedItem[i].Stats_1 / 1000] += usedItem[i].Stats_1 % 1000;
            stats[usedItem[i].Stats_2 / 1000] += usedItem[i].Stats_2 % 1000;
            stats[usedItem[i].Stats_3 / 1000] += usedItem[i].Stats_3 % 1000;
            stats[usedItem[i].Stats_4 / 1000] += usedItem[i].Stats_4 % 1000;
            stats[usedItem[i].Stats_5 / 1000] += usedItem[i].Stats_5 % 1000;
        }
        itemInformation.Atk = stats[1] * bonusEquipmentStats / 100;
        itemInformation.Hp = stats[2] * bonusEquipmentStats / 100;
        itemInformation.Armour = stats[3] * bonusEquipmentStats / 100;
        itemInformation.Move = stats[4] * bonusEquipmentStats / 100;
        itemInformation.Crit = stats[5] * bonusEquipmentStats / 100;
        itemInformation.AttackSpeed = stats[6] * bonusEquipmentStats / 100;
        itemInformation.ExGold = stats[7] * bonusEquipmentStats / 100;
        itemInformation.ExExp = stats[8] * bonusEquipmentStats / 100;
        return itemInformation;
    }
    public List<ItemInventory> fetchAllData()
    {
        return inventoryData;
    }
    public int getBonusItemSameType(int type)
    {
        int count = 0;
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].IsUse < 0 && (inventoryData[i].Id - 10) % 4 == (type - 10) % 4)
            {
                count++;
            }
        }
        return count;
    }
    // common = 1 , uncommon = 2, rare = 3, mythical = 4, arcana = 5
    public void addNewItem(int id, int slot)
    {
        ItemData rawItem = fetchItemById(id);
        // Add consume item
        if (rawItem.Type == 0 || rawItem.Type == 10)
        {
            ItemInventory item = new ItemInventory();
            item.Id = rawItem.Id;
            item.Name = rawItem.Name;
            item.Contents = rawItem.Contents;
            item.Rarity = rawItem.Rarity;
            item.Type = rawItem.Type;
            item.ShopId = 0;
            item.IsUse = 0;
            item.Level = 0;
            item.Stats_1 = 0;
            item.Stats_2 = 0;
            item.Stats_3 = 0;
            item.Stats_4 = 0;
            item.Stats_5 = 0;
            int index = fetchInventoryByIndex(id);
            if (index == -1)
            {
                item.Slot = slot;
                inventoryData.Add(item);
            }
            else
            {
                inventoryData[index].Slot += slot;
            }
        }
        else
        {
            ItemInventory item = new ItemInventory();
            item.Id = rawItem.Id;
            item.Name = rawItem.Name;
            item.Contents = rawItem.Contents;
            item.Rarity = rawItem.Rarity;
            item.Type = rawItem.Type;
            item.Slot = 1;
            item.ShopId = Random.Range(0, 999999);
            item.IsUse = 0;
            item.Level = 1;
            item.Stats_1 = 0;
            item.Stats_2 = 0;
            item.Stats_3 = 0;
            item.Stats_4 = 0;
            item.Stats_5 = 0;
            if ((item.Id - 10) % 4 == 0) { item.Stats_1 = 1000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 1) { item.Stats_1 = 4000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 2) { item.Stats_1 = 3000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 3) { item.Stats_1 = 2000 + randomStatsRarity(item.Rarity); }
            if (item.Rarity >= 1)
            {
                item.Stats_2 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            } if (item.Rarity >= 2)
            {
                item.Stats_3 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 3)
            {
                item.Stats_4 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 4)
            {
                item.Stats_5 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            inventoryData.Add(item);
        }
    }

    private int randomStatsRarity(int rarity)
    {
        if (rarity == 1)
        {
            return Random.Range(10, 21);
        }
        else if (rarity == 2)
        {
            return Random.Range(10, 21);
        }
        else if (rarity == 3)
        {
            return Random.Range(15, 26);
        }
        else if (rarity == 4)
        {
            return Random.Range(20, 31);
        }
        else
        {
            return Random.Range(30, 41);
        }
    }

    public void addNewItem(int id, int slot, int rarity)
    {
        ItemData rawItem = fetchItemById(id);
        // Add consume item
        if (rawItem.Type == 0 || rawItem.Type == 10)
        {
            ItemInventory item = new ItemInventory();
            item.Id = rawItem.Id;
            item.Name = rawItem.Name;
            item.Contents = rawItem.Contents;
            item.Rarity = rawItem.Rarity;
            item.Type = rawItem.Type;
            item.ShopId = 0;
            item.IsUse = 0;
            item.Level = 0;
            item.Stats_1 = 0;
            item.Stats_2 = 0;
            item.Stats_3 = 0;
            item.Stats_4 = 0;
            item.Stats_5 = 0;
            int index = fetchInventoryByIndex(id);
            if (index == -1)
            {
                item.Slot = slot;
                inventoryData.Add(item);
            }
            else
            {
                inventoryData[index].Slot += slot;
            }
        }
        else
        {
            ItemInventory item = new ItemInventory();
            item.Id = rawItem.Id;
            item.Name = rawItem.Name;
            item.Contents = rawItem.Contents;
            item.Rarity = rarity;
            item.Type = rawItem.Type;
            item.Slot = 1;
            item.ShopId = Random.Range(0, 999999);
            item.IsUse = 0;
            item.Level = 1;
            item.Stats_1 = 0;
            item.Stats_2 = 0;
            item.Stats_3 = 0;
            item.Stats_4 = 0;
            item.Stats_5 = 0;
            if ((item.Id - 10) % 4 == 0) { item.Stats_1 = 1000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 1) { item.Stats_1 = 4000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 2) { item.Stats_1 = 3000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 3) { item.Stats_1 = 2000 + randomStatsRarity(item.Rarity); }
            if (item.Rarity >= 1)
            {
                item.Stats_2 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 2)
            {
                item.Stats_3 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 3)
            {
                item.Stats_4 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 4)
            {
                item.Stats_5 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            inventoryData.Add(item);
        }
    }
    public void addNewItemByObject(ItemInventory item)
    {
        if (item.Type == 0 || item.Type == 10)
        {
            int index = fetchInventoryByIndex(item.Id);
            if (index == -1)
            {
                inventoryData.Add(item);
            }
            else
            {
                inventoryData[index].Slot += item.Slot;
            }
        }
        else
        {
            inventoryData.Add(item);
        }
    }
    public ItemInventory getItemObject(int id, int slot, int rarity)
    {
        ItemData rawItem = fetchItemById(id);
        // Add consume item
        if (rawItem.Type == 0 || rawItem.Type == 10)
        {
            ItemInventory item = new ItemInventory();
            item.Id = rawItem.Id;
            item.Name = rawItem.Name;
            item.Contents = rawItem.Contents;
            item.Rarity = rawItem.Rarity;
            item.Type = rawItem.Type;
            item.ShopId = 0;
            item.IsUse = 0;
            item.Level = 0;
            item.Stats_1 = 0;
            item.Stats_2 = 0;
            item.Stats_3 = 0;
            item.Stats_4 = 0;
            item.Stats_5 = 0;
            int index = fetchInventoryByIndex(id);
            item.Slot = slot;
            return item;
        }
        else
        {
            ItemInventory item = new ItemInventory();
            item.Id = rawItem.Id;
            item.Name = rawItem.Name;
            item.Contents = rawItem.Contents;
            item.Rarity = rarity;
            item.Type = rawItem.Type;
            item.Slot = 1;
            item.ShopId = Random.Range(0, 999999);
            item.IsUse = 0;
            item.Level = 1;
            item.Stats_1 = 0;
            item.Stats_2 = 0;
            item.Stats_3 = 0;
            item.Stats_4 = 0;
            item.Stats_5 = 0;

            if ((item.Id - 10) % 4 == 0) { item.Stats_1 = 1000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 1) { item.Stats_1 = 4000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 2) { item.Stats_1 = 3000 + randomStatsRarity(item.Rarity); }
            else if ((item.Id - 10) % 4 == 3) { item.Stats_1 = 2000 + randomStatsRarity(item.Rarity); }
            if (item.Rarity >= 1)
            {
                item.Stats_2 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 2)
            {
                item.Stats_3 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 3)
            {
                item.Stats_4 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            if (item.Rarity >= 4)
            {
                item.Stats_5 = Random.Range(1, 9) * 1000 + randomStatsRarity(item.Rarity);
            }
            return item;
        }
    }

    public ItemInventory dropItem()
    {
        int chance = Random.Range(0, 100);
        if(chance < 30)
        {
            int id = Random.Range(4, 10);
            return getItemObject(id, 1, 1);
        } else if(chance < 40)
        {
            int id = Random.Range(1, 4);
            return getItemObject(id, 1, 1);
        } else if(chance < 60)
        {
            int id = Random.Range(34, 38);
            return getItemObject(id, 1, 1);
        } else if (chance < 90)
        {
            int id = Random.Range(101, 113);
            return getItemObject(id, 1, 1);
        } else
        {
            int id = Random.Range(10, 34);
            return getItemObject(id, 1, returnRandomRarity());
        }
    }
    private int returnRandomRarity()
    {
        int chance = Random.Range(0, 100);
        if(chance <= 60)
        {
            return 1;
        } else if(chance <= 85)
        {
            return 2;
        } else if(chance <= 95)
        {
            return 3;
        } else
        {
            return 4;
        }
    }
    public void removeItemEquipment(int shopId)
    {
        int index = fetchInventoryByShopIdIndex(shopId);
        if (index >= 0)
        {
            inventoryData.RemoveAt(index);
        }
    }
    public void addItemSlotById(int id, int slot)
    {
        int index = fetchInventoryByIndex(id);
        inventoryData[index].Slot += slot;
        
    }
    public void reduceItemSlotById(int id, int slot)
    {
        int index = fetchInventoryByIndex(id);
        if (index >= 0 && inventoryData[index].Slot >= slot)
        {
            inventoryData[index].Slot -= slot;
        }
    }
    public bool canReduceItemSlotEvol(int id, int slot)
    {
        if (fetchInventoryById(id).Slot >= slot)
        {
            return true;
        }
        return false;
    }
    public void fuseItemByShopId(int shopId)
    {
        if (fetchInventoryById(8).Slot <= 0)
        {           
            addNewItem(8, fetchInventoryByShopId(shopId).Rarity + 1);
        }
        else
        {   
            addItemSlotById(8, fetchInventoryByShopId(shopId).Rarity + 1);
        }
        int index = fetchInventoryByShopIdIndex(shopId);
        inventoryData.RemoveAt(index);
        
    }
    public void upgradeItem(int shopId)
    {
        var item = fetchInventoryByShopIdIndex(shopId);

        if (inventoryData[item].Level < 20)
        {
            int goldRequire = (inventoryData[item].Level) * 500 + 500 * (inventoryData[item].Rarity - 1);
            int shardRequire = (inventoryData[item].Level) * 4 / 8 + (inventoryData[item].Rarity);
            UserDatabase.Instance.reduceMoney(goldRequire, 0);
            reduceItemSlotById(5 + (inventoryData[item].Id - 10) % 4, shardRequire);
            int upgradeValue = getUpgradeValue(inventoryData[item].Rarity);
            inventoryData[item].Level++;
            inventoryData[item].Stats_1 += upgradeValue;
            inventoryData[item].Stats_2 += upgradeValue;
            inventoryData[item].Stats_3 += upgradeValue;
            inventoryData[item].Stats_4 += upgradeValue;
            inventoryData[item].Stats_5 += upgradeValue;
        }
    }
    private int getUpgradeValue(int rarity)
    {
        if(rarity == 1 || rarity == 2)
        {
            return 2;
        } else if(rarity == 3 || rarity == 4)
        {
            return 3;
        } else
        {
            return 4;
        }
    }

    public List<ItemInventory> getAllData()
    {
        return inventoryData;
    }

    public List<ItemInventory> getEquipmentData(bool isSort)
    {
        List<ItemInventory> itemInv = new List<ItemInventory>();
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if(inventoryData[i].Type > 0 && inventoryData[i].Type < 10 && inventoryData[i].IsUse >= 0)
            {
                itemInv.Add(inventoryData[i]);
            }
        }
        if (isSort)
        {
            itemInv = sortByLevel(itemInv);
        }
        return itemInv;
    }
    private List<ItemInventory> sortByLevel(List<ItemInventory> arr)
    {
        for(int i = 0; i < arr.Count; i++)
        {
            int max = arr[i].Level;
            int index = i;
            for(int j = i; j < arr.Count; j++)
            {
                if (arr[j].Level >= max)
                {
                    max = arr[j].Level;
                    index = j;
                }
            }
            if (index != i)
            {
                ItemInventory cache = arr[i];
                arr[i] = arr[index];
                arr[index] = cache;
            }
        }
        return arr;
    }
    public void craftItem(int shopId)
    {
        int index = fetchInventoryByShopIdIndex(shopId);
        if (index >= 0)
        {
            inventoryData[index].IsUse = -10;
        }
    }
    public void unCraftItem(int shopId)
    {
        int index = fetchInventoryByShopIdIndex(shopId);
        if (index >= 0)
        {
            inventoryData[index].IsUse = 0;
        }
    }
    public void unCraftAllItem()
    {
        for(int i = 0; i < inventoryData.Count; i++) {
            if (inventoryData[i].IsUse == -10)
            {
                inventoryData[i].IsUse = 0;
            }
        }
    }
    public List<ItemInventory> getMaterialData()
    {
        List<ItemInventory> itemInv = new List<ItemInventory>();
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].Type == 0)
            {
                itemInv.Add(inventoryData[i]);
            }
        }
        return itemInv;
    }
    public List<ItemInventory> getShardData()
    {
        List<ItemInventory> itemInv = new List<ItemInventory>();
        for (int i = 0; i < inventoryData.Count; i++)
        {
            if (inventoryData[i].Type == 10)
            {
                itemInv.Add(inventoryData[i]);
            }
        }
        return itemInv;
    }
    public void equipItemPosition(ItemInventory item)
    {
        for (int i = 0; i < inventoryData.Count; i++)
        {
            // gỡ đồ đang mặc (nếu có)
            if (inventoryData[i].IsUse == -(item.Type))
            {
                inventoryData[i].IsUse = 0;
                break;
            }
        }
        // Mặc đồ 
        int index = fetchInventoryByShopIdIndex(item.ShopId);
        inventoryData[index].IsUse = -item.Type;
    }
    public void unequipItem(int shopId)
    {
        int index = fetchInventoryByShopIdIndex(shopId);
        if (index >= 0)
        {
            inventoryData[index].IsUse = 0;
        }   
    }
    private void OnApplicationQuit()
    {
        unCraftAllItem();
        Save();
    }
}

public class ItemInventory : ItemData
{
    public int Slot { get; set; }
    public int ShopId { get; set; }
    public int IsUse { get; set; }
    public int Stats_1 { get; set; }
    public int Stats_2 { get; set; }
    public int Stats_3 { get; set; }
    public int Stats_4 { get; set; }
    public int Stats_5 { get; set; }
    public int Level { get; set; }
}
public class ItemData
{
    public int Id { get; set; } 
    public string Name { get; set; }
    public string Contents { get; set; }
    public int Rarity { get; set; }
    public int Type { get; set; }
}
public class UserInformation
{
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Move { get; set; }
    public int Crit { get; set; }
    public int AttackSpeed { get; set; }
    public int ExGold { get; set; }
    public int ExExp { get; set; }
}