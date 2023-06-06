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

 

    public GameObject[] Gems_Label;
    public GameObject[] Gems_Description_Text;
    public GameObject[] Gems_SubLabel;
    public GameObject[] Gems_Price;
    public GameObject[] Gems_Btn;

    public GameObject[] Coins_Label;
    public GameObject[] Coins_SubLabel;
    public GameObject[] Coins_Price;
    public GameObject[] Coins_Btn;

    public GameObject celebrationObj;

    void Start()
    {
        InitTargetedOffer();
        InitGems();
        InitCoins();
    }
    void InitTargetedOffer()
    {

        //init data

        GameObject TO = new GameObject();
        int to = UnityEngine.Random.Range(1, 6);
        SetUpTO(TO, to);
    }

    void SetUpTO(GameObject TO, int index)
    {
        TO = Instantiate(Resources.Load($"Prefabs/UI/TargetOffer/TO_{index}") as GameObject, TargetedOfferPanel.transform.GetChild(0).transform);
        TO.GetComponent<Button>().onClick.AddListener(() => OnTargetOfferPurchased(index, StaticInfo.TOBaseValue[index - 1]));
    }

    void InitGems()
    {
        //Item 1
        Gems_Label[0].GetComponent<TextMeshProUGUI>().text = "100";
        Gems_Description_Text[0].GetComponent<TextMeshProUGUI>().text = "";
        Gems_SubLabel[0].GetComponent<TextMeshProUGUI>().text = "Pile of Gems";
        Gems_Price[0].GetComponent<TextMeshProUGUI>().text = "0.99$";
        Gems_Btn[0].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(100, 0.99));

        //Item 2
        Gems_Label[1].GetComponent<TextMeshProUGUI>().text = "200";
        Gems_Description_Text[1].GetComponent<TextMeshProUGUI>().text = "Discount 10%";
        Gems_SubLabel[1].GetComponent<TextMeshProUGUI>().text = "Heap of Gems";
        Gems_Price[1].GetComponent<TextMeshProUGUI>().text = "1.79$";
        Gems_Btn[1].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(200, 1.79));

        //Item 3
        Gems_Label[2].GetComponent<TextMeshProUGUI>().text = "500";
        Gems_Description_Text[2].GetComponent<TextMeshProUGUI>().text = "Discount 20%";
        Gems_SubLabel[2].GetComponent<TextMeshProUGUI>().text = "Bucket of Gems";
        Gems_Price[2].GetComponent<TextMeshProUGUI>().text = "3.99$";
        Gems_Btn[2].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(500, 3.99));

        //Item 4
        Gems_Label[3].GetComponent<TextMeshProUGUI>().text = "1000";
        Gems_Description_Text[3].GetComponent<TextMeshProUGUI>().text = "Discount 30%";
        Gems_SubLabel[3].GetComponent<TextMeshProUGUI>().text = "Barrel of Gems";
        Gems_Price[3].GetComponent<TextMeshProUGUI>().text = "6.99$";
        Gems_Btn[3].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(1000, 6.99));

        //Item 5
        Gems_Label[4].GetComponent<TextMeshProUGUI>().text = "2000";
        Gems_Description_Text[4].GetComponent<TextMeshProUGUI>().text = "Discount 40%";
        Gems_SubLabel[4].GetComponent<TextMeshProUGUI>().text = "Chest of Gems";
        Gems_Price[4].GetComponent<TextMeshProUGUI>().text = "11.99$";
        Gems_Btn[4].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(2000, 11.99));

        //Item 6
        Gems_Label[5].GetComponent<TextMeshProUGUI>().text = "5000";
        Gems_Description_Text[5].GetComponent<TextMeshProUGUI>().text = "Discount 50%";
        Gems_SubLabel[5].GetComponent<TextMeshProUGUI>().text = "Cart of Gems";
        Gems_Price[5].GetComponent<TextMeshProUGUI>().text = "24.99$";
        Gems_Btn[5].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(5000, 24.99));

    }
    void InitCoins()
    {
        //Item 1
        Coins_Label[0].GetComponent<TextMeshProUGUI>().text = "200";
        Coins_SubLabel[0].GetComponent<TextMeshProUGUI>().text = "Pile of Coins";
        //Coins_Price[0].GetComponent<TextMeshProUGUI>().text = "0.99$";
        Coins_Btn[0].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(200, 0));

        //Item 2
        Coins_Label[1].GetComponent<TextMeshProUGUI>().text = "500";
        Coins_SubLabel[1].GetComponent<TextMeshProUGUI>().text = "Heap of Coins";
        Coins_Price[1].GetComponent<TextMeshProUGUI>().text = "<sprite=0>100";
        Coins_Btn[1].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(500, 100));

        //Item 3
        Coins_Label[2].GetComponent<TextMeshProUGUI>().text = "1000";
        Coins_SubLabel[2].GetComponent<TextMeshProUGUI>().text = "Bucket of Coins";
        Coins_Price[2].GetComponent<TextMeshProUGUI>().text = "<sprite=0>180";
        Coins_Btn[2].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(1000, 180));

        //Item 4
        Coins_Label[3].GetComponent<TextMeshProUGUI>().text = "2000";
        Coins_SubLabel[3].GetComponent<TextMeshProUGUI>().text = "Barrel of Coins";
        Coins_Price[3].GetComponent<TextMeshProUGUI>().text = "<sprite=0>320";
        Coins_Btn[3].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(2000, 320));

        //Item 5
        Coins_Label[4].GetComponent<TextMeshProUGUI>().text = "5000";
        Coins_SubLabel[4].GetComponent<TextMeshProUGUI>().text = "Chest of Coins";
        Coins_Price[4].GetComponent<TextMeshProUGUI>().text = "<sprite=0>700";
        Coins_Btn[4].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(5000, 700));

        //Item 6
        Coins_Label[5].GetComponent<TextMeshProUGUI>().text = "10000";
        Coins_SubLabel[5].GetComponent<TextMeshProUGUI>().text = "Cart of Coins";
        Coins_Price[5].GetComponent<TextMeshProUGUI>().text = "<sprite=0>1200";
        Coins_Btn[5].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(10000, 1200));

    }
    void OnTargetOfferPurchased(int type, double price)
    {
        List<ItemInventory> items = new List<ItemInventory>();
        int gold = 0;
        int diamond = 0;
        switch (type)
        {
            case 1 :
                items.Add(ItemDatabase.Instance.getItemObject(1, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(2, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(3, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(4, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(5, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(6, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(7, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(8, 10, 1));
                break;
            case 2 :
                UserDatabase.Instance.gainMoney(0, 1000);
                diamond = 1000;
                break;
            case 3:
                UserDatabase.Instance.gainMoney(10000, 0);
                gold = 10000;
                break;
            case 4:
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 3));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 3));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 3));
                diamond = 500;
                gold = 5000;
                break;
            case 5:
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 14), 1, returnRarityTO()));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(14, 18), 1, returnRarityTO()));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(18, 22), 1, returnRarityTO()));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(22, 26), 1, returnRarityTO()));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(26, 30), 1, returnRarityTO()));
                items.Add(ItemDatabase.Instance.getItemObject(Random.Range(30, 34), 1, returnRarityTO()));
                break;
        }

        // celebration
        openCelebration(items, gold, diamond);
    }
    private int returnRarityTO()
    {
        int rate = Random.Range(0, 100);
        if (rate < 80)
        {
            return 3;
        } else if (rate < 97)
        {
            return 4;
        } else
        {
            return 5;
        }
    }
    private void openCelebration(List<ItemInventory> items, int gold, int diamond)
    {
        for(int i = 0; i < items.Count; i++)
        {
            ItemDatabase.Instance.addNewItemByObject(items[i]);
        }
        UserDatabase.Instance.gainMoneyInGame(0, gold);
        UserDatabase.Instance.gainMoneyInGame(diamond, 0);
        InventoryController.Instance.initEquipment();
            
        celebrationObj.SetActive(true);
        celebrationObj.GetComponent<CelebrationShopController>().initCelebration(items, gold, diamond);
    }
    void OnChestPurchased(string type, int quantity, int price)
    {
        var itemDb = ItemDatabase.Instance;
        var userdb = UserDatabase.Instance;
        if (type.Equals("coin"))
        {
            //Open golden chest
            userdb.reduceMoney(price, 0);
            int rate = Random.Range(0, 101);

            if (rate <= 30)
            {
                itemDb.addNewItem(Random.Range(1, 3), 1);
            }
            else if (rate <= 35)
            {
                itemDb.addNewItem(3, 1);
            }
            else if (rate <= 65)
            {
                itemDb.addNewItem(Random.Range(4, 10), 1);

            }
            else if (rate <= 93)
            {
                itemDb.addNewItem(Random.Range(10, 34), 1, 1);

            }
            else if (rate <= 98)
            {
                itemDb.addNewItem(Random.Range(10, 34), 1, 2);

            }
            else
            {
                itemDb.addNewItem(Random.Range(101, 113), 1);

            }
            
        }
        else
        {
            //Open Diamon chest
            userdb.reduceMoney(0, price);
            int rate;
            for (int i = 0; i < quantity; ++i)
            {
                rate = Random.Range(0, 101);
                if (rate <= 20)
                {
                    itemDb.addNewItem(Random.Range(1, 3), 1);
                }
                else if (rate <= 30)
                {
                    itemDb.addNewItem(3, 1);
                }
                else if (rate <= 40)
                {
                    itemDb.addNewItem(Random.Range(4, 10), 1 );

                }
                else if (rate <= 60)
                {
                    itemDb.addNewItem(Random.Range(10, 34), 1, 1);

                }
                else if (rate <= 70)
                {
                    itemDb.addNewItem(Random.Range(10, 34), 1, 2);

                }
                else if (rate <= 77)
                {
                    itemDb.addNewItem(Random.Range(10, 34), 1, 3);

                }
                else if (rate <= 80)
                {
                    itemDb.addNewItem(Random.Range(10, 34), 1, 4);

                }
                else
                {
                    itemDb.addNewItem(Random.Range(101, 113), 1);

                }
            }
            // tru tien that
        }
    }
    void OnHeroesPurchased(int id, int price)
    {

    }
    void OnDiamondPurchased(int value, double price)
    {
        var userdb = UserDatabase.Instance;

        userdb.gainMoney(0, value);
        // tru tien that
     
    }
    void OnCoinPurchased(int value, int price)
    {
        var userdb = UserDatabase.Instance;

        userdb.gainMoney(value, 0);
        userdb.reduceMoney(0, price);
    }
}
