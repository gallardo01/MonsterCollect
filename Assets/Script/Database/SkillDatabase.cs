using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;

public class SkillDatabase : Singleton<SkillDatabase>
{
    private List<SkillData> SkillData = new List<SkillData>();
    private List<SkillInGame> SkillInGame = new List<SkillInGame>();

    private JsonData SkillDataJson;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        string fileName = "Skill.txt";
        LoadResourceTextfileSkillData(fileName);
    }

    private void LoadResourceTextfileSkillData(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        SkillDataJson = JsonMapper.ToObject(targetFile.text);
        ConstructSkillDb();
    }

    private void ConstructSkillDb()
    {
        for (int i = 0; i < SkillDataJson.Count; i++)
        {
            SkillData newItem = new SkillData();
            newItem.Skill = SkillDataJson[i]["Skill"].ToString();
            newItem.Content = SkillDataJson[i]["Content"].ToString();
            newItem.Second = SkillDataJson[i]["Second"].ToString();
            newItem.Id = (int)SkillDataJson[i]["Id"];
            newItem.Power = (int)SkillDataJson[i]["Power"];
            newItem.Timer = (int)SkillDataJson[i]["Timer"];
            newItem.Upgrade = (int)SkillDataJson[i]["Upgrade"];
            newItem.Level = (int)SkillDataJson[i]["Level"];

            SkillData.Add(newItem);

            SkillInGame newSkill = new SkillInGame();
            newSkill.isTrigger = false;
            newSkill.timerGame = 0f;
            newSkill.powerGame = 0;
            newSkill.data = newItem;
            SkillInGame.Add(newSkill);
        }
    }

    // Fetch trong DB goc'
    public SkillData fetchSkillIndex(int id)
    {
        for (int i = 0; i < SkillData.Count; i++)
        {
            if (SkillData[i].Id == id)
            {
                return SkillData[i];
            }
        }
        return null;
    }
    public SkillInGame fetchSkillIngame(int id)
    {
        for (int i = 0; i < SkillInGame.Count; i++)
        {
            if (SkillInGame[i].data.Id == id)
            {
                return SkillInGame[i];
            }
        }
        return null;
    }
}

public class SkillInGame
{
    public bool isTrigger { get; set; }
    public float timerGame { get; set; }
    public int powerGame { get; set; }
    public SkillData data { get; set; }
}

public class SkillData
{
    public string Skill { get; set; }
    public string Content { get; set; }
    public string Second { get; set; }
    public int Id { get; set; }
    public int Power { get; set; }
    public int Timer { get; set; }
    public int Upgrade { get; set; }
    public int Level { get; set; }
}
