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
        newItem.Level = (int)userData["Level"];
        newItem.Exp = (int)userData["Exp"];
        newItem.Cup = (int)userData["Cup"];
        newItem.Rank = (int)userData["Rank"];
        newItem.Gold = (int)userData["Gold"];
        newItem.Diamond = (int)userData["Diamond"];
        newItem.Shard = (int)userData["Shard"];
        newItem.Vip = (int)userData["Vip"];
        newItem.DiamondPurchased = (int)userData["DiamondPurchased"];
        newItem.BattlePass = (int)userData["BattlePass"];
        newItem.AttackBonus = (int)userData["AttackBonus"];
        newItem.SpecialAtkBonus = (int)userData["SpecialAtkBonus"];
        newItem.DefenseBonus = (int)userData["DefenseBonus"];
        newItem.SpecialDefBonus = (int)userData["SpecialDefBonus"];
        newItem.HpBonus = (int)userData["HpBonus"];
        newItem.SpeedBonus = (int)userData["SpeedBonus"];
        newItem.SuperEffectBonus = (int)userData["SuperEffectBonus"];

        database = newItem;
    }
    private void LoadResourceTextfileCurrentData()
    {
        string tempPath = Application.persistentDataPath + "/c/b/c" + "/UserData.txt";
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
    public void gainVipPoint(int point)
    {
        database.DiamondPurchased += point;
        Save();
    }
    public void setupVipValue(int val)
    {
        database.Vip = val;
        Save();
    }
    public void gainMoney(int gold, int diamond, int shard)
    {
        database.Gold += gold;
        database.Diamond += diamond;
        database.Shard += shard;
        Save();
    }
    public bool reduceMoney(int gold, int diamond, int shard)
    {
        if (database.Gold < gold || database.Diamond < diamond || database.Shard < shard)
        {
            return false;
        }
        database.Gold -= gold;
        database.Diamond -= diamond;
        database.Shard -= shard;
        Save();
        return true;
    }
    public void setBonusDataInfo(int atk, int sAtk, int def, int sDef, int hp, int speed, int superEff, int notEff)
    {
        database.AttackBonus = atk;
        database.SpecialAtkBonus = sAtk;
        database.DefenseBonus = def;
        database.SpecialDefBonus = sDef;
        database.HpBonus = hp;
        database.SpecialDefBonus = speed;
        database.SuperEffectBonus = superEff;
        database.NotEffectBonus = notEff;
        Save();
    }

}
public class UserData
{
    public string Name { get; set; }
    public int Level { get; set; }
    public int Exp { get; set; }
    public int Cup { get; set; }
    public int Rank { get; set; }
    public int Gold { get; set; }
    public int Diamond { get; set; }
    public int Shard { get; set; }
    public int Vip { get; set; }
    public int DiamondPurchased { get; set; }
    public int BattlePass { get; set; }
    public int AttackBonus { get; set; }
    public int SpecialAtkBonus { get; set; }
    public int DefenseBonus { get; set; }
    public int SpecialDefBonus { get; set; }
    public int HpBonus { get; set; }
    public int SpeedBonus { get; set; }
    public int SuperEffectBonus { get; set; }
    public int NotEffectBonus { get; set; }


}
