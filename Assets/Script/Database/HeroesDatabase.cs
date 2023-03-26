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
        firstTimeSetUp();

        LoadResourceTextfileHeroesData(fileName);
        if (!PlayerPrefs.HasKey("HeroesPick"))
        {
            unlockHero(10);
            unlockHero(11);

            PlayerPrefs.SetInt("HeroesPick", 10);
        }

        LoadResourceTextfileCurrentData(myFileName);
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
            newItem.Move = (int)heroesDataJson[i]["Move"];
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
    public MyHeroes fetchMyData(int id)
    {
        for (int i = 0; i < myHeroes.Count; i++)
        {
            Debug.Log(myHeroes[i].Id);
            if (myHeroes[i].Id == id)
            {
                return myHeroes[i];
            }
        }
        return null;
    }

    public MyHeroes fetchMyDataLastest(int id)
    {
        for (int i = 0; i < myHeroes.Count; i++)
        {
            if (myHeroes[i].Id/10 == id/10)
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
            addNew.Move = raw.Move;
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
            myHeroes[pos].Atk = raw.Atk * ((myHeroes[pos].Level-1)*5 + 100) / 100;
            myHeroes[pos].Hp = raw.Hp * ((myHeroes[pos].Level-1) *5 + 100) / 100;
            myHeroes[pos].Armour = raw.Armour * ((myHeroes[pos].Level-1) * 5 + 100) / 100;
            myHeroes[pos].Speed = raw.Speed * ((myHeroes[pos].Level-1) * 5 + 100) / 100;
            myHeroes[pos].Crit = raw.Crit * ((myHeroes[pos].Level-1) * 5 + 100) / 100;
            myHeroes[pos].Move = raw.Move * ((myHeroes[pos].Level-1) * 5 + 100) / 100;
            myHeroes[pos].Name = raw.Name;

            Save();
        }
    }

    public void levelUpHero(int id)
    {
        int pos = fetchMyIndex(id);

        HeroesData raw = fetchHeroesData(id);
        // update chi so
        myHeroes[pos].Atk = raw.Atk * (myHeroes[pos].Level * 5 + 100) / 100;
        myHeroes[pos].Hp = raw.Hp * (myHeroes[pos].Level * 5 + 100) / 100;
        myHeroes[pos].Armour = raw.Armour * (myHeroes[pos].Level * 5 + 100) / 100;
        myHeroes[pos].Speed = raw.Speed * (myHeroes[pos].Level * 5 + 100) / 100;
        myHeroes[pos].Crit = raw.Crit * (myHeroes[pos].Level * 5 + 100) / 100;
        myHeroes[pos].Move = raw.Move * (myHeroes[pos].Level * 5 + 100) / 100;
        myHeroes[pos].Level += 1;

        Save();

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
        if(fetchHeroesData(id+1) != null)
        {
            return true;
        }
        return false;
    }

    public bool isUnlock(int id)
    {
        for (int i = 0; i < myHeroes.Count ; i++)
        {
            if (id/10 == myHeroes[i].Id/10)
            {
                return true;
            }
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
        //if (myHeroes.Count >= id)
        //{
        //    return fetchMyData(myHeroes[id - 1].Id);

        //}
        for (int i = 0; i < myHeroes.Count; i++)
        {
            if (myHeroes[i].Id/10 == id)
            {
                return fetchMyData(myHeroes[i].Id);
            }
        }
        return fetchHeroesData(id * 10);

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
    public int Move { get; set; }
    public string Skill { get; set; }

}
