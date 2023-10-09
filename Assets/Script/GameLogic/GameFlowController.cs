﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DigitalRuby.SoundManagerNamespace;

public class GameFlowController : Singleton<GameFlowController>
{
    [SerializeField] GameObject dead;
    [SerializeField] TextMeshProUGUI countdown;
    [SerializeField] Button watchAds;
    [SerializeField] Button reviveDiamond;

    [SerializeField] Button closeBtn;
    [SerializeField] GameObject sumaryObj;
    [SerializeField] Button acceptRevive;
    [SerializeField] Button cancelRevive;
    private bool isAction = true;

    int variable_progress = 0;
    int variable_stage = 0;
    int variable_gold = 0;
    List<ItemInventory> rewards = new List<ItemInventory>();
    private bool isRevive = true;

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(() => closeButton());
        acceptRevive.onClick.AddListener(() => reviveButton());
        cancelRevive.onClick.AddListener(() => cancelReviveAction());
        reviveDiamond.onClick.AddListener(() => reviveDiamondAction());
    }

    public void userDeath()
    {
        if (isRevive)
        {
            isRevive = false;
            StartCoroutine(revive());
        } else
        {
            closeButton();
        }
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
        if (AdsController.Instance.canShowAds())
        {
            watchAds.gameObject.SetActive(true);
            reviveDiamond.gameObject.SetActive(false);
        } else
        {
            watchAds.gameObject.SetActive(false);
            reviveDiamond.gameObject.SetActive(true);
            if (UserDatabase.Instance.getUserData().Diamond >= 30)
            {
                reviveDiamond.interactable = true;
            } else
            {
                reviveDiamond.interactable = false;
            }
        }
        for (int i = 10; i >= 0; i--)
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
        // watch ads
        AdsController.Instance.ShowAd(1);
    }

    public void pauseWhenWatchAds()
    {
        StopAllCoroutines();
    }

    public void reviveSuccess()
    {
        StartCoroutine(delayAds());
    }

    public void reviveDiamondAction()
    {
        UserDatabase.Instance.reduceMoney(0, 30);
        dead.SetActive(false);
        isAction = false;
        PlayerController.Instance.revivePlayer();
        SoundManagerDemo.Instance.playOneShot(10);
    }

    IEnumerator delayAds()
    {
        yield return new WaitForSeconds(0.3f);
        dead.SetActive(false);
        isAction = false;
        PlayerController.Instance.revivePlayer();
        SoundManagerDemo.Instance.playOneShot(10);
    }
    public void reviveFailed()
    {
        cancelReviveAction();
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
        if (progress < 0)
        {
            progress = 0;
        }
        sumaryObj.SetActive(true);
        sumaryObj.GetComponent<SumaryController>().initEndingData(progress, stage, gold, listItem);
    }
}
