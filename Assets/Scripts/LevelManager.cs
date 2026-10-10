using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class LevelManager : MonoBehaviour {

	// Spawning time
	public float minBaloonSpawnTime;
	public float maxBaloonSpewnTime;

	// Baloon speed
	public float minBaloonSpeed;
	public float maxBaloonSpeed;
	public float fallingSpeed;

	public int pointsScored;
	public Text pointsTextHolder;

	public GameObject baloonPrefab;

	public Transform baloonsHolder;
	public Transform toysHolder;

	public static bool gamePaused;

	public GameObject[] baloonParticlesPool;
	public GameObject[] bombParticlesPool;
	public GameObject[] addTimeAnimationsPool;
	public GameObject[] loseTimeAnimationsPool;
	public GameObject[] addPointsAnimationsPool;

	public GameObject[] listOfToys;

	public Sprite addTimeSprite;
	public Sprite loseTimeSprite;
	public Sprite bombSprite;
	public Sprite doublePointsSprite;

	public float addTimeAmount;
	public float substractTimeAmount;

	public float gameTimeOrig;
	public float gameTime;
	public Timer timer;

	public static bool gameOver;

	public GameObject gameOverPopup;

	// Powerup variable
	public int pointsMultiplier;
	public float doublePointsDuration;
	public float doublePointsStartTime;
	public static bool doublePointsActive;

	// Powerup indicators
	public GameObject doubleCoinsIndicator;

	// Game panel - for animations
	public GameObject gamePanel;

	// Baloon images
	public Sprite[] baloonImages;

	// Level interface elements
	public GameObject pauseButtonHolder;
	public GameObject timerHolder;
	public GameObject pointsHolder;
	public GameObject cloudBlocker;

	public GameObject watchVideoPanelHolder;
	public static bool alreadyContinuedWithVideo;
	public static bool videoReady;

	public static LevelManager levelManager;

	public IEnumerator SpawnBaloon()
	{
		float spawningTime = UnityEngine.Random.Range(minBaloonSpawnTime, maxBaloonSpewnTime);

		yield return new WaitForSeconds(spawningTime);

		float xPos = UnityEngine.Random.Range(-3f, 3f);

		GameObject baloon = Instantiate(baloonPrefab, new Vector3(xPos, -6f, 90), baloonPrefab.transform.rotation) as GameObject;
		baloon.transform.SetParent(baloonsHolder);
		baloon.transform.localScale = Vector3.one;
		baloon.transform.SetAsFirstSibling();

		StartCoroutine("SpawnBaloon");
	}

	public void ScorePoint(int points)
	{
		pointsScored += points * pointsMultiplier;
		pointsTextHolder.text = pointsScored.ToString();
	}

	public void PauseGame()
	{
		if (!gamePaused && !gameOver && !GameObject.Find("Canvas").GetComponent<MenuManager>().popupOpened)
		{
			//GameObject.Find("Canvas").GetComponent<MenuManager>().ShowPopUpMenu(pausePopup);
			Time.timeScale = 0;
			gamePaused = true;
			timer.gamePaused = true;
		}
	}

	void OnApplicationPause(bool paused)
	{
		if (paused)
			PauseGame();
	}

	public void ContinueGame()
	{
		StartCoroutine(UnpauseGameCoroutine());
	}

	IEnumerator UnpauseGameCoroutine()
	{
		//GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(pausePopup);

		Time.timeScale = 1;

		yield return new WaitForSeconds(0.1f);

		if (gamePaused && !gameOver)
		{
			gamePaused = false;
			timer.gamePaused = false;
		}
	}

	public void PlayParticleOnPosition(Vector3 position)
	{
		for (int i = 0; i < baloonParticlesPool.Length; i++)
		{
			if (!baloonParticlesPool[i].GetComponent<BaloonParticle>().active)
			{
				baloonParticlesPool[i].transform.localPosition = position;
				baloonParticlesPool[i].GetComponent<BaloonParticle>().PlayBaloonParticle();
				break;
			}
		}
	}

	public void PlayAddTimeOnPosition(Vector3 position)
	{
		for (int i = 0; i < addTimeAnimationsPool.Length; i++)
		{
			if (!addTimeAnimationsPool[i].GetComponent<BaloonParticle>().active)
			{
				addTimeAnimationsPool[i].transform.localPosition = position;
				addTimeAnimationsPool[i].GetComponent<BaloonParticle>().PlayAddTimeAnimation();
				break;
			}
		}
	}

	public void PlayLoseTimeOnPosition(Vector3 position)
	{
		for (int i = 0; i < loseTimeAnimationsPool.Length; i++)
		{
			if (!loseTimeAnimationsPool[i].GetComponent<BaloonParticle>().active)
			{
				loseTimeAnimationsPool[i].transform.localPosition = position;
				loseTimeAnimationsPool[i].GetComponent<BaloonParticle>().PlayLoseTimeAnimation();
				break;
			}
		}
	}

	public void PlayAddPointOnPosition(Vector3 position, int points)
	{
		for (int i = 0; i < addPointsAnimationsPool.Length; i++)
		{
			if (!addPointsAnimationsPool[i].GetComponent<BaloonParticle>().active)
			{
				addPointsAnimationsPool[i].transform.localPosition = position;
				addPointsAnimationsPool[i].GetComponent<BaloonParticle>().PlayAddPointAnimation(points);
				break;
			}
		}
	}

	public void PlayBombParticleOnPosition(Vector3 position)
	{
		for (int i = 0; i < bombParticlesPool.Length; i++)
		{
			if (!bombParticlesPool[i].GetComponent<BaloonParticle>().active)
			{
				bombParticlesPool[i].transform.localPosition = position;
				bombParticlesPool[i].GetComponent<BaloonParticle>().PlayBaloonParticle();
				break;
			}
		}
	}

	void Awake()
	{
		pointsScored = 0;
		pointsMultiplier = 1;
		fallingSpeed = -fallingSpeed;
		timer.minutes = Mathf.Round(gameTime / 60);
		timer.seconds = gameTime % 60;

		doublePointsActive = false;
		doublePointsStartTime = -doublePointsDuration - 1f;

		gamePaused = false;

		gameOver = false;

		alreadyContinuedWithVideo = false;


		levelManager = this;
	}

	void Start()
	{
		 StartCoroutine("SpawnBaloon");
	}

	public void RestartGame()
	{
		 if (gameOverPopup.activeInHierarchy)
			GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(gameOverPopup);

		StartCoroutine(RestartGameCoroutine());
	}

	IEnumerator RestartGameCoroutine()
	{
		Time.timeScale = 1f;

		// Stop spawning baloons
		StopCoroutine("SpawnBaloon");
		//		StopAllCoroutines();

		// Clear lists for fingerpower elements
		Camera.main.GetComponent<FingerPower>().ClearAdditionalLists();

		// Destroy all gameobjects
		foreach (Transform t in baloonsHolder.transform)
		{
			Destroy(t.gameObject);
		}

		foreach (Transform t in toysHolder.transform)
		{
			Destroy(t.gameObject);
		}

		yield return new WaitForSeconds(0.1f);

        Debug.Log("Trebalo bi da se pozove interstitial");
   

		yield return new WaitForSeconds(0.5f);

		gameTime = gameTimeOrig;

		gamePaused = false;
		timer.gamePaused = false;
	

		pointsScored = 0;
		pointsTextHolder.text = pointsScored.ToString();
		timer.minutes = Mathf.Round(gameTime / 60);
		timer.seconds = gameTime % 60;
		timer.gameoverCalled = false;
		timer.gamePaused = false;

		gameOver = false;

		alreadyContinuedWithVideo = false;

		GameObject.Find("TimerText").GetComponent<Timer>().videoAvailableChecked = false;

		StartCoroutine("SpawnBaloon");
	}

	public void HomeButtonPressed()
	{
		// Set always scale to 1 so if game is paused it will be unpaused
//		if (gamePaused)
		Time.timeScale = 1f;

	
		Application.LoadLevel("SelectGame");
	}

	

	public void ContinueGameAfterVideoIsWatched()
	{
		StartCoroutine("ContinueCoroutine");
	}

	IEnumerator ContinueCoroutine()
	{
		// Close popup menu
		GameObject.Find("Canvas").GetComponent<MenuManager>().ClosePopUpMenu(gameOverPopup);
		Time.timeScale = 1f;

		alreadyContinuedWithVideo = true;

		yield return new WaitForSeconds(0.4f);

		// Hide watch video panel
		watchVideoPanelHolder.SetActive(false);

		// Comtinue game
		gameOver = false;
		gamePaused = false;
		gameTime = 20f;
		timer.minutes = Mathf.Round(gameTime / 60);
		timer.seconds = gameTime % 60;
		timer.gameoverCalled = false;
		timer.gamePaused = false;

		StartCoroutine("SpawnBaloon");


	}
}
