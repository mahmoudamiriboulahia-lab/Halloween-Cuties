using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/**
  * Scene:All
  * Object:Canvas
  * Description: Skripta zaduzena za hendlovanje(prikaz i sklanjanje svih Menu-ja,njihovo paljenje i gasenje, itd...)
**/
public class MenuManager : MonoBehaviour 
{
	public Menu currentMenu;
	Menu currentPopUpMenu;
//	[HideInInspector]
//	public Animator openObject;
	public GameObject[] disabledObjects;
	GameObject ratePopUp, crossPromotionInterstitial;
	public bool popupOpened;
	public bool popupFullyOpened;
	private bool startInterstitialOpened;
	public GameObject soundButton;
	public GameObject loadingHolder;

	// Popup message components
	public GameObject popupMessageObject;
	public Text popupMessageText;
	public Text popupMessageHeaderText;

	public GameObject leaderboardButton;

//	public FacebookNativeAd mainSceneNativeAd;
	
	void Start () 
	{
		if(Application.loadedLevelName=="MainScene")
		{
			crossPromotionInterstitial = GameObject.Find("PopUps/PopUpInterstitial");
			ratePopUp = GameObject.Find("PopUps/PopUpRate");
		}

		if (disabledObjects!=null) {
			for(int i=0;i<disabledObjects.Length;i++)
			{
				// Debug.Log("Gasi "+disabledObjects[i].name);
				disabledObjects[i].SetActive(false);
			}
		}
		
		if(Application.loadedLevelName != "MapScene")
			ShowMenu(currentMenu.gameObject);
			
		popupOpened = false;
		popupFullyOpened = false;
	}
	
	/// <summary>
	/// Funkcija koja pali(aktivira) objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se upali</param>
	public void EnableObject(GameObject gameObject)
	{
		if (gameObject != null) 
		{
			if (!gameObject.activeSelf) 
			{
				gameObject.SetActive (true);
			}
		}
	}

	/// <summary>
	/// Funkcija koja gasi objekat
	/// </summary>
	/// /// <param name="gameObject">Game object koji se prosledjuje i koji treba da se ugasi</param>
	public void DisableObject(GameObject gameObject)
	{
		Debug.Log("Disable Object");
		if (gameObject != null) 
		{
			if (gameObject.activeSelf) 
			{
				gameObject.SetActive (false);
			}
		}
	}
	
