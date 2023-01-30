using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlowController : Singleton<GameFlowController>
{
    public GameObject sumaryObj;
    // Start is called before the first frame update
    void Start()
    {
        List<ItemInventory> rewards = new List<ItemInventory>();
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));

        endGame(20, 1, 2000, rewards.ToArray());
    }

    public void endGame(int progress, int stage, int gold, ItemInventory[] rewards)
    {
        Instantiate(sumaryObj, gameObject.transform);
        sumaryObj.GetComponent<SumaryController>().initEndingData(progress, stage, gold, rewards);
    }
}
