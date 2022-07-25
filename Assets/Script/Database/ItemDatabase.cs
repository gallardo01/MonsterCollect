﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using LitJson;
using System.Text;
using System.IO;
using System;
using Random = UnityEngine.Random;

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
        string fileName = "Item.txt";
        string myFileName = "MyItem.txt";

        LoadResourceTextfileItemData(fileName);
        LoadResourceTextfileCurrentData(myFileName);


        //for (int i = 1; i <= 8; i++)
        //{
        //    addNewItem(i, 10);
        //}
        //Save();

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
            for (int i = 1; i < 9; i++)
            {
                addNewItem(i, 0);

            }
            Save();
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
    public List<ItemInventory> fetchAllData()
    {
        return inventoryData;
    }
    // common = 1 , uncommon = 2, rare = 3, mythical = 4, arcana = 5
    public void addNewItem(int id, int slot)
    {
        ItemData rawItem = fetchItemById(id);
        // Add consume item
        if (rawItem.Type == 0)
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
            Save();
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
            if (item.Rarity == 1)
            {
                int randomValue = getStatsItem(item.Type);
                int randomStat = Random.Range(1, 3);
                item.Stats_1 = randomValue * 100 + randomStat;
            } else if(item.Rarity == 2)
            {
                int randomValue = getStatsItem(item.Type);
                int randomStat = Random.Range(1, 4);
                item.Stats_1 = randomValue * 100 + randomStat;
            }
            else if(item.Rarity == 3)
            {
                int randomValue = getStatsItem(item.Type);
                int randomStat = Random.Range(1, 4);
                item.Stats_1 = randomValue * 100 + randomStat;

                int randomValue1 = getStatsItem(item.Type);
                int randomStat1 = Random.Range(1, 4);
                item.Stats_2 = randomValue1 * 100 + randomStat1;
            }
            else
            {
                int randomValue = getStatsItem(item.Type);
                int randomStat = Random.Range(1, 4);
                item.Stats_1 = randomValue * 100 + randomStat;

                int randomValue1 = getStatsItem(item.Type);
                int randomStat1 = Random.Range(1, 4);
                item.Stats_2 = randomValue1 * 100 + randomStat1;

                int randomValue2 = getStatsItem(item.Type);
                int randomStat2 = Random.Range(1, 4);
                item.Stats_3 = randomValue2 * 100 + randomStat2;
            }
            inventoryData.Add(item);
        }
        Save();
    }

    private int getStatsItem(int type)
    {
        if(type == 1)
        {
            return StaticInfo.statsItem1[Random.Range(0, 4)];
        } else if(type == 2)
        {
            return StaticInfo.statsItem2[Random.Range(0, 4)];
        } else if(type == 3)
        {
            return StaticInfo.statsItem3[Random.Range(0, 4)];
        }
        return StaticInfo.statsItem4[Random.Range(0, 4)];
    }
    public void reduceItemSlot(int shopId, int slot)
    {
        int index = fetchInventoryByShopIdIndex(shopId);
        inventoryData[index].Slot -= slot;
        if (inventoryData[index].Slot <= 0)
        {
            inventoryData.RemoveAt(index);
        }
        Save();
    }
    public void reduceItemSlotById(int id, int slot)
    {
        int index = fetchInventoryByIndex(id);
        if (index >= 0)
        {
            inventoryData[index].Slot -= slot;
            //if (inventoryData[index].Slot <= 0)
            //{
            //    inventoryData.RemoveAt(index);
            //}
            Save();
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

    public List<ItemInventory> getAllData()
    {
        return inventoryData;
    }

    //public void equipItemPosition(ItemInventory item)
    //{
    //    for (int i = 0; i < inventoryData.Count; i++)
    //    {
    //        if (inventoryData[i].Ability == -(item.Type))
    //        {
    //            inventoryData[i].Ability = 0;
    //            break;
    //        }
    //    }
    //    int index = fetchInventoryByShopIdIndex(item.ShopId);
    //    inventoryData[index].Ability = -item.Type;
    //}
    //public void unequipItem(int shopId)
    //{
    //    int index = fetchInventoryByShopIdIndex(shopId);
    //    if (index >= 0)
    //    {
    //        inventoryData[index].Ability = 0;
    //    }
    //}
}

public class ItemInventory : ItemData
{
    public int Slot { get; set; }
    public int ShopId { get; set; }
    public int IsUse { get; set; }
    public int Stats_1 { get; set; }
    public int Stats_2 { get; set; }
    public int Stats_3 { get; set; }
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

