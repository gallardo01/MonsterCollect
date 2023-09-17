using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;


public class PurchaseService : Singleton<PurchaseService>
{
    public enum ProductId
    {
        // Targeted Offers
        evolve_stone_pack,
        gem_pack,
        gold_pack,
        newbie_pack,
        equipment_pack,

        // Gems
        gem_pile,
        gem_heap,
        gem_bucket,
        gem_barrel,
        gem_chest,
        gem_cart,

        // Heroes
        unlock_all_heroes,
        elephany,       // 5
        torchic,        // 6
        tiny,           // 7
        porcupine,      // 8
        flychooper,     // 9
        tailtiger,      // 10
        hempfire,       // 11
        sleepine,       // 12,

        phoenix,        // 6
        tinyhero,       // 7
        kingcune,       // 8
        giantchooper,   // 9
        garchamp,       // 10
        kingfire,       // 11
        dragonpine,     // 12,
    }

    private enum PurchaseState
    {
        requesting,
        successful,
        failed
    }

    private Dictionary<string, IBillingProduct> productDict = new();
    private ConcurrentDictionary<string, PurchaseState> purchaseStates = new();

    // Start is called before the first frame update
    void Start()
    {
        if (BillingServices.IsAvailable())
        {
            Debug.Log("Billing Service available");
            BillingServices.InitializeStore();
        } else
        {
            Debug.Log("Billing Service not available");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    private void OnEnable()
    {
        // register for events
        BillingServices.OnInitializeStoreComplete += OnInitializeStoreComplete;
        BillingServices.OnTransactionStateChange += OnTransactionStateChange;
        BillingServices.OnRestorePurchasesComplete += OnRestorePurchasesComplete;
    }

    private void OnDisable()
    {
        // unregister from events
        BillingServices.OnInitializeStoreComplete -= OnInitializeStoreComplete;
        BillingServices.OnTransactionStateChange -= OnTransactionStateChange;
        BillingServices.OnRestorePurchasesComplete -= OnRestorePurchasesComplete;
    }

    public string GetPrice(ProductId productId, string defaultValue)
    {
        if (productDict.TryGetValue(productId.ToString(), out IBillingProduct product))
        {
            return product.LocalizedPrice;
        } else
        {
            return defaultValue;
        }
    }

    public async Task<bool> Purchase(ProductId productId)
    {
        string id = productId.ToString();

        if (BillingServices.CanMakePayments())
        {
            var product = productDict[id];
            if (product == null)
            {
                Debug.Log("Unable to find product: " + productId);
                return false;
            }

            var canPurchase = purchaseStates.TryAdd(id, PurchaseState.requesting);

            if (canPurchase)
            {
                Debug.Log("Purchasing product: " + productId);
                BillingServices.BuyProduct(product);

                while (purchaseStates[id] == PurchaseState.requesting)
                {
                    await Task.Delay(15);
                }

                purchaseStates.TryRemove(id, out PurchaseState finalState);
                return finalState == PurchaseState.successful;
                
            } else
            {
                Debug.Log(string.Format("Another purchase of id '{0}' is awaiting completion.", productId));
                return false;
            }
        } else
        {
            return false;
        }
    }

    public void Restore()
    {
        BillingServices.RestorePurchases();
    }


    private void OnInitializeStoreComplete(BillingServicesInitializeStoreResult result, Error error)
    {
        if (error == null)
        {
            // update UI
            // show console messages
            var products = result.Products;
            Debug.Log("Store initialized successfully.");
            Debug.Log("Total products fetched: " + products.Length);
            Debug.Log("Below are the available products:");
            for (int iter = 0; iter < products.Length; iter++)
            {
                // Disable purchase button here
                // productButton[iter].interactable = !BillingServices.IsProductPurchased(productId[iter]);
                var product = products[iter];
                productDict[product.Id] = product;
                Debug.Log(string.Format("[{0}]: {1}", iter, product));
            }
        }
        else
        {
            Debug.Log("Store initialization failed with error. Error: " + error);
        }

        var invalidIds = result.InvalidProductIds;
        Debug.Log("Total invalid products: " + invalidIds.Length);
        if (invalidIds.Length > 0)
        {
            Debug.Log("Here are the invalid product ids:");
            for (int iter = 0; iter < invalidIds.Length; iter++)
            {
                Debug.Log(string.Format("[{0}]: {1}", iter, invalidIds[iter]));
            }
        }
    }

    private void OnTransactionStateChange(BillingServicesTransactionStateChangeResult result)
    {
        var transactions = result.Transactions;
        for (int iter = 0; iter < transactions.Length; iter++)
        {
            var transaction = transactions[iter];
            var productId = transaction.Payment.ProductId;
            switch (transaction.TransactionState)
            {
                case BillingTransactionState.Purchased:
                    
                    Debug.Log(string.Format("Buy product with id:{0} finished successfully.", productId));
                    purchaseStates[productId] = PurchaseState.successful;
                    break;

                case BillingTransactionState.Failed:
                    purchaseStates[productId] = PurchaseState.failed;
                    Debug.Log(string.Format("Buy product with id:{0} failed with error. Error: {1}", transaction.Payment.ProductId, transaction.Error));
                    break;
            }
        }
    }

    private void OnRestorePurchasesComplete(BillingServicesRestorePurchasesResult result, Error error)
    {
        if (error == null)
        {
            var transactions = result.Transactions;
            Debug.Log("Request to restore purchases finished successfully.");
            Debug.Log("Total restored products: " + transactions.Length);

            for (int iter = 0; iter < transactions.Length; iter++)
            {
                var transaction = transactions[iter];
                var productId = transaction.Payment.ProductId;

                if (transaction.ReceiptVerificationState == BillingReceiptVerificationState.Success)
                {
                    // TODO: call restore rewards.
                } 

                Debug.Log(string.Format("[{0}]: {1}", iter, productId));
            }
        }
        else
        {
            Debug.Log("Request to restore purchases failed with error. Error: " + error);
        }
    }
}
