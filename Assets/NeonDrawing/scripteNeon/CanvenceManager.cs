using System;
using UnityEngine;
using UnityEngine.UI;

public class CanvenceManager : MonoBehaviour
{
    private static CanvenceManager _instance;
    public static CanvenceManager Instance { get { return _instance; } }
    public int ColoringCount;
    public GameObject _Content;
    public GameObject PrefabSelect;
    public int contoreX;
    public GameObject MyTransition;

    const string c_NamberOfNeonCardsRewardWatched = "NamberOfNeonCardsRewardWatched";

    static int _namberOfNeonCardsRewardWatched
    {
        get { return PlayerPrefs.GetInt(c_NamberOfNeonCardsRewardWatched, 0); }
        set { PlayerPrefs.SetInt(c_NamberOfNeonCardsRewardWatched, value); }
    }

    void Awake()
    {
        if (_instance != null) throw new Exception();
        _instance = this;
    }

    void Start()
    {
        LoadNeonCards();
    }

    public void LoadNeonCards()
    {
        // Destroy existing card objects before creating new ones
        foreach (Transform child in _Content.transform) Destroy(child.gameObject);

        int n = _namberOfNeonCardsRewardWatched;

        for (int i = 0; i < ColoringCount; i++)
        {
            GameObject _postion = Instantiate(PrefabSelect, _Content.transform);
            Transform t = _postion.transform;
            t.GetChild(0).gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>("_NeonCatgory/0" + i);
            _postion.GetComponent<Button>().AddEventListenerNeon(i, BtnChose);

            if(n > 0) {
                n--;
                t.Find("DarkLock").gameObject.SetActive(false);
            }
        }
    }

    public void ShowRewardAds()
    {
        AdsManager.instance.ShowRewardedAd(() => {
            _namberOfNeonCardsRewardWatched++;
            LoadNeonCards();
        });
    }
    
    public void BtnChose(int i)
    {
        contoreX=i;
        // AdsControl.instance.ShowInterstitialAd();
        MyTransition.transform.GetComponent<LoadSceneManager>()._btn_pindah("NeonGamePlay");
    }
}