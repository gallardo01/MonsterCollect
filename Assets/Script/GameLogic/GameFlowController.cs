using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFlowController : Singleton<GameFlowController>
{
    [SerializeField] GameObject dead;
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] Button watchAds;
    [SerializeField] Button closeBtn;
    [SerializeField] GameObject sumaryObj;
    [SerializeField] Button acceptRevive;
    [SerializeField] Button cancelRevive;
    private bool isAction = true;

    int variable_progress = 0;
    int variable_stage = 0;
    int variable_gold = 0;
    List<ItemInventory> rewards = new List<ItemInventory>();

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() => closeButton());
        acceptRevive.onClick.AddListener(() => reviveButton());
        cancelRevive.onClick.AddListener(() => cancelReviveAction());
    }

    public void userDeath()
    {
        StartCoroutine(revive());
    }

    public void initData(int progress, int stage, int gold, List<ItemInventory> listRewards)
    {
        variable_progress = progress;
        variable_stage = stage;
        variable_gold = gold;
        rewards = listRewards;
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
        StopAllCoroutines();
        dead.SetActive(false);
        isAction = false;
        PlayerController.Instance.revivePlayer();
    }
    private void cancelReviveAction()
    {
        closeButton();
    }
    private void closeButton()
    {
        StopAllCoroutines();
        isAction = false;
        endGame(variable_progress, variable_stage, variable_gold, rewards);
        dead.SetActive(false);
    }
    public void gameOver()
    {
        endGame(variable_progress, variable_stage, variable_gold, rewards);
    }
    public void endGame(int progress, int stage, int gold, List<ItemInventory> listItem)
    {
        sumaryObj.SetActive(true);
        sumaryObj.GetComponent<SumaryController>().initEndingData(progress, stage, gold, listItem);
    }
}
