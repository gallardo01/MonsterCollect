using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using DigitalRuby.SoundManagerNamespace;

public class UIShopController : MonoBehaviour
{
    public GameObject ParentTransform;
    public GameObject TargetedOfferPanel;
    public Button watchAds;
    public Button buyGoldenChest;
    public Button buyDiamondChest;
    public Button buyMultipleDiamond;

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

    public Button buyAllHeroes;
    public Button buySingleHeroes;
    private int singleId = 0;
    public TextMeshProUGUI singleName;
    public TextMeshProUGUI singleSaleOff;
    public TextMeshProUGUI singlePrice;
    public TextMeshProUGUI singleOriginalPrice;
    public Image heroesImage;
    public GameObject unlockAll;
    private string[] type = { "", "Fire", "Thunder", "Water", "Grass" };
    private bool isLoadAds = true;
    public GameObject loadAdsDiamond;
    void Start()
    {
        initAds();
        InitTargetedOffer();
        InitGems();
        InitCoins();

        watchAds.onClick.AddListener(() => watchAdsAction());
        buyGoldenChest.onClick.AddListener(() => goldenChest());
        buyDiamondChest.onClick.AddListener(() => diamondChest());
        buyMultipleDiamond.onClick.AddListener(() => mulipleDiamondChest());
        initSellingMonster();
        buyAllHeroes.onClick.AddListener(() => initBuyAlLHeroes());
        buySingleHeroes.onClick.AddListener(() => initBuySingleHeroes());
    }
    private void initAds()
    {
        if (isLoadAds)
        {
            loadAdsDiamond.SetActive(true);
            watchAds.gameObject.SetActive(true);
            Gems_Price[0].SetActive(false);
        } else
        {
            loadAdsDiamond.SetActive(false);
            watchAds.gameObject.SetActive(false);
            Gems_Price[0].SetActive(true);
        }
    }
    private void initBuyAlLHeroes()
    {
        // verify pricing
        List<ItemInventory> availableMonster = new List<ItemInventory>();
        for (int i = 4; i <= 12; i++)
        {
            if (HeroesDatabase.Instance.isUnlockHero(i) == false)
            {
                availableMonster.Add(ItemDatabase.Instance.getItemObject(100 + i, 1, 4));
            }
        }

        celebrationObj.SetActive(true);
        celebrationObj.GetComponent<CelebrationShopController>().initCelebration(availableMonster);
        HeroesDatabase.Instance.unlockAllHeroes();
        initSellingMonster();
    }
    private void initBuySingleHeroes()
    {
        // verify pricing
        List<ItemInventory> availableMonster = new List<ItemInventory>
        {
            ItemDatabase.Instance.getItemObject(100 + singleId, 1, 4)
        };
        celebrationObj.SetActive(true);
        celebrationObj.GetComponent<CelebrationShopController>().initCelebration(availableMonster);
        HeroesDatabase.Instance.unlockHero(singleId*10);
        initSellingMonster();
    }

