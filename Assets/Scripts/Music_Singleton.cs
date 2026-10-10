using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Music_Singleton : MonoBehaviour {
	private static Music_Singleton _instance = null;
	public static Music_Singleton Instance
	{
		get {return _instance ; }
	}

	public bool Mute;
	public Sprite[] Mute_Image;

	public AudioClip[] suara ;
	public List<AudioSource> source;

	public AudioSource SuaraBG;
	public AudioClip[] LaguBGnya;

	void Awake(){
		if(_instance == null){
			_instance = this;
			DontDestroyOnLoad(this);
		}
		else{
			Destroy(this.gameObject);
		}
	}

	void Start(){
		for (int i = 0; i < suara.Length; i++)
		{	
			source.Add(new AudioSource());
			source[i] = gameObject.AddComponent<AudioSource>();
			source[i].clip = suara[i];
		}
	}

	public void s_play(int i){
		source[i].Play();
	}


	public void CekLagu(int i){
		if(i == i){
			if(SuaraBG.clip != LaguBGnya[i]){
				SuaraBG.clip = LaguBGnya[i];
				SuaraBG.Play();
			}
		}
		
	}

	public void btn_muted(){
		s_play(0);
		if(Mute){
			Mute = false;
			for (int i = 0; i < source.Count; i++)
			{
				source[i].mute = false;
			}
			SuaraBG.mute = false;
			GameObject.Find("btn_sounds").GetComponent<Image>().sprite = Mute_Image[0];
			
		}
		else{
			Mute = true;
			for (int i = 0; i < source.Count; i++)
			{
				source[i].mute = true;
			}
			SuaraBG.mute = true;
			GameObject.Find("btn_sounds").GetComponent<Image>().sprite = Mute_Image[1];
		}


	}


	


}
