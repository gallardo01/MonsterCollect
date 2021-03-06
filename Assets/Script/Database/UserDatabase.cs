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

    // Start is called before the first frame update
    void Start()
    {
        firstTimeSetUp();
        LoadResourceTextfileCurrentData();
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
        Save();
        return true;
    }
}
public class UserData
{
    public string Name { get; set; }
    public int Gold { get; set; }
    public int Diamond { get; set; }
    public int Level { get; set; }

    public int atk { get; set; }
    public int hp { get; set; }
    public int armour { get; set; }
    public int speed { get; set; }
    public int bonus { get; set; }

}
