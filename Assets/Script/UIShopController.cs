using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class UIShopController : MonoBehaviour
{
    public GameObject ParentTransform;
    public GameObject TargetedOfferPanel;
    public GameObject ChestsPanel;
    public GameObject HeroesPanel;
    public GameObject GemsPanel;
    public GameObject CoinsPanel;

    public GameObject _go_claim_popup;

    public GameObject Chests_Golden_Label;
    public GameObject Chests_Golden_Description;
    public GameObject Chests_Golden_OpenKeyButton_Text;
    public GameObject Chests_Golden_OpenKeyButton;

    public GameObject Chests_Diamon_Label;
    public GameObject Chests_Diamon_Description;
    public GameObject Chests_Diamon_Draw_Text;
    public GameObject Chests_Diamon_Draw;

    public GameObject Chests_Diamonx10_Label;
    public GameObject Chests_Diamonx10_Description_1;
    public GameObject Chests_Diamonx10_Description_2;
    public GameObject Chests_Diamonx10_BaseValue;
    public GameObject Chests_Diamonx10_Value;
    public GameObject Chests_Diamonx10_Draw;

    public GameObject Heroes_Item1_Label;
    public GameObject Heroes_Item1_SubLabel;
    public GameObject Heroes_Item1_Price;

    public GameObject Heroes_Item2_Label;
    public GameObject Heroes_Item2_SubLabel;
    public GameObject Heroes_Item2_Price;

    public GameObject Heroes_Item3_Label;
    public GameObject Heroes_Item3_SubLabel;
    public GameObject Heroes_Item3_Price;


    void Start()
    {
        InitTargetedOffer();
        InitChests();
        InitHeroes();
        InitGems();
        InitCoins();
    }
    void InitTargetedOffer()
    {

        //init data

        GameObject TO = new GameObject();
        int to = UnityEngine.Random.Range(1, 5);

        SetUpTO(TO, to);

        //setup data
        TargetedOfferPanel.transform.GetChild(0).GetComponent<Button>().onClick.AddListener(() => { SetupCelebration(); });
    }
    void SetupCelebration()
    {
        var _claim_popup = Instantiate(_go_claim_popup, ParentTransform.transform);
        StartCoroutine(FadeCelebration(_claim_popup));
    }
    IEnumerator FadeCelebration(GameObject _claim_popup)
    {
        yield return new WaitForSeconds(0.5f);
        _claim_popup.GetComponent<Animator>().SetTrigger("Fade");
    }


    void SetUpTO(GameObject TO, int index)
    {
        //TargetedOfferPanel.transform.Find("Top/Badge/Text").GetComponent<TextMeshProUGUI>().text = "-" + (100 - (Mathf.Round((float)((StaticInfo.TOValue[index] / StaticInfo.TOBaseValue[index]))) * 100)) + "%";
        TO = Instantiate(Resources.Load($"Prefabs/UI/TargetOffer/TO_{index}") as GameObject, TargetedOfferPanel.transform.GetChild(0).transform);
        TO.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = StaticInfo.TODescription[index];
        TO.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = StaticInfo.TOBaseValue[index] + "$";
        TO.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = StaticInfo.TOValue[index] + "$";

        //var item1_image = TO.transform.GetChild(0).GetComponent<Image>().sprite;
        //var item2_image = TO.transform.GetChild(1).GetChild(1).GetComponent<Image>().sprite;
        //var item3_image = TO.transform.Find("Content/Item1 (2)/Image").GetComponent<Image>().sprite;
        //var item4_image = TO.transform.Find("Content/Item1 (3)/Image").GetComponent<Image>().sprite;
    }

    void InitChests()
    {
        //Golden Chest
        Chests_Golden_Label.GetComponent<TextMeshProUGUI>().text = "Golden Chest";
        Chests_Golden_Description.GetComponent<TextMeshProUGUI>().text = "Contains One <color=#7e7e7e> Common </color> or <color=#00ff00>Great</color> equipmet";
        Chests_Golden_OpenKeyButton_Text.GetComponent<TextMeshProUGUI>().text = "<size=120%><sprite=5></size> 1000";
        Chests_Golden_OpenKeyButton.GetComponent<Button>().onClick.AddListener(() => OnChestPurchased("coin", 1, 1000));

        //Diamon Chest
        Chests_Diamon_Label.GetComponent<TextMeshProUGUI>().text = "Diamon Chest";
        Chests_Diamon_Description.GetComponent<TextMeshProUGUI>().text = "Contains One <color=#00ff00> Great </color>,<color=#00aaff>Rare</color> or <color=#ff00ff>Epic</color> equipmet";
        Chests_Diamon_Draw_Text.GetComponent<TextMeshProUGUI>().text = "<size=120%><sprite=0></size> 200";
        Chests_Diamon_Draw.GetComponent<Button>().onClick.AddListener(() => OnChestPurchased("diamond", 1, 200));

        //X10 Diamon Chest
        Chests_Diamonx10_Label.GetComponent<TextMeshProUGUI>().text = "Diamon Chest X10";
        Chests_Diamonx10_Description_1.GetComponent<TextMeshProUGUI>().text = "Contains 10 <color=#00ff00> Great </color>,<color=#00aaff>Rare</color> or <color=#ff00ff>Epic</color> equipmet";
        Chests_Diamonx10_Description_2.GetComponent<TextMeshProUGUI>().text = "Get an <color=#ff00ff>Epic</color> equipmet";
        Chests_Diamonx10_Value.GetComponent<TextMeshProUGUI>().text = "<size=120%><sprite=0></size> 1800";
        Chests_Diamonx10_BaseValue.GetComponent<TextMeshProUGUI>().text = "<size=120%><sprite=0></size> 2000";
        Chests_Diamonx10_Draw.GetComponent<Button>().onClick.AddListener(() => OnChestPurchased("diamond", 10, 1800));
    }


    void InitHeroes()
    {
        //Item 1
        Heroes_Item1_Label.GetComponent<TextMeshProUGUI>().text = "<color=#FFFFFF>Ice Dragon";
        Heroes_Item1_SubLabel.GetComponent<TextMeshProUGUI>().text = "<color=#FFFFFF>Discount 30%";
        Heroes_Item1_Price.GetComponent<TextMeshProUGUI>().text = "<sprite=0>345";
        //Item 2
        Heroes_Item2_Label.GetComponent<TextMeshProUGUI>().text = "<color=#00FFFF>Earth Mamoth";
        Heroes_Item2_SubLabel.GetComponent<TextMeshProUGUI>().text = "<color=#FFFFFF>Discount 30%";
        Heroes_Item2_Price.GetComponent<TextMeshProUGUI>().text = "<sprite=0>345";
        //Item 3
        Heroes_Item3_Label.GetComponent<TextMeshProUGUI>().text = "<color=#FF0000>Fire Bird";
        Heroes_Item3_SubLabel.GetComponent<TextMeshProUGUI>().text = "<color=#FFFFFF>Discount 30%";
        Heroes_Item3_Price.GetComponent<TextMeshProUGUI>().text = "<sprite=0>345";
    }

    void InitGems()
    {
        //Item 1
        GemsPanel.transform.Find("Content/Item1/Label").GetComponent<TextMeshProUGUI>().text = "100";
        GemsPanel.transform.Find("Content/Item1/Description/Text").GetComponent<TextMeshProUGUI>().text = "";
        GemsPanel.transform.Find("Content/Item1/SubLabel").GetComponent<TextMeshProUGUI>().text = "Pile of Gems";
        GemsPanel.transform.Find("Content/Item1/Price").GetComponent<TextMeshProUGUI>().text = "0.99$";
        GemsPanel.transform.Find("Content/Item1").GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(100, 0.99));

        //Item 2
        GemsPanel.transform.Find("Content/Item2/Label").GetComponent<TextMeshProUGUI>().text = "200";
        GemsPanel.transform.Find("Content/Item2/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 10%";
        GemsPanel.transform.Find("Content/Item2/SubLabel").GetComponent<TextMeshProUGUI>().text = "Heap of Gems";
        GemsPanel.transform.Find("Content/Item2/Price").GetComponent<TextMeshProUGUI>().text = "1.79$";
        GemsPanel.transform.Find("Content/Item2").GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(200, 1.79));

        //Item 3
        GemsPanel.transform.Find("Content/Item3/Label").GetComponent<TextMeshProUGUI>().text = "500";
        GemsPanel.transform.Find("Content/Item3/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 20%";
        GemsPanel.transform.Find("Content/Item3/SubLabel").GetComponent<TextMeshProUGUI>().text = "Bucket of Gems";
        GemsPanel.transform.Find("Content/Item3/Price").GetComponent<TextMeshProUGUI>().text = "3.99$";
        GemsPanel.transform.Find("Content/Item3").GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(500, 3.99));

        //Item 4
        GemsPanel.transform.Find("Content/Item4/Label").GetComponent<TextMeshProUGUI>().text = "1000";
        GemsPanel.transform.Find("Content/Item4/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 30%";
        GemsPanel.transform.Find("Content/Item4/SubLabel").GetComponent<TextMeshProUGUI>().text = "Barrel of Gems";
        GemsPanel.transform.Find("Content/Item4/Price").GetComponent<TextMeshProUGUI>().text = "6.99$";
        GemsPanel.transform.Find("Content/Item4").GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(1000, 6.99));

        //Item 5
        GemsPanel.transform.Find("Content/Item5/Label").GetComponent<TextMeshProUGUI>().text = "2000";
        GemsPanel.transform.Find("Content/Item5/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 40%";
        GemsPanel.transform.Find("Content/Item5/SubLabel").GetComponent<TextMeshProUGUI>().text = "Chest of Gems";
        GemsPanel.transform.Find("Content/Item5/Price").GetComponent<TextMeshProUGUI>().text = "11.99$";
        GemsPanel.transform.Find("Content/Item5").GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(2000, 11.99));

        //Item 6
        GemsPanel.transform.Find("Content/Item6/Label").GetComponent<TextMeshProUGUI>().text = "5000";
        GemsPanel.transform.Find("Content/Item6/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 50%";
        GemsPanel.transform.Find("Content/Item6/SubLabel").GetComponent<TextMeshProUGUI>().text = "Cart of Gems";
        GemsPanel.transform.Find("Content/Item6/Price").GetComponent<TextMeshProUGUI>().text = "24.99$";
        GemsPanel.transform.Find("Content/Item6").GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(5000, 24.99));

    }
    void InitCoins()
    {
        //Item 1
        CoinsPanel.transform.Find("Content/Item1/Label").GetComponent<TextMeshProUGUI>().text = "200";
        //CoinsPanel.transform.Find("Content/Item1/Description/Text").GetComponent<TextMeshProUGUI>().text = "";
        CoinsPanel.transform.Find("Content/Item1/SubLabel").GetComponent<TextMeshProUGUI>().text = "Pile of Coins";
        //CoinsPanel.transform.Find("Content/Item1/Price").GetComponent<TextMeshProUGUI>().text = "0.99$";
        CoinsPanel.transform.Find("Content/Item1/Button/Watch").GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(200, 0));

        //Item 2
        CoinsPanel.transform.Find("Content/Item2/Label").GetComponent<TextMeshProUGUI>().text = "500";
        //CoinsPanel.transform.Find("Content/Item2/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 10%";
        CoinsPanel.transform.Find("Content/Item2/SubLabel").GetComponent<TextMeshProUGUI>().text = "Heap of Coins";
        CoinsPanel.transform.Find("Content/Item2/PriceTransform/Price").GetComponent<TextMeshProUGUI>().text = "<sprite=0>100";
        CoinsPanel.transform.Find("Content/Item2").GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(500, 100));

        //Item 3
        CoinsPanel.transform.Find("Content/Item3/Label").GetComponent<TextMeshProUGUI>().text = "1000";
        //CoinsPanel.transform.Find("Content/Item3/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 20%";
        CoinsPanel.transform.Find("Content/Item3/SubLabel").GetComponent<TextMeshProUGUI>().text = "Bucket of Coins";
        CoinsPanel.transform.Find("Content/Item3/PriceTransform/Price").GetComponent<TextMeshProUGUI>().text = "<sprite=0>180";
        CoinsPanel.transform.Find("Content/Item3").GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(1000, 180));

        //Item 4
        CoinsPanel.transform.Find("Content/Item4/Label").GetComponent<TextMeshProUGUI>().text = "2000";
        //CoinsPanel.transform.Find("Content/Item4/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 30%";
        CoinsPanel.transform.Find("Content/Item4/SubLabel").GetComponent<TextMeshProUGUI>().text = "Barrel of Coins";
        CoinsPanel.transform.Find("Content/Item4/PriceTransform/Price").GetComponent<TextMeshProUGUI>().text = "<sprite=0>320";
        CoinsPanel.transform.Find("Content/Item4").GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(2000, 320));

        //Item 5
        CoinsPanel.transform.Find("Content/Item5/Label").GetComponent<TextMeshProUGUI>().text = "5000";
        //CoinsPanel.transform.Find("Content/Item5/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 40%";
        CoinsPanel.transform.Find("Content/Item5/SubLabel").GetComponent<TextMeshProUGUI>().text = "Chest of Coins";
        CoinsPanel.transform.Find("Content/Item5/PriceTransform/Price").GetComponent<TextMeshProUGUI>().text = "<sprite=0>700";
        CoinsPanel.transform.Find("Content/Item5").GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(5000, 700));

        //Item 6
        CoinsPanel.transform.Find("Content/Item6/Label").GetComponent<TextMeshProUGUI>().text = "10000";
        //CoinsPanel.transform.Find("Content/Item6/Description/Text").GetComponent<TextMeshProUGUI>().text = "Discount 50%";
        CoinsPanel.transform.Find("Content/Item6/SubLabel").GetComponent<TextMeshProUGUI>().text = "Cart of Coins";
        CoinsPanel.transform.Find("Content/Item6/PriceTransform/Price").GetComponent<TextMeshProUGUI>().text = "<sprite=0>1200";
        CoinsPanel.transform.Find("Content/Item6").GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(10000, 1200));

    }
    void OnTargetOfferPurchased(string type, double price)
    {

    }
    void OnChestPurchased(string type, int quantity, int price)
    {
        var itemDb = ItemDatabase.Instance;
        var userdb = UserDatabase.Instance;
        if (type.Equals("coin"))
        {
            //Open golden chest
            userdb.reduceMoney(price, 0);
            itemDb.addNewItem(Random.Range(9, 36), 1);
        }
        else
        {
            //Open Diamon chest
            userdb.reduceMoney(0, price);
            for (int i = 0; i < quantity; ++i)
            {
                itemDb.addNewItem(Random.Range(9, 36), 1);
            }
        }
    }
    void OnHeroesPurchased(int id, int price)
    {

    }
    void OnDiamondPurchased(int value, double price)
    {
        var userdb = UserDatabase.Instance;

        userdb.gainMoney(0, value);
     
    }
    void OnCoinPurchased(int value, int price)
    {
        var userdb = UserDatabase.Instance;

        userdb.gainMoney(value, 0);
        userdb.reduceMoney(0, price);
    }
}
