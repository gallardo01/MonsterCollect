using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MarchingBytes;
using DragonBones;

public class InflateItemRewards : MonoBehaviour
{
    public Image backerItem;
    public Image iconItem;
    public TextMeshProUGUI slotItem;
    private Sprite[] itemSprite;

    // Start is called before the first frame update

    private void Awake()
    {
    }
    void Start()
    {
        
    }

    public void inflateItem(ItemInventory item)
    {
        itemSprite = Resources.LoadAll<Sprite>("Contents/Item/Item");
        backerItem.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + item.Rarity);
        int index;
        if (item.Id < 100)
        {
            index = item.Id - 1;
        }
        else
        {
            index = item.Id - 68;
        }
        iconItem.sprite = itemSprite[index];
        slotItem.text = item.Slot.ToString();
    }

    public void inflateGoldItem(int slot)
    {
        iconItem.sprite = Resources.Load<Sprite>("UI/Sprites/Gold");
        slotItem.text = slot.ToString();
    }

    public void inflateDiamondItem(int slot)
    {
        GameObject particle = EasyObjectPool.instance.GetObjectFromPool("Diamond", transform.position, transform.rotation);
        particle.transform.SetParent(gameObject.transform);

        iconItem.sprite = Resources.Load<Sprite>("UI/Sprites/Diamond");
        StartCoroutine(animationDiamond(slot));
    }

    IEnumerator animationDiamond(int slot)
    {
        for (int i = 0; i < 6; i++)
        {
            slotItem.text = Random.Range(1, 30).ToString();
            yield return new WaitForSeconds(0.2f);
        }
        slotItem.text = slot.ToString();
    }
}