	/// <summary>
	/// F-ja koji poziva ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadScene(string levelName )
	{
		if (levelName != "") {
			try {
//				if (levelName == "MainScene" && Application.loadedLevelName == "CharacterSelect")
//					GlobalVariables.playLoadingDepart = false;
				Application.LoadLevel (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}
	
	/// <summary>
	/// F-ja koji poziva asihrono ucitavanje Scene
	/// </summary>
	/// <param name="levelName">Level name.</param>
	public void LoadSceneAsync(string levelName )
	{
		if (levelName != "") {
			try {
				Application.LoadLevelAsync (levelName);
			} catch (System.Exception e) {
				Debug.Log ("Can't load scene: " + e.Message);
			}
		} else {
			Debug.Log ("Can't load scene: Level name to set");
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao Menu
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ShowMenu(GameObject menu)
	{
		Debug.Log("ShowMwnu   " + menu);
		if (currentMenu != null)
		{
			currentMenu.IsOpen = false;
			currentMenu.gameObject.SetActive(false);
		}

		currentMenu = menu.GetComponent<Menu> ();
		menu.gameObject.SetActive (true);
		currentMenu.IsOpen = true;
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao Meni
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void CloseMenu(GameObject menu)
	{
		Debug.Log("CloseMenu");
		if (menu != null) 
		{
			menu.GetComponent<Menu> ().IsOpen = false;
			menu.SetActive (false);
		}
	}

	/// <summary>
	/// Funkcija za prikaz Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje za prikaz, mora imati na sebi skriptu Menu.</param>
	public void ShowPopUpMenu(GameObject menu)
	{
//		Debug.Log(menu.name);
//		Debug.Log("popupOpened: "    + popupOpened);
//		Debug.Log("popupFullyOpened: "    + popupFullyOpened);

		if (!popupOpened && !popupFullyOpened)
		{
			popupOpened = true;
			menu.gameObject.SetActive (true);
			currentPopUpMenu = menu.GetComponent<Menu> ();
			currentPopUpMenu.IsOpen = true;

//			if (menu.name == "PopUpCrossPromotionOfferWall" || menu.name == "PopUpMessage" || menu.name == "PopUpTutorial")
//			{
//				mainSceneNativeAd.CancelLoading();
//				mainSceneNativeAd.HideNativeAd();
//			}

			SoundManager.PlaySound("PopupArrive");
			StartCoroutine(WaitForPopupToBeFullyOpened());
			AdsControl.instance.ShowInterstitialAd();
		}

//		if (menu.name == "VideoNotAvailable")
//		{
//			menu.gameObject.SetActive (true);
//			currentPopUpMenu = menu.GetComponent<Menu> ();
//			currentPopUpMenu.IsOpen = true;
//		}
	}

	IEnumerator MyAds()
    {
		yield return new WaitForSeconds(2f);
	}

	IEnumerator WaitForPopupToBeFullyOpened()
	{
		yield return new WaitForSeconds(1.2f);
		
		if (popupOpened)
			popupFullyOpened = true;
	}

	/// <summary>
	/// Funkcija za zatvaranje Menu-ja koji je pozvan kao PopUp-a, poziva inace coroutine-u, ima delay zbog animacije odlaska Menu-ja
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje i treba da se skloni, mora imati na sebi skriptu Menu.</param>
	public void ClosePopUpMenu(GameObject menu)
	{
		Debug.Log("ClosePopUpMenu");
		StartCoroutine("HidePopUp",menu);
	}

	/// <summary>
	/// Couorutine-a za zatvaranje Menu-ja koji je pozvan kao PopUp-a
	/// </summary>
	/// /// <param name="menu">Game object koji se prosledjuje, mora imati na sebi skriptu Menu.</param>
	IEnumerator HidePopUp(GameObject menu)
	{
//		SoundManager.Instance.PlayPopupDepartSound();

		menu.GetComponent<Menu> ().IsOpen = false;

//		if (menu.name == "PopUpCrossPromotionOfferWall" || menu.name == "PopUpMessage" || menu.name == "PopUpInterstitial" || menu.name == "PopUpRate" || menu.name == "PopUpTutorial")
//		{
//			mainSceneNativeAd.LoadAd();
//		}

		if (menu.name == "PopUpPause")
			yield return new WaitForSeconds(0.17f);
		else
			yield return new WaitForSeconds(1.2f);

		Debug.Log(menu.name);

//		popupOpened = false;
//		popupFullyOpened = false;

		// if menu is one of the popup bought menus set popup to be opened and set last opened popup to be shop popup
//		if (menu.name == "VideoNotAvailable")
//		{
////			Debug.Log("Minja close test");
////				popupOpened = true;
////				popupFullyOpened = true;
////	
//			currentPopUpMenu = GameObject.Find("PopUpShop").GetComponent<Menu> ();
//		}
//		else
//		{
			popupOpened = false;
			popupFullyOpened = false;
//		}

		if (Application.loadedLevelName == "HospitalSelect")
			GameObject.Find("BackButton").GetComponent<Button>().enabled = true;

		menu.SetActive (false);
	}

	/// <summary>
	/// Funkcija za prikaz poruke preko Log-a, prilikom klika na dugme
	/// </summary>
	/// /// <param name="message">poruka koju treba prikazati.</param>
	public void ShowMessage(string message)
	{
		Debug.Log(message);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessage(string messageTitleText, string messageText)
	{
		popupMessageHeaderText.text=messageTitleText;
		popupMessageText.text=messageText;
		ShowPopUpMenu(popupMessageObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov CustomMessage-a, i ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageCustomMessageText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpMessageTitleText(string messageTitleText)
	{
		popupMessageHeaderText.text=messageTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku CustomMessage, i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpMessageTitleText u redosledu: 1-ShowPopUpMessageTitleText 2-ShowPopUpMessageCustomMessageText
	/// </summary>
	/// <param name="messageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpMessageCustomMessageText(string messageText)
	{
		popupMessageText.text=messageText;		
		ShowPopUpMenu(popupMessageObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga kao i poruku u dialogu i ova f-ja se poziva iz skripte
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialog(string dialogTitleText, string dialogMessageText)
	{
		popupMessageHeaderText.text=dialogTitleText;
		popupMessageText.text=dialogMessageText;
		ShowPopUpMenu(popupMessageObject);
	}

	/// <summary>
	/// Funkcija koja podesava naslov dialoga, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogCustomMessageText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogTitleText">naslov koji treba prikazati.</param>
	public void ShowPopUpDialogTitleText(string dialogTitleText)
	{
		popupMessageHeaderText.text=dialogTitleText;
	}

	/// <summary>
	/// Funkcija koja podesava poruku dialoga i poziva meni u vidu pop-upa, ova f-ja se poziva preko button-a zajedno za f-jom ShowPopUpDialogTitleText u redosledu: 1-ShowPopUpDialogTitleText 2-ShowPopUpDialogCustomMessageText
	/// </summary>
	/// <param name="dialogMessageText">custom poruka koju treba prikazati.</param>
	public void ShowPopUpDialogCustomMessageText(string dialogMessageText)
	{
		popupMessageText.text=dialogMessageText;		
		ShowPopUpMenu(popupMessageObject);
	}

	void Awake()
	{
		// Set sound button
		if (SoundManager.soundOn == 0)
		{
			//soundButton.transform.GetChild(1).GetComponent<Image>().enabled = true;
			soundButton.GetComponent<Image>().sprite = SoundManager.Instance.musicOffImageHolder;
			soundButton.transform.GetChild(0).GetComponent<Image>().sprite = SoundManager.Instance.musicOffSprite;
		}

		StartCoroutine(StartSceneWithLoadingHolderCoroutine());
	}

	public void ToggleSound()
	{
		SoundManager.ToggleSound();
	}

	void Update()
	{
		// Check exit interstitial
		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel == 2 && !popupOpened && !startInterstitialOpened)
			Application.Quit();

		if (Input.GetKeyDown(KeyCode.Escape) && !popupOpened && Application.loadedLevel == 3)
		{
			LevelManager.levelManager.PauseGame();
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && popupOpened && popupFullyOpened && Application.loadedLevel != 3)
        {
            ClosePopUpMenu(currentPopUpMenu.gameObject);
		}
		else if (Input.GetKeyDown(KeyCode.Escape) && popupOpened && Application.loadedLevel == 3)
			LevelManager.levelManager.ContinueGame();

		if (Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevel == 2 && startInterstitialOpened)
		{
			startInterstitialOpened = false;
			ClosePopUpMenu(disabledObjects[2]);
		}
	}

	public void OpenLevelScene(string levelName)
	{
		Application.LoadLevel(levelName);
	}

	// Play button click sound
	public void PlayButtonClickSound()
	{
		SoundManager.PlaySound("ButtonClick");
	}

	public void LoadSceneWithLoading(string sceneName)
	{
		StartCoroutine(LoadSceneWithLoadingCoroutine(sceneName));
	}

	IEnumerator LoadSceneWithLoadingCoroutine(string sceneName)
	{
		if (loadingHolder != null)
		{
			loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;
			// Play loading scene arrive animation
			loadingHolder.transform.GetChild(0).GetComponent<Animator>().Play("LoadingArriving", 0, 0);
			yield return new WaitForSeconds (2f);
			Application.LoadLevel(sceneName);
		}
	}

	IEnumerator StartSceneWithLoadingHolderCoroutine()
	{
		yield return null;
	}

	public void HomeButtonPressed()
	{
//		GlobalVariables.playLoadingDepart = true;

		StartCoroutine(LoadSceneWithLoadingCoroutine("MainScene"));
	}

	public void NextPatientPressed()
	{
		StartCoroutine(LoadSceneWithLoadingCoroutine("CharacterSelect"));
	}

	public void PlayButtonPressed()
	{
		loadingHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;
		Application.LoadLevel("Level");
	}

	public void AtemptToBuyInapp(string sku) {}
	public void BuyInappTestis() {}
}