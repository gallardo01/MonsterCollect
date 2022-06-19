﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;

public class HeroesDatabase : Singleton<UserDatabase>
{
    private HeroesData database = new HeroesData();
    private JsonData userData;

    // Start is called before the first frame update
    void Start()
    {
        firstTimeSetUp();
        LoadResourceTextfileCurrentData();
    }

    private void firstTimeSetUp()
    {

        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "HeroesData.txt";
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            string fileName = "HeroesData.txt";
            LoadResourceTextfileItemData(fileName);
            ConstructItemDatabase();
            Save();
        }

    }
    private void ConstructItemDatabase()
    {
        HeroesData newItem = new HeroesData();
        newItem.Name = userData["Name"].ToString();
        newItem.Unlock = (int)userData["Level"];
        newItem.HeroesPick = (int)userData["HeroesPick"];
        newItem.Atk = (int)userData["Atk"];
        newItem.Hp = (int)userData["Hp"];
        newItem.Armour = (int)userData["Armour"];
        newItem.Speed = (int)userData["Speed"];
        newItem.BonusExp = (int)userData["BonusExp"];
        newItem.BonusGold = (int)userData["BonusGold"];
        database = newItem;
    }
    private void LoadResourceTextfileCurrentData()
    {
        string tempPath = Application.persistentDataPath + "/c/b/c" + "/HeroesData.txt";
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
        string filePath = tempPath + "UserData.txt";

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
    public HeroesData getUserData()
    {
        return database;
    }
}
public class HeroesData
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Unlock { get; set; }
    public int HeroesPick { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
    public int BonusExp { get; set; }
    public int BonusGold { get; set; }

}
