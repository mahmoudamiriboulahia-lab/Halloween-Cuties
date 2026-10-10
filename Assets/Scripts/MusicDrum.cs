using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class MusicDrum : MonoBehaviour
{

    public AudioClip[] ListStyleMusic;

     AudioSource audioSource;



    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

    }


    public void PlayMusic(int i)
    {
        audioSource.clip = ListStyleMusic[i];
        audioSource.Play();

        Debug.Log("ok")
;    }

}
