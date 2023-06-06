using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CelebrationShopController : MonoBehaviour
{
    [SerializeField] GameObject[] itemsCelebration;
    [SerializeField] Button closePanel;

    // Start is called before the first frame update
    void Start()
    {
        closePanel.onClick.AddListener(() => closeCurrent());
    }
    private void closeCurrent()
    {
        gameObject.SetActive(false);
    }

    public void initCelebration(List<ItemInventory> items, int gold, int diamond)
    {
        StartCoroutine(showAnimation(items, gold, diamond));
    }

    IEnumerator showAnimation(List<ItemInventory> items, int gold, int diamond)
    {
        closePanel.interactable = false;
        yield return new WaitForSeconds(0.5f);
        int index = 0;
        if (gold > 0)
        {
            index++;
        }
        if(diamond > 0)
        {
            index++;
        }
        index += items.Count;
        for(int i = 0; i < 10; i++)
        {
            itemsCelebration[i].SetActive(false);
        }
        int start = 0;
        if (gold > 0)
        {
            itemsCelebration[start].SetActive(true);
            itemsCelebration[start].GetComponent<Animator>().Play("Shake");
            itemsCelebration[start].GetComponent<ItemInflate>().setupRarityObj(1);
            itemsCelebration[start].GetComponent<ItemInflate>().setupCurrencyItem(gold, 0);
            yield return new WaitForSeconds(0.5f);
            start++;
        }
        if (diamond > 0)
        {
            itemsCelebration[start].SetActive(true);
            itemsCelebration[start].GetComponent<Animator>().Play("Shake");
            itemsCelebration[start].GetComponent<ItemInflate>().setupRarityObj(1);
            itemsCelebration[start].GetComponent<ItemInflate>().setupCurrencyItem(0, diamond);
            yield return new WaitForSeconds(0.5f);
            start++;
        }

        for(int i = 0; i < items.Count; i++)
        {
            itemsCelebration[start + i].SetActive(true);
            itemsCelebration[start + i].GetComponent<Animator>().Play("Shake");
            itemsCelebration[start + i].GetComponent<ItemInflate>().setupRarityObj(items[i].Rarity);
            itemsCelebration[start + i].GetComponent<ItemInflate>().InitData(items[i]);
            yield return new WaitForSeconds(0.5f);
        }
        closePanel.interactable = true;
    }
}
