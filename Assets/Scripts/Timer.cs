using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// Scene: MiniGame_3
/// Object:N/A
/// Description: Used for counting down time in pop the cat min game
/// </summary>

public class Timer : MonoBehaviour {

	public Text timer;
	public float minutes = 0;
	public float seconds = 20;
	float miliseconds = 0;

	string secondsString;
	string milisecondsString;

	public bool gameoverCalled;

	public bool gamePaused;

	public bool videoAvailableChecked;

	public static Timer instance;

	void Awake()
	{
		timer = GetComponent<Text>();

		videoAvailableChecked = false;

		gameoverCalled = false;

		gamePaused = false;

		instance = this;
	}

	public void AddTime(float sec)
	{
		seconds += sec;

		if (seconds >= 60)
		{
			minutes += Mathf.Round(seconds / 60f);
			seconds = seconds % 60;
		}
		else if (seconds <= 0)
		{
			minutes = 0;
			seconds = 0;
		}
	}

	void Update(){

		if (!gamePaused)
		{
			if(miliseconds <= 0){
				if(seconds <= 0){
					minutes--;
					seconds = 59;
				}
				else if(seconds >= 0){
					seconds--;
				}
				
				miliseconds = 100;
			}
			
			miliseconds -= Time.deltaTime * 100;

			if (seconds < 10)
				secondsString = "0" + seconds.ToString();
			else
				secondsString = seconds.ToString();

			// If seconds less than 5 colour lettrers in red, else white
			if (seconds < 5 && minutes < 1)
			{
				Color c = GetComponent<Text>().color;

				c.r = 1f;
				c.g = 0;
				c.b = 0;

				GetComponent<Text>().color = c;

			

				// If beeping is not playing play
//				if (!SoundManager.Instance.beepSound.isPlaying)
//					SoundManager.Instance.Play_BeepSound();
			}
			else
			{
				Color c = GetComponent<Text>().color;
				
				c.r = 1f;
				c.g = 1f;
				c.b = 1f;
				
				GetComponent<Text>().color = c;

//				SoundManager.Instance.Stop_BeepSound();
			}

			if (miliseconds < 10)
			{
				int ms;
				ms = (int)miliseconds;
				milisecondsString = "0" + ms.ToString();
			}
			else
			{
				int ms;
				ms = (int)miliseconds;
				milisecondsString = ms.ToString();
			}

			if (minutes >= 0)
			{
				timer.text = minutes.ToString() + ":" + secondsString + ":" + milisecondsString;
			}
			else
			{
				timer.text = "0:00";

				if (!gameoverCalled)
				{
					LevelManager.gameOver = true;

					Time.timeScale = 0;

					// Show gameover popup
					GameObject.Find("Canvas").GetComponent<MenuManager>().ShowPopUpMenu(LevelManager.levelManager.gameOverPopup);


					gameoverCalled = true;

					// Check if its highscore
					if (LevelManager.levelManager.pointsScored > PlayerPrefs.GetInt("ScoreOfTheHighes"))
					{
						// Set highscore in prefs and send to play services
						PlayerPrefs.SetInt("ScoreOfTheHighes", LevelManager.levelManager.pointsScored);

						//GlobalVariables.globalVariables.PostScoreToLeaderboard(LevelManager.levelManager.pointsScored);
					}
						

					// Play won sound
//					SoundManager.Instance.Play_WonSound();

//					UndressingGameManager.instance.GameOver();
				}
			}
		}
	}
}
