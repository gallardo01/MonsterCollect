using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using MarchingBytes;

public class SumaryController : MonoBehaviour
{
    public GameObject[] stars;
    public Image bossIcon;
    public TextMeshProUGUI textProgress;
    public GameObject[] item;
    public Button closePanel;
    public GameObject hide;
    private Sprite[] heroesSprite;

    // Start is called before the first frame update
    void Start()
    {
        closePanel.onClick.AddListener(() => closeGame());
    }

    private void closeGame()
    {
        SceneManager.LoadScene(0);
    }

    public void initEndingData(int progress, int stage, int gold, List<ItemInventory> rewards)
    {
        heroesSprite = Resources.LoadAll<Sprite>("Contents/Icon/Island");
        hide.SetActive(false);
        //Time.timeScale = 0;
        stars[0].SetActive(true);
        if (progress >= 50)
        {
            stars[1].SetActive(true);
        }
        if (progress >= 90)
        {
            stars[2].SetActive(true);
        }
        bossIcon.sprite = heroesSprite[stage - 1];
        bossIcon.GetComponent<Image>().SetNativeSize();
        textProgress.text = "You reach <size=70><color=yellow>" + progress.ToString() + "%<size=50><color=white>Stage " + stage.ToString();

        int diamond = 0;
        if (progress >= 70)
        {
            diamond = Random.Range(1, 10);
            if (progress >= 100)
            {
                diamond = diamond + 5 + stage * 2;
            }
        }
        UserDatabase.Instance.gainMoneyInGame(gold, diamond);
        StartCoroutine(animationItem(gold, rewards, diamond));
    }

    IEnumerator animationItem(int gold, List<ItemInventory> rewards, int diamond)
    {
        int num = -1;
        if (gold > 0)
        {
            num++;
            item[num].SetActive(true);
            item[num].GetComponent<InflateItemRewards>().inflateGoldItem(gold);
        }
        if (diamond > 0)
        {
            num++;
            item[num].SetActive(true);
            item[num].GetComponent<InflateItemRewards>().inflateDiamondItem(diamond);
        }
        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < rewards.Count; i++)
        {
            item[i + num + 1].SetActive(true);
            item[i + num + 1].gameObject.GetComponent<InflateItemRewards>().inflateItem(rewards[i]);
            ItemDatabase.Instance.addNewItem(rewards[i].Id, rewards[i].Slot, rewards[i].Rarity);
            yield return new WaitForSeconds(0.5f);
        }
    }
}
