using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFlowController : Singleton<GameFlowController>
{
    public GameObject dead;
    public TextMeshProUGUI countdown;
    public Button watchAds;
    public Button closeBtn;
    public GameObject sumaryObj;
    private bool isAction = true;

    int variable_progress = 0;
    int variable_stage = 0;
    int variable_gold = 0;
    ItemInventory[] variable_rewards;

    // Start is called before the first frame update
    void Start()
    {
        List<ItemInventory> rewards = new List<ItemInventory>();
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));
        rewards.Add(ItemDatabase.Instance.getItemObject(30, 1, 2));
        initData(60, 4, 3000, rewards.ToArray());

        closeBtn.onClick.AddListener(() => closeButton());
    }

    public void userDeath()
    {
        StartCoroutine(revive());
    }

    public void initData(int progress, int stage, int gold, ItemInventory[] rewards)
    {
        variable_progress = progress;
        variable_stage = stage;
        variable_gold = gold;
        variable_rewards = rewards;
    }

    IEnumerator revive()
    {
        dead.SetActive(true);
        for(int i = 5; i >= 0; i--)
        {
            countdown.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        if (isAction)
        {
            closeButton();
        }
    }
    private void reviveButton()
    {

    }
    private void closeButton()
    {
        isAction = false;
        endGame(variable_progress, variable_stage, variable_gold, variable_rewards);
        dead.SetActive(false);
    }

    public void endGame(int progress, int stage, int gold, ItemInventory[] rewards)
    {
        sumaryObj.SetActive(true);
        sumaryObj.GetComponent<SumaryController>().initEndingData(progress, stage, gold, rewards);
    }
}
