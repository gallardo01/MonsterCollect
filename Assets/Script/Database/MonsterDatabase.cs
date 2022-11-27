using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;

public class MonsterDatabase : Singleton<MonsterDatabase>
{
    private List<MonsterData> monsterData = new List<MonsterData>();

    private JsonData monsterDataJson;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        string fileName = "Monster.txt";
        //firstTimeSetUp();

        LoadResourceTextfilemonsterData(fileName);
    }

    private void LoadResourceTextfilemonsterData(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        monsterDataJson = JsonMapper.ToObject(targetFile.text);
        ConstructMonsterDb();
    }

    private void ConstructMonsterDb()
    {
        for (int i = 0; i < monsterDataJson.Count; i++)
        {
            MonsterData newItem = new MonsterData();
            newItem.Name = monsterDataJson[i]["Name"].ToString();
            newItem.Id = (int)monsterDataJson[i]["No"];
            newItem.Atk = (int)monsterDataJson[i]["Atk"];
            newItem.Hp = (int)monsterDataJson[i]["Hp"];
            newItem.Armour = (int)monsterDataJson[i]["Armour"];
            newItem.Speed = (int)monsterDataJson[i]["Speed"];
            newItem.Type = (int)monsterDataJson[i]["Type"];
            monsterData.Add(newItem);
        }
    }

    // Fetch trong DB goc'
    public MonsterData fetchMonsterIndex(int id)
    {
        for (int i = 0; i < monsterData.Count; i++)
        {
            if (monsterData[i].Id == id)
            {
                return monsterData[i];
            }
        }
        return null;
    }
}

public class MonsterData
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
}
