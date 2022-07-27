using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;

public class UserDatabase : Singleton<UserDatabase>
{
    private UserData database = new UserData();
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
        gainMoney(50000, 10000);
    }

    private void firstTimeSetUp()
    {

        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "UserData.txt";
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
            string fileName = "User.txt";
            LoadResourceTextfileItemData(fileName);
            ConstructItemDatabase();
            Save();
        }

    }
    private void ConstructItemDatabase()
    {
        UserData newItem = new UserData();
        newItem.Name = userData["Name"].ToString();
        newItem.Gold = (int)userData["Gold"];
        newItem.Diamond = (int)userData["Diamond"];
        newItem.Level = (int)userData["Level"];
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
        string tempPath = Application.persistentDataPath + "/c/b/c" + "/UserData.txt";
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
    public UserData getUserData()
    {
        return database;
    }

    public void gainMoney(int gold, int diamond)
    {
        database.Gold += gold;
        database.Diamond += diamond;
        Save();
    }
    public bool reduceMoney(int gold, int diamond)
    {
        if (database.Gold < gold || database.Diamond < diamond)
        {
            return false;
        }
        database.Gold -= gold;
        database.Diamond -= diamond;
        UIController.Instance.InitUI();
        Save();
        return true;
    }
    public int getTotalLevel()
    {
        return (database.Atk + database.Hp + database.Armour + database.Speed + database.BonusExp + database.BonusGold - 5);
    }
    public void gainLevel(int type)
    {
        switch(type)
        {
            case 1:
                database.Atk++;
                break;
            case 2:
                database.Hp++;
                break;
            case 3:
                database.Armour++;
                break;
            case 4:
                database.Speed++;
                break;
            case 5:
                database.BonusExp++;
                break;
            case 6:
                database.BonusGold++;
                break;
        }
        Save();
    }
}
public class UserData
{
    public string Name { get; set; }
    public int Gold { get; set; }
    public int Diamond { get; set; }
    public int Level { get; set; }

    public int HeroesPick { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
    public int BonusExp { get; set; }
    public int BonusGold { get; set; }

}
