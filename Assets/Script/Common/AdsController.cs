using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdsController : Singleton<AdsController>
{
    private int adsId;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    #if (UNITY_EDITOR || DEVELOPMENT_BUILD)

    // Test Ads
    #if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
    #elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
    #else
        private const string _adUnitId = "unused";
    #endif

    #else

    // Production Ads
    #if UNITY_ANDROID
        private const string _adUnitId = "ca-app-pub-7999860288970453/4602702447";
    #elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-7999860288970453/1976539105";
    #else
        private const string _adUnitId = "unused";
    #endif

    #endif

    private void Start()
    {
        LoadAd();
    }
    private RewardedAd _rewardedAd;

    /// <summary>
    /// Loads the ad.
    /// </summary>
    public void LoadAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            DestroyAd();
        }

        Debug.Log("Loading rewarded ad.");

        // Create our request used to load the ad.
        var adRequest = new AdRequest();

        // Send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // If the operation failed with a reason.
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load an ad with error : " + error);
                return;
            }
            // If the operation failed for unknown reasons.
            // This is an unexpected error, please report this bug if it happens.
            if (ad == null)
            {
                Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
                return;
            }

            // The operation completed successfully.
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;

            // Register to ad events to extend functionality.
            RegisterEventHandlers(ad);

            // Inform the UI that the ad is ready.
            //AdLoadedStatus?.SetActive(true);
        });
    }

    /// <summary>
    /// Shows the ad.
    /// </summary>
    public bool ShowAd(int id)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            adsId = id;
            Debug.Log("Showing rewarded ad.");
            if (id == 1)
            {
                GameFlowController.Instance.pauseWhenWatchAds();
            }
            _rewardedAd.Show((Reward reward) =>
            {
                // success
                if(id == 1)
                {
                    GameFlowController.Instance.reviveSuccess();
                } else if(id == 2)
                {
                    UIShopController.Instance.giveDiamondWatchAds();
                } else if(id == 3)
                {
                    UIShopController.Instance.watchAdsAction();
                }
            });
            LoadAd();
            return true;
        }
        else
        {
            return false;
        }
        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Destroys the ad.
    /// </summary>
    public void DestroyAd()
    {
        if (_rewardedAd != null)
        {
            Debug.Log("Destroying rewarded ad.");
            _rewardedAd.Destroy();
            _rewardedAd = null;

            if (adsId == 1)
            {
                GameFlowController.Instance.reviveSuccess();
            } else if (adsId == 2)
            {
                UIShopController.Instance.giveDiamondWatchAds();
            } else if (adsId == 3)
            {
                UIShopController.Instance.watchAdsAction();
            }

            LoadAd();
        }

        // Inform the UI that the ad is not ready.
        //AdLoadedStatus?.SetActive(false);
    }

    /// <summary>
    /// Logs the ResponseInfo.
    /// </summary>
    public void LogResponseInfo()
    {
        if (_rewardedAd != null)
        {
            var responseInfo = _rewardedAd.GetResponseInfo();
            UnityEngine.Debug.Log(responseInfo);
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when the ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content with error : "
                + error);
            if (adsId == 1)
            {
                GameFlowController.Instance.reviveFailed();
            }
        };
    }
}
