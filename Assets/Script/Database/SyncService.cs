using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Newtonsoft.Json;

public class SyncService : Singleton<SyncService>
{
    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private const string cloudInventoryKey = "inventory";
    private const string cloudUserKey = "user";
    private const string cloudHeroesKey = "heroes";

    private CloudData cloudData = null;
    private bool isActiveCloudData = false;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("checking cloud service availability...");
        if (CloudServices.IsAvailable())
        {
            Debug.Log("starting to synchronize...");
            CloudServices.Synchronize();
        }
    }

    // Returns whether cloud data has been retrieved, or if cloud service is
    // not available (due to no internet connection)
    public bool HasRetrievedCloudData() => cloudData != null || !CloudServices.IsAvailable();

    private void OnEnable()
    {
        // register for events
        CloudServices.OnUserChange += OnUserChange;
        CloudServices.OnSavedDataChange += OnSavedDataChange;
        CloudServices.OnSynchronizeComplete += OnSynchronizeComplete;
    }

    private void OnDisable()
    {
        // unregister from events
        CloudServices.OnUserChange -= OnUserChange;
        CloudServices.OnSavedDataChange -= OnSavedDataChange;
        CloudServices.OnSynchronizeComplete -= OnSynchronizeComplete;
    }

    private void OnUserChange(CloudServicesUserChangeResult result, Error error)
    {
        Debug.Log(result.User);
    }


    private void OnSynchronizeComplete(CloudServicesSynchronizeResult result)
    {
        RetrieveCloudData();
    }
    public bool getCloudStatus()
    {
        return isActiveCloudData;
    }
    private void RetrieveCloudData()
    {
        string inventoryJson = CloudServices.GetString(cloudInventoryKey);
        string userJson = CloudServices.GetString(cloudUserKey);
        string heroesJson = CloudServices.GetString(cloudHeroesKey);

        if ( inventoryJson != null && userJson != null && heroesJson != null)
        {
            isActiveCloudData = true;
        }

        CloudData data = new()
        {
            inventory = inventoryJson == null
                ? null
                : JsonConvert.DeserializeObject<List<ItemInventory>>(inventoryJson),

            user = userJson == null
                ? null
                : JsonConvert.DeserializeObject<UserData>(userJson),

            heroes = heroesJson == null
                ? null
                : JsonConvert.DeserializeObject<List<MyHeroes>>(heroesJson),
        };

        cloudData = data;
        //Debug.Log("Cloud synchronization completed:");
        //Debug.Log($" - inventory: {inventoryJson}");
        //Debug.Log($" - user: {userJson}");
        //Debug.Log($" - heroes: {heroesJson}");
    }

    private void OnSavedDataChange(CloudServicesSavedDataChangeResult result)
    {
        // Data changed externally from another device

    }

    public List<ItemInventory> GetInventory() => cloudData?.inventory;

    public UserData GetUser() => cloudData?.user;

    public List<MyHeroes> GetHeroes() => cloudData?.heroes;

    public void PushInventory(List<ItemInventory> data)
    {
        if (data == null || data.Count == 0) return;

        string jsonData = JsonConvert.SerializeObject(data);
        //Debug.Log($"Saving inventory {data.Count}, {jsonData}...");

        //cloudData.inventory = data;
        CloudServices.SetString(cloudInventoryKey, jsonData);
        //CloudServices.Synchronize();
    }

    public void PushUser(UserData data)
    {
        if (data == null || data.Name == null) return;

        string jsonData = JsonConvert.SerializeObject(data);
        //cloudData.user = data;
        CloudServices.SetString(cloudUserKey, jsonData);
        //CloudServices.Synchronize();
    }

    public void PushHeroes(List<MyHeroes> data)
    {
        if (data == null || data.Count == 0) return;

        string jsonData = JsonConvert.SerializeObject(data);
        //Debug.Log($"Saving heroes {data.Count}, {jsonData}");

        //cloudData.heroes = data;
        CloudServices.SetString(cloudHeroesKey, jsonData);
        //CloudServices.Synchronize();
    }

    private void OnDestroy()
    {
        CloudServices.Synchronize();
    }
}

public class CloudData
{
    public List<ItemInventory> inventory;
    public UserData user;
    public List<MyHeroes> heroes;
}
