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
    private List<HeroesData> heroesData = new List<HeroesData>();
    private List<MyHeroes> myHeroes = new List<MyHeroes>();

    private JsonData heroesDataJson;
    private JsonData myHeroesJson;

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        string fileName = "Heroes.txt";
        string myFileName = "MyHeroes.txt";
        //firstTimeSetUp();

        LoadResourceTextfileHeroesData(fileName);
        LoadResourceTextfileCurrentData(myFileName);
        //if (!PlayerPrefs.HasKey("HeroesPick"))
        //{
        //    unlockHero(10);
        //    PlayerPrefs.SetInt("HeroesPick", 10);
        //}

        unlockHero(10);
        PlayerPrefs.SetInt("HeroesPick", 10);

    }

    private void firstTimeSetUp()
    {
        string tempPath = Application.persistentDataPath + "/c/b/c/";
        string filePath = tempPath + "MyHeroes.txt";
        if (!Directory.Exists(Path.GetDirectoryName(tempPath)))
        {
            Directory.CreateDirectory(Path.GetDirectoryName(tempPath));
        }
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
        //add hero dau tien
    }


    private void LoadResourceTextfileHeroesData(string path)
    {
        string filePath = "StreamingAssets/" + path.Replace(".txt", "");
        TextAsset targetFile = Resources.Load<TextAsset>(filePath);
        heroesDataJson = JsonMapper.ToObject(targetFile.text);
        ConstructHeroesDb();
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
    private void ConstructHeroesDb()
    {
        for (int i = 0; i < heroesDataJson.Count; i++)
        {
            HeroesData newItem = new HeroesData();
            newItem.Name = heroesDataJson[i]["Name"].ToString();
            newItem.Id = (int)heroesDataJson[i]["Id"];
            newItem.Atk = (int)heroesDataJson[i]["Atk"];
            newItem.Hp = (int)heroesDataJson[i]["Hp"];
            newItem.Armour = (int)heroesDataJson[i]["Armour"];
            newItem.Speed = (int)heroesDataJson[i]["Speed"];
            newItem.Crit = (int)heroesDataJson[i]["Crit"];
            newItem.Spell = (int)heroesDataJson[i]["Spell"];
            newItem.Type = (int)heroesDataJson[i]["Type"];
            newItem.Skill = heroesDataJson[i]["Skill"].ToString();
            heroesData.Add(newItem);
        }
    }

    private void ConstructMyHeroes()
    {
        for (int i = 0; i < myHeroesJson.Count; i++)
        {
            MyHeroes newItem = new MyHeroes();
            newItem.Name = myHeroesJson[i]["Name"].ToString();
            newItem.Id = (int)myHeroesJson[i]["Id"];
            //newItem.Unlock = (int)userData[i]["Unlock"];
            newItem.Level = (int)myHeroesJson[i]["Level"];
            newItem.Atk = (int)myHeroesJson[i]["Atk"];
            newItem.Hp = (int)myHeroesJson[i]["Hp"];
            newItem.Armour = (int)myHeroesJson[i]["Armour"];
            newItem.Speed = (int)myHeroesJson[i]["Speed"];
            newItem.Crit = (int)myHeroesJson[i]["Crit"];
            newItem.Spell = (int)myHeroesJson[i]["Spell"];
            newItem.Type = (int)myHeroesJson[i]["Type"];
            newItem.Skill = myHeroesJson[i]["Skill"].ToString();
            myHeroes.Add(newItem);
        }
    }

    // Fetch trong DB goc'
    public HeroesData fetchHeroesData(int id)
    {
        for(int i = 0; i < heroesData.Count; i++)
        {
            if(heroesData[i].Id == id)
            {
                return heroesData[i];
            }
        }
        return null;
    }

    // Fetch trong DB goc'
    public int fetchHeroesIndex(int id)
    {
        for (int i = 0; i < heroesData.Count; i++)
        {
            if (heroesData[i].Id == id)
            {
                return i;
            }
        }
        return 0;
    }
    // Fetch trong DB so huu
    public HeroesData fetchMyData(int id)
    {
        for (int i = 0; i < myHeroes.Count; i++)
        {
            if (myHeroes[i].Id == id)
            {
                return myHeroes[i];
            }
        }
        return null;
    }

    // Fetch trong DB so huu
    public int fetchMyIndex(int id)
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
        if (fetchMyData(id) == null)
        {
            HeroesData raw = fetchHeroesData(id);
            MyHeroes addNew = new MyHeroes();


            addNew.Name = raw.Name;
            addNew.Id = raw.Id;
            addNew.Hp = raw.Hp;
            addNew.Armour = raw.Armour;
            addNew.Speed = raw.Speed;
            addNew.Spell = raw.Spell;
            addNew.Type = raw.Type;
            addNew.Skill = raw.Skill;
            addNew.Crit = raw.Crit;
            addNew.Atk = raw.Atk;
            addNew.Level = 1;

            myHeroes.Add(addNew);
            Save();
        }

    }

    public void evolveHero(int id)
    {
        if (fetchMyData(id) != null && canEvolve(id))
        {
            HeroesData raw = fetchHeroesData(id+1);
            int pos = fetchMyIndex(id);
            myHeroes[pos].Id += 1;
            // update chi so
            myHeroes[pos].Atk = raw.Atk * (myHeroes[pos].Level*5 + 100) / 100; // bonus
            //
            Save();
        }
    }

    public void levelUpHero(int id)
    {
        int pos = fetchMyIndex(id);
        myHeroes[pos].Level += 1;
    }

    private bool canEvolve(int id)
    {
        if(fetchHeroesData(id+1) != null)
        {
            return true;
        }
        return false;
    }

    // tao ra list 
    public List<HeroesData> fetchAllEvolveHero(int data)
    {
        List<HeroesData> listHero = new List<HeroesData>();
        //Debug.Log(database.Count);
        for (int i = 0; i < heroesData.Count; i++)
        {
            if (heroesData[i].Id /10 == data/10)
            {
                listHero.Add(heroesData[i]);
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

    
    public HeroesData getCurrentHero(int id)
    {
        for (int i = heroesData.Count - 1; i >=0 ; i--)
        {
            //if (database[i].Id/10 == id && database[i].Unlock == 1)
            //{
            //    return database[i];
            //}
        }
        return fetchHeroesData(id*10);
    }

    public void Save()
    {
        string jsonData = JsonConvert.SerializeObject(myHeroesJson, Formatting.Indented);

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


public class MyHeroes : HeroesData
{
    public int Level { get; set; }
}

public class HeroesData
{
    public string Name { get; set; }
    public int Id { get; set; }
    public int Type { get; set; }
    public int Atk { get; set; }
    public int Hp { get; set; }
    public int Armour { get; set; }
    public int Speed { get; set; }
    public int Crit { get; set; }
    public int Spell { get; set; }
    public string Skill { get; set; }

}
