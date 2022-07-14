﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;

public class HeroesDatabase : Singleton<HeroesDatabase>
{
    private List<HeroesData> database = new List<HeroesData>();
    private JsonData userData;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        firstTimeSetUp();
        LoadResourceTextfileCurrentData();
    }

    private void firstTimeSetUp()
    {

        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "Heroes.txt";
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            string fileName = "Heroes.txt";
            LoadResourceTextfileItemData(fileName);
            ConstructItemDatabase();
            Save();
        }

    }
    private void ConstructItemDatabase()
    {
        for (int i = 0; i < userData.Count; i++)
        {
            HeroesData newItem = new HeroesData();
            newItem.Name = userData[i]["Name"].ToString();
            newItem.Id = (int)userData[i]["Id"];
            newItem.Unlock = (int)userData[i]["Unlock"];
            newItem.Atk = (int)userData[i]["Atk"];
            newItem.Hp = (int)userData[i]["Hp"];
            newItem.Armour = (int)userData[i]["Armour"];
            newItem.Speed = (int)userData[i]["Speed"];
            newItem.XpGain = (int)userData[i]["XpGain"];
            newItem.GoldGain = (int)userData[i]["GoldGain"];
            database.Add(newItem);
        }
    }
    public HeroesData fetchHeroesData(int id)
    {
        for(int i = 0; i < database.Count; i++)
        {
            if(database[i].Id == id)
            {
                return database[i];
            }
        }
        return null;
    }

    public int fetchHeroesIndex(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].Id == id)
            {
                return i;
            }
        }
        return 0;
    }

    public List<HeroesData> fetchAllEvolveHero(int data)
    {
        List<HeroesData> listHero = new List<HeroesData> ();
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].Id / 10 == data /10)
            {
                listHero.Add(database[i]);
            }
        }
        return listHero;
    }

    public bool buyHeroes(int id)
    {
        int cost = StaticInfo.costHeroes[id / 10];
        if(UserDatabase.Instance.reduceMoney(0, cost) == true)
        {
            unlockHero(id);
            Save();
            return true;
        }
        return false;
    }

    private void unlockHero(int id)
    {
        database[fetchHeroesIndex(id)].Unlock = 1;
    }

    public HeroesData getCurrentHero(int id)
    {
        for (int i = 0; i < database.Count; i++)
        {
            if (database[i].Id/10 == id && database[i].Unlock == 1)
            {
                return database[i];
            }
        }
        return fetchHeroesData(id*10);
    }

    private void LoadResourceTextfileCurrentData()
    {
        string tempPath = Application.persistentDataPath + "/c/b/c" + "/Heroes.txt";
        Debug.Log(tempPath);
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
        userData = JsonMapper.ToObject(jsonData);
        ConstructItemDatabase();
    }
    private void LoadResourceTextfileItemData(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        userData = JsonMapper.ToObject(targetFile.text);
        //ConstructItemDatabase();
    }
    public void Save()
    {
        string jsonData = JsonConvert.SerializeObject(database, Formatting.Indented);

        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "Heroes.txt";

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
}
public class HeroesData
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Unlock { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
    public int XpGain { get; set; }
    public int GoldGain { get; set; }

}
