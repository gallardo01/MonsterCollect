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
    private JsonData myHeroesJson;

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
        var heroes = SyncService.Instance.GetHeroes();

        if (heroes == null || heroes.Count == 0)
        {
            Debug.Log("Loading heroes from file...");
            firstTimeSetUp();
            SyncService.Instance.PushHeroes(myHeroes);
        }
        else
        {
            Debug.Log("Using heroes from cloud...");
            myHeroes = heroes;
        }
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
    }


    private void LoadResourceTextfileMyHeroes(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        myHeroesJson = JsonMapper.ToObject(targetFile.text);
        Debug.Log($"Loaded heroes: {myHeroesJson}");
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
    public MyHeroes fetchLastestEvolve(int id)
    {
        for(int i = myHeroes.Count - 1; i >= 0; i--)
        {
            if (myHeroes[i].Id/10 == id)
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
        return -1;
    }
    public int getEvolveStone(int id)
    {
        if(id % 10 == 0)
        {
            if(getTotalEvolve(id) == 3) { return 10; }
            else { return 5; }
        } else if(id % 10 == 1)
        {
            if (getTotalEvolve(id) == 3) { return 20; }
            else if(getTotalEvolve(id) == 4) { return 15; }
            else { return 10; }
        } else if(id % 10 == 2)
        {
            if (getTotalEvolve(id) == 3) { return 0; }
            else if (getTotalEvolve(id) == 4) { return 20; }
            else { return 15; }
        } else if(id % 10 == 3)
        {
            if (getTotalEvolve(id) == 3) { return 0; }
            else if (getTotalEvolve(id) == 4) { return 0; }
            else { return 20; }
        } else
        {
            return 0;
        }
    }
    private int getTotalEvolve(int id)
    {
        int num = 1;
        int idMonster = id / 10;
        for(int i = 1; i < 5; i++)
        {
            if(fetchMyHeroes(idMonster*10 + i) != null)
            {
                num++;
            } else
            {
                return num;
            }
        }
        return num;
    }
    // unlock all heroes
    public void unlockAllHeroes()
    {
        for(int i = 1; i <= 12; i++)
        {
            if (fetchHeroesIndex(i * 10) >= 0)
            {
                int index = fetchHeroesIndex(i * 10);
                if (myHeroes[index].Level == 0)
                {
                    myHeroes[index].Level = 1;
                }
            }
        }
    }
    // unlock heroes
    public void unlockHero(int id)
    {
        if (fetchHeroesIndex(id) >= 0)
        {
            int index = fetchHeroesIndex(id);
            myHeroes[index].Level = 1;
        }
    }
    // run 1 2 3 4 5
    public bool isUnlockHero(int id)
    {
        if (fetchHeroesIndex(id*10) >= 0)
        {
            int index = fetchHeroesIndex(id * 10);
            if (myHeroes[index].Level > 0)
            {
                return true;
            }
            return false;
        }
        return false;
    }
    public bool canEvolve(int id)
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
        for (int i = myHeroes.Count-1; i >= 0; i--)
        {
            if (myHeroes[i].Id/10 == id && myHeroes[i].Level > 0)
            {
                return myHeroes[i];
            }
        }
        return fetchMyHeroes(id*10);
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

        Debug.Log($"Preparing to push heroes: {jsonData}");
        SyncService.Instance.PushHeroes(myHeroes);
    }
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