    private void initSellingMonster()
    {
        List<int> availableMonster = new List<int>();
        for (int i = 6; i <= 12; i++)
        {
            if (HeroesDatabase.Instance.isUnlockHero(i) == false)
            {
                availableMonster.Add(i);
            }
        }
        if(availableMonster.Count > 0)
        {
            int randomValue = Random.Range(0, availableMonster.Count);
            int idSale_1 = availableMonster[randomValue];
            singleId = idSale_1;
            MyHeroes heroes = HeroesDatabase.Instance.fetchLastestEvolve(idSale_1);
            singleName.text = heroes.Name + " - " + type[heroes.Type] + $"<sprite={10+heroes.Type}>";
            singleSaleOff.text = "<size=65><color=yellow>" + (100 - 100 * StaticInfo.newPrice[idSale_1] / StaticInfo.costPrice[idSale_1]) + "% OFF </color></size> \n Unlock your favourite!";
            singlePrice.text = StaticInfo.newPrice[idSale_1] + "$";
            singleOriginalPrice.text = StaticInfo.costPrice[idSale_1] + "$";
            Sprite[] sprite = Resources.LoadAll<Sprite>("UI/Icons/Monster");
            int index = HeroesDatabase.Instance.fetchHeroesIndex(heroes.Id);
            heroesImage.sprite = sprite[index];
            heroesImage.SetNativeSize();

        } else
        {
            buySingleHeroes.interactable = false;
            buyAllHeroes.gameObject.SetActive(false);
            unlockAll.gameObject.SetActive(true);
            heroesImage.gameObject.SetActive(false);
        }
    }
    private void watchAdsAction()
    {
        SoundManagerDemo.Instance.playOneShot(9);
        OnChestPurchased(1, 1, 0, 0);
    }
    private void goldenChest()
    {
        OnChestPurchased(1, 1, 1000, 0);
    }
    private void diamondChest()
    {
        OnChestPurchased(2, 1, 0, 200);
    }
    private void mulipleDiamondChest()
    {
        OnChestPurchased(2, 10, 0, 2000);
    }
    void InitTargetedOffer()
    {
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
        if (isLoadAds)
        {

        }
        //Item 1
        Gems_Label[0].GetComponent<TextMeshProUGUI>().text = "100";
        Gems_Description_Text[0].GetComponent<TextMeshProUGUI>().text = "";
        Gems_SubLabel[0].GetComponent<TextMeshProUGUI>().text = "Pile of Gems";
        Gems_Price[0].GetComponent<TextMeshProUGUI>().text = "0.99$";
        Gems_Btn[0].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(100, 0.99));

