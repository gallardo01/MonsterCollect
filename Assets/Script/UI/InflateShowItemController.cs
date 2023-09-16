using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using DigitalRuby.SoundManagerNamespace;

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
        closePanel.onClick.AddListener(() => closePanelButton());
        equipButton.onClick.AddListener(() => equipItem());
        upgradeButton.onClick.AddListener(() => upgradeItem());
    }

    private void OnEnable()
    {
        canClose = true;
        particle.SetActive(false);
    }
    private void equipItem()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        if (itemData.IsUse < 0)
        {
            ItemDatabase.Instance.unequipItem(itemData.ShopId);
        } else
        {
            ItemDatabase.Instance.equipItemPosition(itemData);
        }
        InventoryController.Instance.playEquipAnimation(itemData.Type - 1);
        closePanelButton();
    }
    private void closePanelButton()
    {
        if (canClose)
        {
            SoundManagerDemo.Instance.playOneShot(9);
            if (itemData.Type % 10 > 0)
            {
                particle.SetActive(false);
            }
            InventoryController.Instance.enableHeroes();
            gameObject.SetActive(false);
        }
    }
    private void upgradeItem()
    {
        if (canClose)
        {
            SoundManagerDemo.Instance.playOneShot(9);
            canClose = false;
            upgradeButton.interactable = false;
            StartCoroutine(upgradeItemAnimation());
        }
    }

    IEnumerator upgradeItemAnimation()
    {
        particle.SetActive(false);
        //upgradeSlider.GetComponent<Slider>().DO
        upgradeSlider.DOValue(1f, 1f, false);
        upgradeSlider2.DOValue(1f, 1f, false);
        ItemDatabase.Instance.upgradeItem(itemData.ShopId);
        yield return new WaitForSeconds(1f);
        SoundManagerDemo.Instance.playOneShot(10);
        particle.SetActive(true);
        upgradeSlider.GetComponent<Slider>().value = 0f;
        upgradeSlider2.GetComponent<Slider>().value = 0f;
        itemData = ItemDatabase.Instance.fetchInventoryByShopId(itemData.ShopId);
        initItem(itemData);
        InventoryController.Instance.initItemData();
        yield return new WaitForSeconds(2f);
        particle.SetActive(false);
        canClose = true;
    }
    public void initItem(ItemInventory item)
    {
        //item = ItemDatabase.Instance.fetchInventoryByShopId(item.ShopId);
        itemData = item;
        nameItem.text = item.Name;
        if((item.Id-10)%4 == 0)
        {
            nameItem.color = Color.red;
        } else if((item.Id - 10) % 4 == 1) { nameItem.color = Color.yellow; }
        else if ((item.Id - 10) % 4 == 2) { nameItem.color = Color.blue; }
        else if ((item.Id - 10) % 4 == 3) { nameItem.color = Color.green; }

        typeItem.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + item.Rarity.ToString());
        iconItem.sprite = InventoryController.Instance.getSpriteIndex(item.Id);
        levelItem.text = $"Level: {item.Level}/20";
        iconFirstStat.sprite = Resources.Load<Sprite>("Contents/Icon/DameType/" + (item.Stats_1/1000).ToString());
        textFirstStat.text = "+" + (item.Stats_1 % 100).ToString();
        contentText.text = item.Contents;

        string textAllStats = "";
        if (item.Stats_2 > 1000) textAllStats += $"<sprite={item.Stats_2 / 1000}> {textStats[item.Stats_2 / 1000]} +{item.Stats_2 % 1000} \n";
        if (item.Stats_3 > 1000) textAllStats += $"<sprite={item.Stats_3 / 1000}> {textStats[item.Stats_3 / 1000]} +{item.Stats_3 % 1000} \n";
        if (item.Stats_4 > 1000) textAllStats += $"<sprite={item.Stats_4 / 1000}> {textStats[item.Stats_4 / 1000]} +{item.Stats_4 % 1000} \n";
        if (item.Stats_5 > 1000) textAllStats += $"<sprite={item.Stats_5 / 1000}> {textStats[item.Stats_5 / 1000]} +{item.Stats_5 % 1000} \n";

        if(ItemDatabase.Instance.getBonusItemSameType(item.Id) >= 3)
        {
            textAllStats += $"[Actived with 3 {textTypes[(item.Id-10)%4 + 1]}Type Items]\n<sprite={11 + (item.Id - 10) % 4}>All {textTypes[(item.Id - 10) % 4 + 1]} Monster +5% Stats\n";
        } else
        {
            textAllStats += $"<color=#8A8A8A>[Active with 3 {textTypes[(item.Id - 10) % 4 + 1]}Type Items]\n<sprite={11 + (item.Id - 10) % 4}>All {textTypes[(item.Id - 10) % 4 + 1]} Monster +5% Stats </color>\n";
        }
        if (ItemDatabase.Instance.getBonusItemSameType(item.Id) >= 6)
        {
            textAllStats += $"[Actived with 6 {textTypes[(item.Id - 10) % 4 + 1]}Type Items]\n<sprite=10> All Monster +5% Stats";
        }
        else
        {
            textAllStats += $"<color=#8A8A8A>[Actived with 6 {textTypes[(item.Id - 10) % 4 + 1]}Type Items]\n<sprite=10> All Monster +5% Stats </color>";
        }
        allStatsText.text = textAllStats;
        upgradeButton.interactable = true;
        iconShard.sprite = InventoryController.Instance.getSpriteIndex(5 + (item.Id - 10) % 4);
        int goldRequire = (item.Level) * 500 + 500 * (item.Rarity - 1);
        goldText.text = goldRequire.ToString();
        if(goldRequire <= UserDatabase.Instance.getUserData().Gold) { goldText.color = Color.white; } else { goldText.color = Color.red; upgradeButton.interactable = false;}
        int shardRequire = (item.Level) * 4/8 + (item.Rarity);
        string colorText;
        if (shardRequire <= ItemDatabase.Instance.fetchInventoryById(5+ (item.Id - 10) % 4).Slot) { colorText = "white"; } else { colorText = "red"; upgradeButton.interactable = false;}
        shardText.text = $"<color={colorText}> {ItemDatabase.Instance.fetchInventoryById(5 + (item.Id - 10) % 4).Slot} </color>/{shardRequire}";

        if (item.IsUse < 0)
        {
            equipText.text = "Unequip";
        } else
        {
            equipText.text = "Equip";
        }
    }

    public void consumeItem(ItemInventory item)
    {
        itemData = item;
        nameItem.text = item.Name;
        nameItem.color = Color.white;

        typeItem.sprite = Resources.Load<Sprite>("UI/Inventory/SlotItem/" + item.Rarity.ToString());
        iconItem.sprite = InventoryController.Instance.getSpriteIndex(item.Id);
        levelItem.text = $"Quantity: {item.Slot}";
        contentText.text = item.Contents;
    }
}
