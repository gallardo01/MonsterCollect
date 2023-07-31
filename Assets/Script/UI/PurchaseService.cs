using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VoxelBusters.CoreLibrary;
using VoxelBusters.EssentialKit;


public class PurchaseService : MonoBehaviour
{
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

    public void Purchase(int productIndex)
    {
        if (BillingServices.CanMakePayments())
        {
            BillingServices.BuyProduct(BillingServices.Products[productIndex]);
        }
    }

    public void Restore()
    {
        BillingServices.RestorePurchases();
    }

    private void RewardPurchase(string productId)
    {
        // Call UIShopController.OnXXXPurchased here
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
            switch (transaction.TransactionState)
            {
                case BillingTransactionState.Purchased:
                    Debug.Log(string.Format("Buy product with id:{0} finished successfully.", transaction.Payment.ProductId));
                    RewardPurchase(transaction.Payment.ProductId);
                    break;

                case BillingTransactionState.Failed:
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

                if (transaction.ReceiptVerificationState == BillingReceiptVerificationState.Success)
                {
                    RewardPurchase(transaction.Payment.ProductId);
                }

                Debug.Log(string.Format("[{0}]: {1}", iter, transaction.Payment.ProductId));
            }
        }
        else
        {
            Debug.Log("Request to restore purchases failed with error. Error: " + error);
        }
    }
}