        //Item 2
        Gems_Label[1].GetComponent<TextMeshProUGUI>().text = "220";
        Gems_Description_Text[1].GetComponent<TextMeshProUGUI>().text = "Discount 10%";
        Gems_SubLabel[1].GetComponent<TextMeshProUGUI>().text = "Heap of Gems";
        Gems_Price[1].GetComponent<TextMeshProUGUI>().text = "1.99$";
        Gems_Btn[1].GetComponent<Button>().onClick.AddListener(() => OnDiamondPurchased(220, 1.99));

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
        Coins_Label[0].GetComponent<TextMeshProUGUI>().text = "500";
        Coins_SubLabel[0].GetComponent<TextMeshProUGUI>().text = "Pile of Coins";
        Coins_Price[1].GetComponent<TextMeshProUGUI>().text = "100";
        Coins_Btn[0].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(500, 100));

        //Item 2
        Coins_Label[1].GetComponent<TextMeshProUGUI>().text = "1000";
        Coins_SubLabel[1].GetComponent<TextMeshProUGUI>().text = "Heap of Coins";
        Coins_Price[1].GetComponent<TextMeshProUGUI>().text = "200";
        Coins_Btn[1].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(1000, 200));

        //Item 3
        Coins_Label[2].GetComponent<TextMeshProUGUI>().text = "2000";
        Coins_SubLabel[2].GetComponent<TextMeshProUGUI>().text = "Bucket of Coins";
        Coins_Price[2].GetComponent<TextMeshProUGUI>().text = "400";
        Coins_Btn[2].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(2000, 400));

        //Item 4
        Coins_Label[3].GetComponent<TextMeshProUGUI>().text = "5000";
        Coins_SubLabel[3].GetComponent<TextMeshProUGUI>().text = "Barrel of Coins";
        Coins_Price[3].GetComponent<TextMeshProUGUI>().text = "1000";
        Coins_Btn[3].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(5000, 1000));

        //Item 5
        Coins_Label[4].GetComponent<TextMeshProUGUI>().text = "10000";
        Coins_SubLabel[4].GetComponent<TextMeshProUGUI>().text = "Chest of Coins";
        Coins_Price[4].GetComponent<TextMeshProUGUI>().text = "2000";
        Coins_Btn[4].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(10000, 2000));

        //Item 6
        Coins_Label[5].GetComponent<TextMeshProUGUI>().text = "20000";
        Coins_SubLabel[5].GetComponent<TextMeshProUGUI>().text = "Cart of Coins";
        Coins_Price[5].GetComponent<TextMeshProUGUI>().text = "4000";
        Coins_Btn[5].GetComponent<Button>().onClick.AddListener(() => OnCoinPurchased(20000, 4000));

    }
    void OnTargetOfferPurchased(int type, double price)
    {
        SoundManagerDemo.Instance.playOneShot(10);
        List<ItemInventory> items = new List<ItemInventory>();
        int gold = 0;
        int diamond = 0;
        switch (type)
        {
            case 1:
                items.Add(ItemDatabase.Instance.getItemObject(1, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(2, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(3, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(4, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(5, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(6, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(7, 10, 1));
                items.Add(ItemDatabase.Instance.getItemObject(8, 10, 1));
                break;
            case 2:
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
        }
        else if (rate < 97)
        {
            return 4;
        }
        else
        {
            return 5;
        }
    }
    private void openCelebration(List<ItemInventory> items, int gold, int diamond)
    {
        for (int i = 0; i < items.Count; i++)
        {
            ItemDatabase.Instance.addNewItemByObject(items[i]);
        }
        UserDatabase.Instance.gainMoneyInGame(gold, diamond);
        InventoryController.Instance.initEquipment();
        InventoryController.Instance.initMaterial();
        InventoryController.Instance.initShard();
        InventoryController.Instance.initLayout();
        celebrationObj.SetActive(true);
        celebrationObj.GetComponent<CelebrationShopController>().initCelebration(items, gold, diamond);
    }
    void OnChestPurchased(int type, int quantity, int gold, int diamond)
    {
        if (UserDatabase.Instance.getUserData().Gold >= gold && UserDatabase.Instance.getUserData().Diamond >= diamond)
        {
            SoundManagerDemo.Instance.playOneShot(4);
            UserDatabase.Instance.reduceMoney(gold, diamond);
            List<ItemInventory> items = new List<ItemInventory>();
            for (int i = 0; i < quantity; i++)
            {
                if (type == 1)
                {
                    int rate = Random.Range(0, 100);
                    if (rate < 30)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(1, 5), 1, 1));
                    }
                    else if (rate < 70)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(5, 10), Random.Range(1, 3), 1));
                    }
                    else if (rate < 90)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 1));
                    }
                    else if (rate < 98)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 2));
                    }
                    else
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 3));
                    }
                }
                else if (type == 2)
                {
                    int rate = Random.Range(0, 100);
                    if (quantity == 10 && i == 0)
                    {
                        rate = 78;
                    }

                    if (rate < 15)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(1, 5), Random.Range(1, 3), 1));
                    }
                    else if (rate < 30)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(5, 10), Random.Range(1, 4), 1));
                    }
                    else if (rate < 55)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 1));
                    }
                    else if (rate < 70)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 2));
                    }
                    else if (rate < 77)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 3));
                    }
                    else if (rate < 79)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 4));
                    }
                    else if (rate < 80)
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(10, 34), 1, 5));
                    }
                    else
                    {
                        items.Add(ItemDatabase.Instance.getItemObject(Random.Range(101, 113), Random.Range(1, 3), 3));
                    }
                }
            }
            celebrationObj.SetActive(true);
            celebrationObj.GetComponent<CelebrationShopController>().initCelebration(items, 0, 0);
            ItemDatabase.Instance.addNewItemByListObject(items);
            InventoryController.Instance.initItemData();
        } else
        {
            SoundManagerDemo.Instance.playOneShot(12);
        }
    }
    void OnDiamondPurchased(int value, double price)
    {
        if (value == 100 && isLoadAds == true)
        {
            // verify watch ads
        }
        // verify purchases
        SoundManagerDemo.Instance.playOneShot(4);
        celebrationObj.SetActive(true);
        UserDatabase.Instance.gainMoneyInGame(0, value);
        celebrationObj.GetComponent<CelebrationShopController>().initCelebration(null, 0, value);

    }
    void OnCoinPurchased(int value, int price)
    {
        if (UserDatabase.Instance.getUserData().Diamond >= price)
        {
            SoundManagerDemo.Instance.playOneShot(4);
            UserDatabase.Instance.reduceMoney(0, price);
            UserDatabase.Instance.gainMoneyInGame(value, 0);
            celebrationObj.SetActive(true);
            celebrationObj.GetComponent<CelebrationShopController>().initCelebration(null, value, 0);
        }
    }
}
