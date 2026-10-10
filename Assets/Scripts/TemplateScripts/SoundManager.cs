using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
  * Scene:All
  * Object:SoundManager
  * Description: Skripta zaduzena za zvuke u apliakciji, njihovo pustanje, gasenje itd...
  **/
public class SoundManager : MonoBehaviour {

	public static int musicOn = 1;
	public static int soundOn = 1;
	public static bool forceTurnOff = false;

	// FX
	public AudioSource buttonClick;
	public AudioSource popupArrive;
	public AudioSource bombSound;
	public AudioSource inappBought;
	public AudioSource minusSec;
	public AudioSource plusSec;
	public AudioSource baloonPop;
	public AudioSource toyPop;

	// Music
	public AudioSource menuMusic;

	public GameObject musicObjectsHolder;
	public GameObject fxObjectsHolder;

	// Music on off sprites
	public Sprite musicOffImageHolder;
	public Sprite musicOffSprite;
	public Sprite musicOnImageHolder;
	public Sprite musicOnSprite;

	static SoundManager instance;

	public static SoundManager Instance
	{
		get
		{
			if(instance == null)
			{
				instance = GameObject.FindObjectOfType(typeof(SoundManager)) as SoundManager;
			}

			return instance;
		}
	}

	public void OnLevelWasLoaded(int level)
	{
		if (level == 1)
		{
			if (!IsSoundPlaying("Music"))
				PlaySound("Music");
		}
//		else if (level == 2)
//			StopSound("Music");
	}

	void Start () 
	{
		DontDestroyOnLoad(this.gameObject);

		if(PlayerPrefs.HasKey("SoundOn"))
		{
			musicOn = PlayerPrefs.GetInt("MusicOn");
			soundOn = PlayerPrefs.GetInt("SoundOn");
		}
		else
		{
			PlayerPrefs.SetInt("SoundOn", 1);
			PlayerPrefs.SetInt("MusicOn", 1);
			musicOn = 1;
			soundOn = 1;
		}

		Screen.sleepTimeout = SleepTimeout.NeverSleep; 
	}

	public static void ToggleSound()
	{
		if (soundOn == 0)
		{
			soundOn = 1;
			PlayerPrefs.SetInt("SoundOn", 1);

			// Unmute all sounds just in case
			foreach (Transform t in Instance.transform)
			{
				t.GetComponent<AudioSource>().mute = false;
			}

			// Play menu music
			PlaySound("Music");

			//GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(1).GetComponent<Image>().enabled = false;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOnImageHolder;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOnSprite;
		}
		else if (soundOn == 1)
		{
			soundOn = 0;
			PlayerPrefs.SetInt("SoundOn", 0);

			// Mute all sounds just in case
			foreach (Transform t in Instance.transform)
			{
				t.GetComponent<AudioSource>().mute = true;
			}

			// GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(1).GetComponent<Image>().enabled = true;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOffImageHolder;
			GameObject.Find("Canvas").GetComponent<MenuManager>().soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOffSprite;
		}
	}

	public static void PlaySound(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
				case "Music":
					Instance.menuMusic.Play();
					break;
				case "ButtonClick":
					Instance.buttonClick.Play();
					break;
				case "PopupArrive":
					Instance.popupArrive.Play();
					break;
				case "BombSound":
					Instance.bombSound.Play();
					break;
				case "InappBought":
					Instance.inappBought.Play();
					break;
				case "MinusSec":
					Instance.minusSec.Play();
					break;
				case "PlusSec":
					Instance.plusSec.Play();
					break;
				case "BaloonPop":
					Instance.baloonPop.Play();
					break;
				case "ToyPop":
					Instance.toyPop.Play();
					break;
				default:
					break;
			}
		}
	}

	public static void StopSound(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
			case "Music":
				Instance.StartCoroutine(Instance.FadeOut(Instance.menuMusic, 0.03f));
				break;
			default:
				break;
			}
		}
	}

	public static bool IsSoundPlaying(string soundName)
	{
		if (soundOn == 1)
		{
			switch(soundName)
			{
			case "Music":
				if (Instance.menuMusic.isPlaying)
					return true;
				break;
			case "ButtonClick":
				if (Instance.buttonClick.isPlaying)
					return true;
				break;
			case "PopupArrive":
				if (Instance.popupArrive.isPlaying)
					return true;
				break;
			case "BombSound":
				if (Instance.bombSound.isPlaying)
					return true;
				break;
			case "InappBought":
				if (Instance.inappBought.isPlaying)
					return true;
				break;
			case "MinusSec":
				if (Instance.minusSec.isPlaying)
					return true;
				break;
			case "PlusSec":
				if (Instance.plusSec.isPlaying)
					return true;
				break;
			case "BaloonPop":
				if (Instance.baloonPop.isPlaying)
					return true;
				break;
			case "ToyPop":
				if (Instance.toyPop.isPlaying)
					return true;
				break;
			default:
				break;
			}
		}

		return false;
	}

	/// <summary>
	/// Corutine-a koja za odredjeni AudioSource, kroz prosledjeno vreme, utisava AudioSource do 0, gasi taj AudioSource, a zatim vraca pocetni Volume na pocetan kako bi AudioSource mogao opet da se koristi
	/// </summary>
	/// <param name="sound">AudioSource koji treba smanjiti/param>
	/// <param name="time">Vreme za koje treba smanjiti Volume/param>
	IEnumerator FadeOut(AudioSource sound, float time)
	{
		float originalVolume = 1f;

		while(sound.volume != 0)
		{
			sound.volume = Mathf.MoveTowards(sound.volume, 0, time);
			yield return null;
		}

		sound.Stop();
		sound.volume = originalVolume;
	}
	
}
