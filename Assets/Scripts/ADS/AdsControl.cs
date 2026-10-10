using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GoogleMobileAds.Api;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using GoogleMobileAds.Common;
/*
 * 
 * Document for Unity Ads : https://docs.unity.com/ads/ImplementingBasicAdsUnity.html
 */
/*
 * 
 * Document for Google Admob : https://developers.google.com/admob/unity/quick-start
 */

public class AdsControl : MonoBehaviour
{

    public static AdsControl instance;


    //for Admob

    public string Android_AppID, IOS_AppID;


    public string Android_Interestital_Key, IOS_Interestital_Key;

    public string Android_RW_Key, IOS_RW_Key;

    private AppOpenAd appOpenAd;
    private RewardedAd rewardedAd;
    private RewardedInterstitialAd rewardedInterstitialAd;
    private bool isShowingAppOpenAd;

    UnityEvent OnAdLoadedEvent;
    UnityEvent OnAdFailedToLoadEvent;
    UnityEvent OnAdOpeningEvent;
    UnityEvent OnAdFailedToShowEvent;
    UnityEvent OnUserEarnedRewardEvent;
    UnityEvent OnAdClosedEvent;

    public int adCurrent;
    string is_premium;

    public GameObject AdLoadedStatus;
    private InterstitialAd interstitialAd;
    private RewardedAd _rewardedAd;

    // Static flag: ensures MobileAds.Initialize() is called only once per process,
    // even if both AdsControl and AdsManager exist in the scene.
    private static bool _adsInitialized = false;




    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
  private string _adUnitId ="unused";
  private const string _adUnitIdrewarded ="unused";
#elif UNITY_IPHONE
    private string _adUnitId = "ca-app-pub-8345838164321650/5292567110";
    private const string _adUnitIdrewarded = "ca-app-pub-8345838164321650/3979485446";
#else
  private string _adUnitId = "unused";
  private const string _adUnitIdrewarded = "unused";
#endif



    public static AdsControl Instance { get { return instance; } }

    void Awake()
    {
        if (FindObjectsOfType(typeof(AdsControl)).Length > 1)
        {
            Destroy(gameObject);
            return;
        }


        instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private void Start()
    {
        // Guard: only initialize MobileAds once across the entire app lifetime.
        // AdsManager may have already called MobileAds.Initialize() in its own Start().
        // Calling Initialize() twice causes duplicate load callbacks and race conditions.
        if (!_adsInitialized)
        {
            _adsInitialized = true;
            // Initialize the Google Mobile Ads SDK.
            MobileAds.Initialize((InitializationStatus initStatus) =>
            {
                // This callback is called once the MobileAds SDK is initialized.
                LoadRewardedAd();
                LoadInterstitialAd();
            });
        }
        else
        {
            // SDK was already initialized (by AdsManager). Just load ads directly.
            LoadRewardedAd();
            LoadInterstitialAd();
        }

        is_premium = PlayerPrefs.GetString("GoPremium");
    }



    #region INTERSTITIAL ADS
    public void LoadInterstitialAd()
    {
       
        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // NOTE: Do NOT add "unity-admob-sample" keyword — it forces Google to serve test ads.

        // send the request to load the ad.
        InterstitialAd.Load(_adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
            });
    }
    public void ShowInterstitialAd()
    {

        if (PlayerPrefs.GetString("GoPremium") == "premium")
        {
            Debug.Log("pack premium no ads");
        }
        else
        {
            if (interstitialAd.CanShowAd())
            {
                interstitialAd.Show();
                LoadInterstitialAd();
            }

        }
    }
    public void ShowInterstitalRandom()
    {
        if (PlayerPrefs.GetString("GoPremium") == "premium")
        {
            Debug.Log("pack premium no ads");
        }
        else
        {
            StartCoroutine(ShowInterstitalRandomIE());
        }
    }
    IEnumerator ShowInterstitalRandomIE()
    {
        yield return new WaitForSeconds(0.5f);
        ShowInterstitialAd();
        if (adCurrent >= 1)
        {
             ShowInterstitialAd();
            adCurrent = 0;
        }
        else
            adCurrent++;
    }
    public void ShowInterstitalRandomCurrent()
    {
        if (PlayerPrefs.GetString("GoPremium") == "premium")
        {
            Debug.Log("pack premium no ads");
        }
        else
        {
            StartCoroutine(ShowInterstitalRandomIECurrent());
        }
    }
    IEnumerator ShowInterstitalRandomIECurrent()
    {
        yield return new WaitForSeconds(0.5f);

        if (adCurrent >= 2)
        {
            ShowInterstitialAd();
            adCurrent = 0;
        }
        else
            adCurrent++;
    }
    public void DestroyInterstitialAd()
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
        }
    }

    #endregion

    #region REWARDED ADS

    //


    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        // NOTE: Do NOT add "unity-admob-sample" keyword — it forces Google to serve test ads.

        // send the request to load the ad.
        RewardedAd.Load(_adUnitIdrewarded, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
                RegisterEventHandlers(rewardedAd);
            });
    }

    public void ShowRewardedAd()
    {
        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));
            });
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
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
//            swipe.USE.RewardClose();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            LoadRewardedAd();
        };
    }




    #endregion

    #region HELPER METHODS

    private AdRequest CreateAdRequest()
    {
        var adRequest = new AdRequest();
        // NOTE: Do NOT add "unity-admob-sample" keyword — it forces Google to serve test ads.
        return adRequest;
    }

    public void OnApplicationPause(bool paused)
    {
        // Display the app open ad when the app is foregrounded.
        if (!paused)
        {
            // ShowAppOpenAd();
        }
    }

    #endregion
}









