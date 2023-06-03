using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;
using Newtonsoft.Json;

public class SyncService : Singleton<SyncService>
{
    private const string cloudInventoryKey = "inventory";
    private const string cloudUserKey = "user";
    private const string cloudHeroesKey = "heroes";

    private CloudData cloudData = null;

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

    public bool HasRetrievedCloudData() => cloudData != null;

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

    private void RetrieveCloudData()
    {
        string inventoryJson = CloudServices.GetString(cloudInventoryKey);
        string userJson = CloudServices.GetString(cloudUserKey);
        string heroesJson = CloudServices.GetString(cloudHeroesKey);

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
        Debug.Log("Cloud synchronization completed:");
        Debug.Log($" - inventory: {inventoryJson}");
        Debug.Log($" - user: {userJson}");
        Debug.Log($" - heroes: {heroesJson}");
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
        string jsonData = JsonConvert.SerializeObject(data);
        Debug.Log($"Saving inventory {jsonData}...");

        cloudData.inventory = data;
        CloudServices.SetString(cloudInventoryKey, jsonData);
    }

    public void PushUser(UserData data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        cloudData.user = data;
        CloudServices.SetString(cloudUserKey, jsonData);
    }

    public void PushHeroes(List<MyHeroes> data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        cloudData.heroes = data;
        CloudServices.SetString(cloudHeroesKey, jsonData);
    }

}

public class CloudData
{
    public List<ItemInventory> inventory;
    public UserData user;
    public List<MyHeroes> heroes;
}
