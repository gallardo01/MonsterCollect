using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SumaryController : MonoBehaviour
{
    public GameObject[] stars;
    public Image bossIcon;
    public TextMeshProUGUI textProgress;
    public GameObject[] item;
    public Button closePanel;
    public GameObject hide;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void closeGame()
    {

    }

    public void initEndingData(int progress, int stage, int gold, ItemInventory[] rewards)
    {
        //hide.SetActive(false);
        //Time.timeScale = 0;
        stars[0].SetActive(true);
        if(progress >= 50)
        {
            stars[1].SetActive(true);
        }
        if (progress >= 90)
        {
            stars[2].SetActive(true);
        }
        bossIcon.sprite = Resources.Load<Sprite>("UI/Icons/Creep/" + (stage*10).ToString());
        textProgress.text = "You reach <size=70><color=yellow>" + progress.ToString() + "%<size=50><color=white>Stage " + stage.ToString();

        item[0].SetActive(true);
        item[0].GetComponent<InflateItemRewards>().inflateGoldItem(gold);
        UserDatabase.Instance.gainMoney(gold, 0);
        for (int i = 0; i < rewards.Length; i++)
        {
            item[i + 1].SetActive(true);
            item[i + 1].gameObject.GetComponent<InflateItemRewards>().inflateItem(rewards[i].Id, rewards[i].Slot);
            ItemDatabase.Instance.addNewItem(rewards[i].Id, rewards[i].Slot, rewards[i].Rarity);
        }

    }
}
