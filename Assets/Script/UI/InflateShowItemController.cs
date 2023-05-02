using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InflateShowItemController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI nameItem;
    [SerializeField] Image typeItem;
    [SerializeField] Image iconItem;
    [SerializeField] TextMeshProUGUI levelItem;
    [SerializeField] Image iconFirstStat;
    [SerializeField] TextMeshProUGUI textFirstStat;
    [SerializeField] TextMeshProUGUI contentText;

    [SerializeField] TextMeshProUGUI allStatsText;
    [SerializeField] TextMeshProUGUI goldText;
    [SerializeField] Image iconShard;
    [SerializeField] TextMeshProUGUI shardText;

    [SerializeField] TextMeshProUGUI equipText;
    [SerializeField] Button equipButton;
    [SerializeField] Button upgradeButton;
    [SerializeField] Button closePanel;
    private ItemInventory itemData;
    private string[] textStats = { "", "Attack", "Hp", "Armour", "Movement Speed", "Crit", "Attack Speed", "Extra Gold", "Extra Exp" };
    private string[] textTypes = {"", "Fire", "Thunder", "Water", "Grass" };
    private bool canClose = true;

    [SerializeField] Slider upgradeSlider;
    [SerializeField] Slider upgradeSlider2;
    [SerializeField] GameObject particle;
    // Start is called before the first frame update
    void Start()
    {
        equipButton.onClick.AddListener(() => equipItem());
        upgradeButton.onClick.AddListener(() => upgradeItem());
        closePanel.onClick.AddListener(() => closePanelButton());
    }

    private void equipItem()
    {
        if(itemData.IsUse < 0)
        {
            ItemDatabase.Instance.unequipItem(itemData.ShopId);
        } else
        {
            ItemDatabase.Instance.equipItemPosition(itemData);
        }
        closePanelButton();
    }
    private void closePanelButton()
    {
        if (canClose)
        {
            InventoryController.Instance.enableHeroes();
            gameObject.SetActive(false);
        }
    }
    private void upgradeItem()
    {
        if (canClose)
        {
            canClose = false;
            upgradeButton.interactable = false;
            StartCoroutine(upgradeItemAnimation());
        }
    }

    IEnumerator upgradeItemAnimation()
    {
        particle.SetActive(false);
        upgradeSlider.DOValue(1f, 1f, true);
        upgradeSlider2.DOValue(1f, 1f, true);
        ItemDatabase.Instance.upgradeItem(itemData.ShopId);
        yield return new WaitForSeconds(1f);
        particle.SetActive(true);
        upgradeSlider.GetComponent<Slider>().value = 0f;
        upgradeSlider2.GetComponent<Slider>().value = 0f;
        itemData = ItemDatabase.Instance.fetchInventoryByShopId(itemData.ShopId);
        initItem(itemData);
        particle.SetActive(true);
        yield return new WaitForSeconds(2f);
        particle.SetActive(false);
        canClose = true;
    }
    public void initItem(ItemInventory item)
    {
        itemData = item;
        nameItem.text = item.Name;
        if((item.Id-10)%4 == 0)
        {
            nameItem.color = new Color(255f, 136f, 0f, 255f);
        } else if((item.Id - 10) % 4 == 1) { nameItem.color = Color.yellow; }
        else if ((item.Id - 10) % 4 == 2) { nameItem.color = new Color(0f, 201f, 255f, 255f); }
        else if ((item.Id - 10) % 4 == 3) { nameItem.color = new Color(15f, 255f, 0f, 255f); }

        typeItem.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + item.Rarity.ToString());
        iconItem.sprite = Resources.Load<Sprite>("Contents/Item/" + item.Id.ToString());
        levelItem.text = $"Level: {item.Level}/20";
        iconFirstStat.sprite = Resources.Load<Sprite>("Contents/Icon/DameType/" + (item.Stats_1/100).ToString());
        textFirstStat.text = "+" + (item.Stats_1 % 100).ToString();
        contentText.text = item.Contents;

        string textAllStats = "";
        if (item.Stats_2 > 0) textAllStats += $"<sprite={item.Stats_2 / 1000}> {textStats[item.Stats_2 / 1000]} +{item.Stats_2 % 1000} \n";
        if (item.Stats_3 > 0) textAllStats += $"<sprite={item.Stats_3 / 1000}> {textStats[item.Stats_3 / 1000]} +{item.Stats_3 % 1000} \n";
        if (item.Stats_4 > 0) textAllStats += $"<sprite={item.Stats_4 / 1000}> {textStats[item.Stats_4 / 1000]} +{item.Stats_4 % 1000} \n";
        if (item.Stats_5 > 0) textAllStats += $"<sprite={item.Stats_5 / 1000}> {textStats[item.Stats_5 / 1000]} +{item.Stats_5 % 1000} \n";

        if(ItemDatabase.Instance.getBonusItemSameType(item.Id) >= 3)
        {
            textAllStats += $"[Actived with 3 {textTypes[(item.Id-10)%4 + 1]}Type Items]\n      <sprite={10+ (item.Id - 10) % 4}>All {textTypes[(item.Id - 10) % 4 + 1]} Monster +5% Stats\n";
        } else
        {
            textAllStats += $"<color=#8A8A8A>[Active with 3 {textTypes[(item.Id - 10) % 4 + 1]}Type Items]\n      <sprite=11>All {textTypes[(item.Id - 10) % 4 + 1]} Monster +5% Stats </color>\n";
        }
        if (ItemDatabase.Instance.getBonusItemSameType(item.Id) >= 6)
        {
            textAllStats += $"[Actived with 6 {textTypes[(item.Id - 10) % 4 + 1]}Type Items]\n<sprite=10>All Monster +5% Stats";
        }
        else
        {
            textAllStats += $"<color=#8A8A8A>[Actived with 6 {textTypes[(item.Id - 10) % 4 + 1]}Type Items]\n<sprite=10> All Monster +5% Stats </color>";
        }
        allStatsText.text = textAllStats;
        upgradeButton.interactable = true;
        iconShard.sprite = Resources.Load<Sprite>("Contents/Item/" + (5 + (item.Id - 10) % 4).ToString());
        int goldRequire = (item.Level) * 500 + 500 * (item.Rarity - 1);
        goldText.text = goldRequire.ToString();
        Debug.Log(UserDatabase.Instance.getUserData().Gold);
        if(goldRequire <= UserDatabase.Instance.getUserData().Gold) { goldText.color = Color.white; } else { goldText.color = Color.red; upgradeButton.interactable = false;}
        int shardRequire = (item.Level) * 4/8 + (item.Rarity);
        string colorText = "white";
        if (shardRequire <= ItemDatabase.Instance.fetchInventoryById(5+ (item.Id - 10) % 4).Slot) { colorText = "white"; } else { goldText.color = Color.red; upgradeButton.interactable = false;}
        shardText.text = $"<color={colorText}> {ItemDatabase.Instance.fetchInventoryById(5 + (item.Id - 10) % 4).Slot} </color>/{shardRequire}";

        if (item.IsUse < 0)
        {
            equipText.text = "Unequip";
        } else
        {
            equipText.text = "Equip";
        }
    }
}
