using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class swipe : MonoBehaviour
{
    public string saveIndexString = "ColoringList";

    public int ColoringCount;
    public GameObject _Content;
    public GameObject PrefabSelect;
    public GameObject MyTransition;
    static GameObject ButtonReward;
    public GameObject WatchVideoPanel;
    public GameObject WatchVideoButton;
    public string Catgory;
    public string MyTransitionName;

    List<Image> ListColoringImage = new List<Image>();

    int indexColring;
    public static swipe USE { get; set; }

    const string c_NamberOfColoringRewardWatched = "NamberOfColoringRewardWatched";

    static int _namberOfColoringRewardWatched
    {
        get { return PlayerPrefs.GetInt(c_NamberOfColoringRewardWatched, 0); }
        set { PlayerPrefs.SetInt(c_NamberOfColoringRewardWatched, value); }
    }

    void Awake()
    {
        USE = this;
    }

    void Start()
    {
        LoadColoringCards();
    }

    public void LoadColoringCards()
    {
        // Destroy existing card objects before creating new ones
        foreach (Transform child in _Content.transform) Destroy(child.gameObject);

        int n = _namberOfColoringRewardWatched;

        for (int i = 0; i < ColoringCount; i++)
        {
            GameObject _postion = Instantiate(PrefabSelect, _Content.transform);
            Transform t = _postion.transform;
            t.GetChild(0).Find("icon").gameObject.GetComponent<Image>().sprite = Resources.Load<Sprite>(Catgory + "/0" + i);
            _postion.name = i + "";
            Button b = _postion.GetComponent<Button>();
            b.AddEventListener(i, ShowInt);
            b.interactable = false;
            MyObject(_postion, t.GetChild(0).Find("icon").gameObject);

            if (n > 0)
            {
                n--;
                b.interactable = true;
                t.GetChild(0).GetChild(0).GetChild(0).GetChild(0).gameObject.SetActive(false);
                t.GetChild(0).GetChild(2).gameObject.SetActive(false);
            }
        }
    }

    public void ShowRewardAds()
    {
        if (AdsManager.instance != null)
        {
            AdsManager.instance.ShowRewardedAd(() => {
                _namberOfColoringRewardWatched++;
                LoadColoringCards();
            });
        }
    }

    public void LoadGame(int index)
    {
        indexColring = index;

        PlayerPrefs.SetInt(saveIndexString, index);
        PlayerPrefs.Save();

        if (_Content.transform.GetChild(index).childCount > 0) ColoringBookManager.maskTexIndex = index;
        else ColoringBookManager.maskTexIndex = -1;

        ColoringBookManager.ID = saveIndexString + index.ToString();
        //SceneManager.LoadScene("GlittersGamePlay");

        MyTransition.transform.GetComponent<LoadSceneManager>()._btn_pindah(MyTransitionName);
    }
    
    public void WatchPanelVideo(GameObject obj)
    {
        if (PlayerPrefs.GetString("GoPremium") == "premium") ShowRewardedAd(obj);
        else {
            if (PlayerPrefs.HasKey("button_alredy_rewaded")) {
                string s = PlayerPrefs.GetString("button_alredy_rewaded");
                if (s.Contains(obj.name)) ShowRewardedAd(obj);
                else {
                    WatchVideoPanel.SetActive(true);
                    WatchVideoButton.GetComponent<Button>().AddEventListener(obj, ShowRewardedAd);
                }
            }
            else {
                WatchVideoPanel.SetActive(true);
                WatchVideoButton.GetComponent<Button>().AddEventListener(obj, ShowRewardedAd);
            }
        }
    }

    public void ShowRewardedAd(GameObject obj)
    {
        int.TryParse(obj.name, out indexColring);

        if (PlayerPrefs.GetString("GoPremium") == "premium") LoadGame(indexColring);
        else {
            if (PlayerPrefs.HasKey("button_alredy_rewaded"))
            {
                string s = PlayerPrefs.GetString("button_alredy_rewaded");
                if (s.Contains(obj.name)) LoadGame(indexColring);
                else {
                    WatchVideoPanel.SetActive(false);
                    AdsManager.instance.ShowRewardedAd(() => { ChangeButton(obj); RewardClose(); });
                }
            }
            else {
                WatchVideoPanel.SetActive(false);
                AdsManager.instance.ShowRewardedAd(() => { ChangeButton(obj); RewardClose(); });
            }
        }
    }

    public void ShowInt(int indexColring)
    {
        // AdsControl.instance.ShowInterstitialAd();
        LoadGame(indexColring);
    }

    public void ChangeButton(GameObject button)
    {
        ButtonReward = button;
    }

    public void RewardClose()
    {
        ButtonReward.transform.GetChild(0).transform.GetChild(2).gameObject.SetActive(false);

        if (PlayerPrefs.HasKey("button_alredy_rewaded"))
        {
            string s = PlayerPrefs.GetString("button_alredy_rewaded") + "_" + ButtonReward.name;
            PlayerPrefs.SetString("button_alredy_rewaded", s);
            PlayerPrefs.Save();
        }
        else {
            PlayerPrefs.SetString("button_alredy_rewaded", ButtonReward.name);
            PlayerPrefs.Save();
        }

        ///GoTo Page Coloring By index
        int.TryParse(ButtonReward.name, out indexColring);
        LoadGame(indexColring);
    }

    public void MyObject(GameObject MyObj, GameObject LockIcon)
    {
        if (PlayerPrefs.GetString("GoPremium") == "premium") LockIcon.SetActive(false);

        if (PlayerPrefs.HasKey("button_alredy_rewaded"))
        {
            string s = PlayerPrefs.GetString("button_alredy_rewaded");
            if (s.Contains(MyObj.name)) {
                Debug.Log("DKHAL");
                LockIcon.SetActive(false);
            }
        }
    }
}