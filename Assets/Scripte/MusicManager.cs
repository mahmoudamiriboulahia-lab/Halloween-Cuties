using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    
    public GameObject[] ListCategory;
    public AudioSource[] LisButton;
    // Start is called before the first frame update
    void Start()
    {
        CategoryMusic(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CategoryMusic(int i)
    {
        for(int j =0;j< LisButton.Length; j++)
        {
            LisButton[j].clip = ListCategory[i].transform.GetChild(j).GetComponent<AudioSource>().clip;
        }
       AdsControl.Instance.ShowInterstitialAd();

    }
}
