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
    }
    public void LoadData()
    {
        if (SyncService.Instance.getCloudStatus())
        {
            var user = SyncService.Instance.GetUser();
            if (user != null)
            {
                database = user;
                string tempPath = Application.persistentDataPath + "/c/b/c/";
                string filePath = tempPath + "UserData.txt";
                //Create Directory if it does not exist
                if (!File.Exists(filePath))
                {
                    SaveFile();
                }
            } else
            {
                firstTimeSetUp();
            }
        } else
        {
            firstTimeSetUp();
        }
    }
    private void firstTimeSetUp()
    {
        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "UserData.txt";

        string fileName = "User.txt";
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        if (!File.Exists(filePath))
        {
            LoadResourceTextfileItemData(fileName);
            Save();
        }
        else
        {
            LoadResourceTextfileCurrentData();
        }
    }

    private void ConstructItemDatabase()
    {
        database.Name = userData["Name"]?.ToString() ?? "Player";
        database.Gold = (int)userData["Gold"];
        database.Diamond = (int)userData["Diamond"];
        database.Level = (int)userData["Level"];
        database.HeroesPick = (int)userData["HeroesPick"];
        database.Atk = (int)userData["Atk"];
        database.Hp = (int)userData["Hp"];
        database.Armour = (int)userData["Armour"];
        database.Move = (int)userData["Move"];
        database.Crit = (int)userData["Crit"];
        database.Speed = (int)userData["Speed"];
        database.Equipment = (int)userData["Equipment"];
        database.ExtraGold = (int)userData["ExtraGold"];
        database.ExtraExp = (int)userData["ExtraExp"];
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
    public void setLevelMap(int level)
    {
        if (level > database.Level)
        {
            database.Level = level;
            PlayerPrefs.SetInt("Map", level);
            Save();
        }
    }
    private void LoadResourceTextfileItemData(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        userData = JsonMapper.ToObject(targetFile.text);

        ConstructItemDatabase();
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

        if (SyncService.Instance.getSynchronizeStatus())
        {
            SyncService.Instance.PushUser(database);
        }
    }
    public void SaveFile()
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
    public void deleteData()
    {
        string fileName = "User.txt";
        LoadResourceTextfileItemData(fileName);
        Save();
    }
    public UserData getUserData()
    {
        return database;
    }

    public void gainMoney(int gold, int diamond)
    {
        database.Gold += gold;
        database.Diamond += diamond;
        UIController.Instance.InitUI();
        Save();
    }

    public void gainMoneyInGame(int gold, int diamond)
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
        //tru trong db
        database.Gold -= gold;
        database.Diamond -= diamond;
        //tru tren UI
        UIController.Instance.InitUI();
        Save();
        return true;
    }
    public int getTotalLevel()
    {
        return (database.Atk + database.Hp + database.Armour + database.Move + database.Crit + database.Speed + database.Equipment + database.ExtraGold + database.ExtraExp);
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
                database.Move++;
                break;
            case 5:
                database.Crit++;
                break;
            case 6:
                database.Speed++;
                break;
            case 7:
                database.Equipment++;
                break;
            case 8:
                database.ExtraGold++;
                break;
            case 9:
                database.ExtraExp++;
                break;
        }
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
    public int Move { get; set; }
    public int Crit { get; set; }
    public int Speed { get; set; }
    public int Equipment { get; set; }
    public int ExtraGold { get; set; }
    public int ExtraExp { get; set; }
}
