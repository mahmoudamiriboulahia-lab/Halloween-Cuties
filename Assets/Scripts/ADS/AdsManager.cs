using System;
using UnityEngine;
using GoogleMobileAds.Api;
using System.Collections.Generic;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    // Test App ID: ca-app-pub-3940256099942544~3347511713

    const string iosTestAdMobIdInter  = "ca-app-pub-3940256099942544/4411468910";
    const string iosTestAdMobIdReward = "ca-app-pub-3940256099942544/1712485313";

    const string androidTestAdMobIdInter  = "ca-app-pub-3940256099942544/1033173712";
    const string androidTestAdMobIdReward = "ca-app-pub-3940256099942544/5224354917";

    const string iosRealAdMobIdInter  = "ca-app-pub-8345838164321650/5292567110";
    const string iosRealAdMobIdReward = "ca-app-pub-8345838164321650/3979485446";

    const string androidRealAdMobIdInter  = "ca-app-pub-8345838164321650/3858103154";
    const string androidRealAdMobIdReward = "ca-app-pub-8345838164321650/9611226359";

    string _adMobIdInter;
    string _adMobIdReward;

    private RewardedAd _rewardedAd;
    private InterstitialAd _interstitialAd;

    [SerializeField] bool _useTestAds = false;

    private Action onRewardEarned;
    
    float lastAdTime = -999f; // ensures first ad can show
    float cooldown = 150f;    // 2.5 minutes

    private void Awake()
    {
        SetUnitsIDs();
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start() => AdMobInitializeAds();

    public void SetUnitsIDs()
    {
#if UNITY_ANDROID
            if(_useTestAds){
                _adMobIdInter  = androidTestAdMobIdInter;
                _adMobIdReward = androidTestAdMobIdReward;
            }
            else {
                _adMobIdInter  = androidRealAdMobIdInter;
                _adMobIdReward = androidRealAdMobIdReward;
            }
#elif UNITY_IPHONE
            if(_useTestAds){
                _adMobIdInter  = iosTestAdMobIdInter;
                _adMobIdReward = iosTestAdMobIdReward;
            }
            else {
                _adMobIdInter  = iosRealAdMobIdInter;
                _adMobIdReward = iosRealAdMobIdReward;
            }
#else
            _adMobIdInter  = "unused";
            _adMobIdReward = "unused";
#endif
    }

    public void AdMobInitializeAds()
    {
        //---- Initialize the Google Mobile Ads SDK ---------
        MobileAds.Initialize((initStatus) =>
        {
            LoadInterstitialAd();
        });
    }

#region Interstitial Ads

    public void ShowAdMobInterstitial02()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd()) _interstitialAd.Show();
        else Debug.Log("Interstitial ad is not ready yet.");
    }

    public void ShowAdMobInterstitial()
    {
        // Check if cooldown has passed
        if (!CanShowAd())
        {
            Debug.Log("Ad on cooldown...");
            return;
        }
        
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            lastAdTime = Time.time;
            _interstitialAd.Show();
        }
        else Debug.Log("Interstitial ad is not ready yet.");
    }

    public bool CanShowAd()
    {
        return Time.time - lastAdTime >= cooldown;
    }

    void LoadInterstitialAd()
    {
        // Clean up the old ad before loading a new one.
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }
        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("Admob_Interstitial");

        // send the request to load the ad.
        InterstitialAd.Load(_adMobIdInter, adRequest, (InterstitialAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null) {
                Debug.LogError("interstitial ad failed to load an ad " + "with error : " + error);
                LoadRewardedAd();
                return;
            }
            Debug.Log("Interstitial ad loaded with response : " + ad.GetResponseInfo());
            _interstitialAd = ad;
            // Register event handlers only after the ad object is assigned.
            RegisterEventHandlers(_interstitialAd);
            LoadRewardedAd();
        });
    }


    void RegisterEventHandlers(InterstitialAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " + "with error : " + error);
            // Reload the ad so that we can show another as soon as possible.
            LoadInterstitialAd();
        };
    }

#endregion Interstitial Ads

#region Reward Ads

    public void ShowAdMobRewardedAd() 
    {
        const string rewardMsg = "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd()) {
            Debug.Log("<color=blue>Show Ads!</color>");
            _rewardedAd.Show((Reward reward) => {
                // Reward the user.
                Debug.Log(string.Format(rewardMsg, reward.Type, reward.Amount));
            });
        }
    }

    public void ShowRewardedAd(Action rewardCallback)
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            onRewardEarned = rewardCallback;
            _rewardedAd.Show(reward => {
                Debug.Log("User earned reward");
                onRewardEarned?.Invoke();
            });
        }
        else {
            Debug.Log("Ad not ready, loading...");
            LoadRewardedAd();
        }
    }

    void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }
        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();
        adRequest.Keywords.Add("Admob_RewardAd");

        // send the request to load the ad.
        RewardedAd.Load(_adMobIdReward, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            // if error is not null, the load request failed.
            if (error != null || ad == null) {
                Debug.LogError("Rewarded ad failed to load an ad " + "with error : " + error);
                return;
            }
            Debug.Log("Rewarded ad loaded with response : " + ad.GetResponseInfo());
            _rewardedAd = ad;
            // Register event handlers only after the ad object is assigned.
            RegisterEventHandlers(_rewardedAd);
        });
    }


    void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Rewarded ad paid {0} {1}.", adValue.Value, adValue.CurrencyCode));
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
            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " + "with error : " + error);
            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

#endregion Reward Ads
}