using UnityEngine;
using UnityEngine.UI;

public class BackBtnInterAd : MonoBehaviour
{
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(BackButtonClicked);
    }

    public void BackButtonClicked()
    {
        if (AdsManager.instance != null)
            AdsManager.instance.ShowAdMobInterstitial();
    }
}