using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using System;

public class HeroesDatabase : Singleton<HeroesDatabase>
{
    private List<MyHeroes> myHeroes = new List<MyHeroes>();
    private List<RealDataHeroes> dataHeroes = new List<RealDataHeroes>();
    private JsonData myHeroesJson;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        firstTimeSetUp();
    }

    private void firstTimeSetUp()
    {
        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "MyHeroes.txt";

        string fileName = "Heroes.txt";
        string myFileName = "MyHeroes.txt";
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        if (!File.Exists(filePath))
        {
            LoadResourceTextfileMyHeroes(fileName);
            File.Create(filePath).Close();
        } else
        {
            LoadResourceTextfileCurrentData(myFileName);
        }
        initRealDataHeroes();
    }


    private void LoadResourceTextfileMyHeroes(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        myHeroesJson = JsonMapper.ToObject(targetFile.text);
        ConstructMyHeroes();
    }
    private void LoadResourceTextfileCurrentData(string path)
    {
        string tempPath = Application.persistentDataPath + "/c/b/c" + "/MyHeroes.txt";
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
        myHeroesJson = JsonMapper.ToObject(jsonData);
        ConstructMyHeroes();
    }
    private void ConstructMyHeroes()
    {
        for (int i = 0; i < myHeroesJson.Count; i++)
        {
            MyHeroes newItem = new MyHeroes();
            newItem.Name = myHeroesJson[i]["Name"].ToString();
            newItem.Id = (int)myHeroesJson[i]["Id"];
            newItem.Level = (int)myHeroesJson[i]["Level"];
            newItem.Atk = (int)myHeroesJson[i]["Atk"];
            newItem.Hp = (int)myHeroesJson[i]["Hp"];
            newItem.Armour = (int)myHeroesJson[i]["Armour"];
            newItem.Speed = (int)myHeroesJson[i]["Speed"];
            newItem.Crit = (int)myHeroesJson[i]["Crit"];
            newItem.Move = (int)myHeroesJson[i]["Move"];
            newItem.Type = (int)myHeroesJson[i]["Type"];
            newItem.Skill = myHeroesJson[i]["Skill"].ToString();
            myHeroes.Add(newItem);
        }
    }
    public void initRealDataHeroes()
    {
        dataHeroes.Clear();
        for(int i = 1; i <= 12; i++)
        {
            if(getCurrentHero(i) != null)
            {
                RealDataHeroes data = new RealDataHeroes();
                MyHeroes raw = getCurrentHero(i);
                data.Atk = raw.Atk * (100 + (raw.Level - 1) * 5) / 100;
                data.Hp = raw.Hp * (100 + (raw.Level - 1) * 5) / 100;
                data.Armour = raw.Armour * (100 + (raw.Level - 1) * 5) / 100;
                data.Speed = raw.Speed * (100 + (raw.Level - 1) * 5) / 100;
                data.Move = raw.Move * (100 + (raw.Level - 1) * 5) / 100;
                data.Crit = raw.Crit * (100 + (raw.Level - 1) * 5) / 100;
                data.Type = raw.Type;
                data.Id = raw.Id;
            }
        }
    }
    public RealDataHeroes fetchRealData(int id)
    {
        for (int i = 0; i < dataHeroes.Count; i++)
        {
            if (dataHeroes[i].Id == id)
            {
                return dataHeroes[i];
            }
        }
        return null;
    }

    public MyHeroes fetchMyHeroes(int id)
    {
        for(int i = 0; i < myHeroes.Count; i++)
        {
            if(myHeroes[i].Id == id)
            {
                return myHeroes[i];
            }
        }
        return null;
    }
    public int fetchHeroesIndex(int id)
    {
        for (int i = 0; i < myHeroes.Count; i++)
        {
            if (myHeroes[i].Id == id)
            {
                return i;
            }
        }
        return 0;
    }
    // unlock heroes
    private void unlockHero(int id)
    {
        if (fetchMyHeroes(id) == null)
        {
            int index = fetchHeroesIndex(id);
            myHeroes[index].Level = 1;
        }
    }
    public void evolveHero(int id)
    {
        if (canEvolve(id))
        {
            int pos = fetchHeroesIndex(id);
            myHeroes[pos + 1].Level = myHeroes[pos].Level;
        }
        initRealDataHeroes();
    }
    public void levelUpHero(int id)
    {
        int pos = fetchHeroesIndex(id);
        myHeroes[pos].Level += 1;
        initRealDataHeroes();
        //tru tien
        reduceItemForLevelUpAndEvolve(myHeroes[pos]);
    }
    private void reduceItemForLevelUpAndEvolve(MyHeroes raw)
    {
        ItemDatabase.Instance.reduceItemSlotById(1, raw.Level / 3 +3);
        ItemDatabase.Instance.reduceItemSlotById(2, raw.Level / 5 + 2);
        ItemDatabase.Instance.reduceItemSlotById(3, raw.Level / 10 + 1);
        ItemDatabase.Instance.reduceItemSlotById(raw.Id/10+100, raw.Level +1);

        UserDatabase.Instance.reduceMoney(raw.Level *200, 0);
    }

    private bool canEvolve(int id)
    {
        if(fetchMyHeroes(id+1) != null)
        {
            return true;
        }
        return false;
    }
    // tao ra list 
    public List<MyHeroes> fetchAllEvolveHero(int data)
    {
        List<MyHeroes> listHero = new List<MyHeroes>();
        for (int i = 0; i < myHeroes.Count; i++)
        {
            if (myHeroes[i].Id /10 == data/10)
            {
                listHero.Add(myHeroes[i]);
            }
        }
        return listHero;
    }

    //
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
    public MyHeroes getCurrentHero(int id)
    {
        Debug.Log(myHeroes.Count);
        for (int i = myHeroes.Count-1; i >= 0; i--)
        {
            if (myHeroes[i].Id/10 == id && myHeroes[i].Level > 0)
            {
                return myHeroes[i];
            }
        }
        return fetchMyHeroes(id*10+1);
    }
    private void OnDestroy()
    {
        Save();
    }

    public void Save()
    {
        string jsonData = JsonConvert.SerializeObject(myHeroes, Formatting.Indented);

        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "MyHeroes.txt";

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

public class RealDataHeroes
{
    public int Id { get; set; }
    public int Type { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
    public int Crit { get; set; }
    public int Move { get; set; }
}

public class MyHeroes
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
    public int Crit { get; set; }
    public int Move { get; set; }
    public string Skill { get; set; }
    public int Level { get; set; }
}

