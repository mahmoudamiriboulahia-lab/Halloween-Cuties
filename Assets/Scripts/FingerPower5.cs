using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class FingerPower5 : MonoBehaviour {

	private List<GameObject> listOfBombs;
	// Create listo of all objects that clicking is needed
	private List<GameObject> listOfObjectsThatNeedCallingClickOn;

	public bool pierce;

	private List<GameObject> listOfHits;

	void Awake()
	{
		listOfBombs = new List<GameObject>();
		listOfObjectsThatNeedCallingClickOn = new List<GameObject>();
		listOfHits = new List<GameObject>();
	}

	public void Click(Vector3 clickPosition)
	{
		if (!pierce)
		{
			RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(clickPosition), Vector3.zero); // Ovde je bio inout.mouseposition umesto click positiona
			if (hit.collider != null)
			{
				hit.collider.enabled = false;

				if (hit.collider.transform.parent.tag == "Baloon")
				{
					if (hit.collider.tag == "PointBaloon")
					{
						switch (hit.collider.gameObject.transform.parent.GetComponent<Baloon>().baloonType)
						{
						case 0:
							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("BaloonPop");

							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.gameObject));
							break;
						case 1:
							hit.collider.gameObject.transform.parent.SetParent(LevelManager.levelManager.toysHolder);
	//						hit.collider.transform.parent.localPosition = new Vector3(hit.collider.transform.parent.localPosition.x, hit.collider.transform.parent.localPosition.y, 0);

							if (hit.collider.transform.parent.GetChild(1).GetComponent<Animator>() == null)
								hit.collider.transform.parent.GetComponent<Baloon>().movingSpeed = LevelManager.levelManager.fallingSpeed;
							else
							{
								hit.collider.transform.parent.GetComponent<Baloon>().movingSpeed = 0;
								hit.collider.transform.parent.GetChild(1).GetComponent<Animator>().Play("FallingObjectActive", 0, 0);
							}

							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("BaloonPop");

							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.GetChild(0).gameObject));
							break;
						case 2:
							hit.collider.transform.parent.GetChild(0).GetChild(0).GetComponent<Animator>().Play("BaloonsSpread");
							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("BaloonPop");

							StartCoroutine(ChangeParentForInnerBallonsToBaloonsHolder(hit.collider.transform.parent.gameObject));
							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject));
							break;
						case 3:
							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("PlusSec");

							LevelManager.levelManager.timer.AddTime(LevelManager.levelManager.addTimeAmount);
							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.gameObject));
							break;
						case 4:
							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("MinusSec");

							LevelManager.levelManager.timer.AddTime(-LevelManager.levelManager.substractTimeAmount);
							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.gameObject));
							break;
						case 5:
							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("BombSound");

							// Add first bomb to the list of bombs
							listOfBombs.Add(hit.collider.transform.parent.gameObject);

							// Find all bombs in radius and add them to list of bombs
							foreach (Transform b in LevelManager.levelManager.baloonsHolder)
							{
								if (Vector2.Distance(hit.collider.transform.parent.localPosition, b.localPosition) < 500f && b.gameObject != hit.collider.transform.parent.gameObject && b.GetComponent<Baloon>().baloonType == 5)
								{
									listOfBombs.Add(b.gameObject);
								}
							}

							// Go through all bombs, and add all elements that are not bombs to items that need clicking on
							for (int i = 0; i < listOfBombs.Count; i++)
							{
								foreach (Transform b in LevelManager.levelManager.baloonsHolder)
								{
									if (Vector2.Distance(listOfBombs[i].transform.localPosition, b.localPosition) < 500f && !listOfObjectsThatNeedCallingClickOn.Contains(b.gameObject) && !listOfBombs.Contains(b.gameObject))
										listOfObjectsThatNeedCallingClickOn.Add(b.gameObject);
								}

								foreach (Transform b in LevelManager.levelManager.toysHolder)
								{
									if (Vector2.Distance(listOfBombs[i].transform.localPosition, b.localPosition) < 500f && !listOfObjectsThatNeedCallingClickOn.Contains(b.gameObject) && !listOfBombs.Contains(b.gameObject))
										listOfObjectsThatNeedCallingClickOn.Add(b.gameObject);
								}
							}

							// Click on every item from the list
							for (int i = 0; i < listOfObjectsThatNeedCallingClickOn.Count; i++)
							{
								LevelManager.levelManager.PlayParticleOnPosition(listOfObjectsThatNeedCallingClickOn[i].transform.localPosition);
								ClickOnBaloonWithoutRaycast(listOfObjectsThatNeedCallingClickOn[i]);
							}

							// Destroy all bombs
							for (int i = 0; i < listOfBombs.Count; i++)
							{
								LevelManager.levelManager.PlayParticleOnPosition(listOfBombs[i].transform.localPosition);
								Destroy(listOfBombs[i]);
							}

							// Remove all objects from lists
							listOfBombs.Clear();
							listOfObjectsThatNeedCallingClickOn.Clear();

							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.gameObject));
							break;
						case 6:
							LevelManager.doublePointsActive = true;
							LevelManager.levelManager.ScorePoint(1);
							LevelManager.levelManager.doublePointsStartTime = Time.time;
							LevelManager.levelManager.pointsMultiplier = 2;
							LevelManager.levelManager.doubleCoinsIndicator.SetActive(true);
							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.gameObject));
							break;
						default:
							break;
						}

						LevelManager.levelManager.PlayParticleOnPosition(hit.collider.transform.parent.localPosition);
					}
					else if (hit.collider.tag == "PointToy")
					{
						LevelManager.levelManager.ScorePoint(3);
						Destroy(hit.collider.gameObject.transform.parent.gameObject);
					}
				}
				else if (hit.collider.transform.parent.tag == "InnerBaloon")
				{
					if (hit.collider.tag == "PointBaloon")
					{
						switch (hit.collider.gameObject.transform.parent.GetComponent<Baloon>().baloonType)
						{
						case 0:
							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("BaloonPop");

							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.gameObject));
							break;
						case 1:
							hit.collider.gameObject.transform.parent.SetParent(LevelManager.levelManager.toysHolder);
							hit.collider.transform.parent.localPosition = new Vector3(hit.collider.transform.parent.localPosition.x, hit.collider.transform.parent.localPosition.y, 0);

							if (hit.collider.transform.parent.GetChild(0).GetComponent<Animator>() == null)
								hit.collider.transform.parent.GetComponent<Baloon>().movingSpeed = LevelManager.levelManager.fallingSpeed;
							else
							{
								hit.collider.transform.parent.GetComponent<Baloon>().movingSpeed = 0;
								hit.collider.transform.parent.GetChild(0).GetComponent<Animator>().Play("FallingObjectActive", 0, 0);
							}

							LevelManager.levelManager.ScorePoint(1);

							// Play baloon pop sound
							SoundManager.PlaySound("BaloonPop");

							StartCoroutine(DestroyObjectAfterDelay(hit.collider.gameObject.transform.parent.GetChild(1).gameObject));
							break;
						default:
							break;
						}

						LevelManager.levelManager.PlayParticleOnPosition(hit.collider.transform.parent.localPosition);
					}
					else if (hit.collider.tag == "PointToy")
					{
						LevelManager.levelManager.ScorePoint(3);
						Destroy(hit.collider.gameObject.transform.parent.gameObject);
					}
				}
			}
		} /* Pierce */ 
		else
		{
			RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(clickPosition), Vector3.zero);
			if (hit.Length > 0 && hit[0].collider.tag != "Cloud")
			{
				for (int j = 0; j < hit.Length; j++)
				{
					if (hit[j].collider.transform.parent.tag == "Baloon")
					{
						if (hit[j].collider.tag == "PointToy")
						{
							if (hit[j].collider.transform.parent.childCount == 2)
							{
								hit[j].collider.enabled = false;
								LevelManager.levelManager.ScorePoint(3);

								// Play baloon pop sound
								SoundManager.PlaySound("ToyPop");

								// FIXME pitanje da li je dobra poyicija!!!
								if (hit[j].collider.GetComponent<Animator>() != null)
								{
									LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition + hit[j].collider.transform.localPosition);

									LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition + hit[j].collider.transform.localPosition, 3);
								}
								else
								{
									LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition);

									LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 3);
								}

								Destroy(hit[j].collider.gameObject.transform.parent.gameObject);
							}
						}
						else if (hit[j].collider.tag == "PointBaloon")
						{
							hit[j].collider.enabled = false;

							switch (hit[j].collider.gameObject.transform.parent.GetComponent<Baloon>().baloonType)
							{
							case 0:
								LevelManager.levelManager.ScorePoint(1);

								// Play baloon pop sound
								SoundManager.PlaySound("BaloonPop");

								LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 1);

								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.gameObject));
								break;
							case 1:
								hit[j].collider.gameObject.transform.parent.SetParent(LevelManager.levelManager.toysHolder);
								//	hit.collider.transform.parent.localPosition = new Vector3(hit.collider.transform.parent.localPosition.x, hit.collider.transform.parent.localPosition.y, 0);

								if (hit[j].collider.transform.parent.childCount > 2 && hit[j].collider.transform.parent.GetChild(2).GetComponent<Animator>() == null)
								{
									hit[j].collider.transform.parent.GetComponent<Baloon>().movingSpeed = LevelManager.levelManager.fallingSpeed;
									hit[j].collider.transform.parent.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
								}
								else
								{
									hit[j].collider.transform.parent.GetComponent<Baloon>().movingSpeed = 0;
									hit[j].collider.transform.parent.GetChild(2).GetComponent<Animator>().Play("FallingObjectActive", 0, 0);
								}

								LevelManager.levelManager.ScorePoint(1);

								LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 1);

								// Play baloon pop sound
								SoundManager.PlaySound("BaloonPop");

								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.GetChild(0).gameObject));
								break;
							case 2:
								hit[j].collider.transform.parent.GetChild(1).GetChild(0).GetComponent<Animator>().Play("BaloonsSpread");
								LevelManager.levelManager.ScorePoint(1);

								LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 1);

								// Play baloon pop sound
								SoundManager.PlaySound("BaloonPop");

								StartCoroutine(ChangeParentForInnerBallonsToBaloonsHolder(hit[j].collider.transform.parent.gameObject));
								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject));
								break;
							case 3:
//								LevelManager.levelManager.ScorePoint(1);

								// Play baloon pop sound
								SoundManager.PlaySound("PlusSec");

								LevelManager.levelManager.PlayAddTimeOnPosition(hit[j].collider.transform.parent.localPosition);

								LevelManager.levelManager.timer.AddTime(LevelManager.levelManager.addTimeAmount);
								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.gameObject));
								break;
							case 4:
//								LevelManager.levelManager.ScorePoint(1);

								// Play baloon pop sound
								SoundManager.PlaySound("MinusSec");

								LevelManager.levelManager.PlayLoseTimeOnPosition(hit[j].collider.transform.parent.localPosition);

								LevelManager.levelManager.timer.AddTime(-LevelManager.levelManager.substractTimeAmount);
								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.gameObject));
								break;
							case 5:
//								LevelManager.levelManager.ScorePoint(1);

								// Play baloon pop sound
								SoundManager.PlaySound("BombSound");

								LevelManager.levelManager.gamePanel.GetComponent<Animator>().Play("Bomb", 0, 0);

								// Add first bomb to the list of bombs
								listOfBombs.Add(hit[j].collider.transform.parent.gameObject);

								// Find all bombs in radius and add them to list of bombs
								foreach (Transform b in LevelManager.levelManager.baloonsHolder)
								{
									if (Vector2.Distance(hit[j].collider.transform.parent.localPosition, b.localPosition) < 500f && b.gameObject != hit[j].collider.transform.parent.gameObject && b.GetComponent<Baloon>().baloonType == 5)
									{
										listOfBombs.Add(b.gameObject);
									}
								}

								// Go through all bombs, and add all elements that are not bombs to items that need clicking on
								for (int i = 0; i < listOfBombs.Count; i++)
								{
									foreach (Transform b in LevelManager.levelManager.baloonsHolder)
									{
										if (Vector2.Distance(listOfBombs[i].transform.localPosition, b.localPosition) < 500f && !listOfObjectsThatNeedCallingClickOn.Contains(b.gameObject) && !listOfBombs.Contains(b.gameObject))
											listOfObjectsThatNeedCallingClickOn.Add(b.gameObject);
									}

									foreach (Transform b in LevelManager.levelManager.toysHolder)
									{
										if (Vector2.Distance(listOfBombs[i].transform.localPosition, b.localPosition) < 500f && !listOfObjectsThatNeedCallingClickOn.Contains(b.gameObject) && !listOfBombs.Contains(b.gameObject))
											listOfObjectsThatNeedCallingClickOn.Add(b.gameObject);
									}
								}

								// Click on every item from the list
								for (int i = 0; i < listOfObjectsThatNeedCallingClickOn.Count; i++)
								{
									LevelManager.levelManager.PlayParticleOnPosition(listOfObjectsThatNeedCallingClickOn[i].transform.localPosition);
									ClickOnBaloonWithoutRaycast(listOfObjectsThatNeedCallingClickOn[i]);
								}

								// Destroy all bombs
								for (int i = 0; i < listOfBombs.Count; i++)
								{
									LevelManager.levelManager.PlayBombParticleOnPosition(listOfBombs[i].transform.localPosition);
									Destroy(listOfBombs[i]);
								}

								// Remove all objects from lists
								listOfBombs.Clear();
								listOfObjectsThatNeedCallingClickOn.Clear();

								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.gameObject));
								break;
							case 6:
								LevelManager.doublePointsActive = true;
								LevelManager.levelManager.ScorePoint(1);
								LevelManager.levelManager.doublePointsStartTime = Time.time;
								LevelManager.levelManager.pointsMultiplier = 2;
								LevelManager.levelManager.doubleCoinsIndicator.SetActive(true);
								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.gameObject));
								break;
							default:
								break;
							}

							LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition);
						}
					}
					else if (hit[j].collider.transform.parent.tag == "InnerBaloon" && hit[j].collider.transform.parent.parent.name != "MoreBaloons")
					{
						if (hit[j].collider.tag == "PointToy")
						{
							if (hit[j].collider.transform.parent.childCount == 2)
							{
								LevelManager.levelManager.ScorePoint(3);

//								LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 3);

								if (hit[j].collider.GetComponent<Animator>() != null)
								{
//									LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition);

									LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition + hit[j].collider.transform.localPosition, 3);
									LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition + hit[j].collider.transform.localPosition);
								}
								else
								{
//									LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.localPosition);

									LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 3);
									LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition);
								}

								// Play baloon pop sound
								SoundManager.PlaySound("ToyPop");

								Destroy(hit[j].collider.gameObject.transform.parent.gameObject);
							}
						}
						else if (hit[j].collider.tag == "PointBaloon")
						{
							switch (hit[j].collider.gameObject.transform.parent.GetComponent<Baloon>().baloonType)
							{
							case 0:
								LevelManager.levelManager.ScorePoint(1);

								LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 1);

								// Play baloon pop sound
								SoundManager.PlaySound("BaloonPop");

								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.gameObject));
								break;
							case 1:
								hit[j].collider.gameObject.transform.parent.SetParent(LevelManager.levelManager.toysHolder);
								hit[j].collider.transform.parent.localPosition = new Vector3(hit[j].collider.transform.parent.localPosition.x, hit[j].collider.transform.parent.localPosition.y, 0);

								if (hit[j].collider.transform.parent.childCount > 2 && hit[j].collider.transform.parent.GetChild(2).GetComponent<Animator>() == null)
								{
									hit[j].collider.transform.parent.GetComponent<Baloon>().movingSpeed = LevelManager.levelManager.fallingSpeed;
									hit[j].collider.transform.parent.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
								}
								else
								{
									hit[j].collider.transform.parent.GetComponent<Baloon>().movingSpeed = 0;
									hit[j].collider.transform.parent.GetChild(2).GetComponent<Animator>().Play("FallingObjectActive", 0, 0);
								}

								LevelManager.levelManager.ScorePoint(1);

								LevelManager.levelManager.PlayAddPointOnPosition(hit[j].collider.transform.parent.localPosition, 1);

								// Play baloon pop sound
								SoundManager.PlaySound("BaloonPop");

								StartCoroutine(DestroyObjectAfterDelay(hit[j].collider.gameObject.transform.parent.GetChild(0).gameObject));
								break;
							default:
								break;
							}

							LevelManager.levelManager.PlayParticleOnPosition(hit[j].collider.transform.parent.localPosition);
						}
					}
				}
			}
		}
	}

	public void ClickOnBaloonWithoutRaycast(GameObject baloon)
	{
		switch(baloon.GetComponent<Baloon>().baloonType)
		{
		case 0:
			LevelManager.levelManager.ScorePoint(1);

			LevelManager.levelManager.PlayAddPointOnPosition(baloon.transform.localPosition, 1);

			StartCoroutine(DestroyObjectAfterDelay(baloon));
			break;
		case 1:
			baloon.transform.SetParent(LevelManager.levelManager.toysHolder);
//			baloon.transform.localPosition = new Vector3(hit.collider.transform.parent.localPosition.x, hit.collider.transform.parent.localPosition.y, 0);

			if (baloon.transform.childCount > 2)
			{
				if (baloon.transform.GetChild(2).GetComponent<Animator>() == null)
				{
					baloon.transform.GetComponent<Baloon>().movingSpeed = LevelManager.levelManager.fallingSpeed;
					baloon.transform.GetChild(2).GetChild(0).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
				}
				else
				{
					baloon.transform.GetComponent<Baloon>().movingSpeed = 0;
					baloon.transform.GetChild(2).GetComponent<Animator>().Play("FallingObjectActive", 0, 0);
				}
			}

			// FIXME ovde proveriti... izgleda da kad unistava toy element zeza jer nemachild od 1 (vec ga je unistio...)
			if (baloon.transform.childCount > 2)
			{
				Debug.Log("Samo balon kod igrackice");
				LevelManager.levelManager.ScorePoint(1);

				LevelManager.levelManager.PlayAddPointOnPosition(baloon.transform.localPosition, 1);

				StartCoroutine(DestroyObjectAfterDelay(baloon.transform.GetChild(0).gameObject));
			}
			else
			{
				Debug.Log("Igrackica");

				LevelManager.levelManager.ScorePoint(3);

				LevelManager.levelManager.PlayAddPointOnPosition(baloon.transform.localPosition, 3);

				StartCoroutine(DestroyObjectAfterDelay(baloon));
			}
			break;
		case 2:
			baloon.transform.GetChild(1).GetChild(0).GetComponent<Animator>().Play("BaloonsSpread");
			LevelManager.levelManager.ScorePoint(1);
			LevelManager.levelManager.PlayAddPointOnPosition(baloon.transform.localPosition, 1);
			StartCoroutine(ChangeParentForInnerBallonsToBaloonsHolder(baloon));

			if (baloon.transform.childCount > 1)
				StartCoroutine(DestroyObjectAfterDelay(baloon.transform.GetChild(1).gameObject));
			break;
		case 3:
//			LevelManager.levelManager.ScorePoint(1);
			LevelManager.levelManager.PlayAddTimeOnPosition(baloon.transform.localPosition);
			LevelManager.levelManager.timer.AddTime(LevelManager.levelManager.addTimeAmount);
			StartCoroutine(DestroyObjectAfterDelay(baloon));
			break;
		case 4:
//			LevelManager.levelManager.ScorePoint(1);
			LevelManager.levelManager.PlayLoseTimeOnPosition(baloon.transform.localPosition);
			LevelManager.levelManager.timer.AddTime(-LevelManager.levelManager.substractTimeAmount);
			StartCoroutine(DestroyObjectAfterDelay(baloon));
			break;
		case 6:
			LevelManager.doublePointsActive = true;
			LevelManager.levelManager.ScorePoint(1);
			LevelManager.levelManager.doublePointsStartTime = Time.time;
			LevelManager.levelManager.pointsMultiplier = 2;

			if (!LevelManager.levelManager.doubleCoinsIndicator.activeInHierarchy)
				LevelManager.levelManager.doubleCoinsIndicator.SetActive(true);
			
			StartCoroutine(DestroyObjectAfterDelay(baloon));
			break;
		default:
			break;
		}
	}

	IEnumerator CallDestroyBaloonsWithBombAfterDelay(GameObject bombBaloon)
	{
		yield return new WaitForSeconds (0.05f);

		foreach (Transform t in LevelManager.levelManager.toysHolder)
		{
			if (Vector2.Distance(bombBaloon.transform.localPosition, t.localPosition) < 500f)
			{
				ClickOnBaloonWithoutRaycast(t.gameObject);
			}
		}

		foreach (Transform b in LevelManager.levelManager.baloonsHolder)
		{
			if (Vector2.Distance(bombBaloon.transform.localPosition, b.localPosition) < 500f)
			{
				ClickOnBaloonWithoutRaycast(b.gameObject);
			}
		}
	}

	IEnumerator ChangeParentForInnerBallonsToBaloonsHolder(GameObject baloon)
	{
		yield return new WaitForSeconds(0.17f);

		List<GameObject> listOfBaloons = new List<GameObject>();
		List<GameObject> objectsThatNeedDestruction = new List<GameObject>();

		if (baloon != null)
		{
			if (baloon.transform.GetChild(0).childCount > 0 && baloon.transform.GetChild(0).GetChild(0).GetComponent<Animator>() != null)
			{
				baloon.transform.GetChild(0).GetChild(0).GetComponent<Animator>().StopPlayback();
				baloon.transform.GetChild(0).GetChild(0).GetComponent<Animator>().enabled = false;
			}

			foreach (Transform b in baloon.transform.GetChild(0).GetChild(0))
			{
				// FIXME ovo je privremeno resenje
				if (b.gameObject.GetComponent<Baloon>() != null)
					listOfBaloons.Add(b.gameObject);
				else
					objectsThatNeedDestruction.Add(b.gameObject);
			}

			for (int i = 0; i < listOfBaloons.Count; i++)
			{
				listOfBaloons[i].transform.SetParent(LevelManager.levelManager.baloonsHolder);

				listOfBaloons[i].transform.GetComponent<Baloon>().movingSpeed = Random.Range(LevelManager.levelManager.minBaloonSpeed, LevelManager.levelManager.maxBaloonSpeed);
			}

			for (int i = 0; i < objectsThatNeedDestruction.Count; i++)
				Destroy(objectsThatNeedDestruction[0]);

			Destroy(baloon);
		}
	}

	IEnumerator DestroyObjectAfterDelay(GameObject go)
	{
		yield return new WaitForSeconds (0.1f);
		Destroy(go);
	}

	public void ClearAdditionalLists()
	{
		listOfBombs.Clear();
		listOfObjectsThatNeedCallingClickOn.Clear();
	}

	void Update()
	{
		if ((Input.touchCount > 5 && Input.touches[5].phase == TouchPhase.Began) && !LevelManager.gamePaused && !LevelManager.gameOver && EventSystem.current.currentSelectedGameObject == null)
		{
			Click(Input.touches[5].position);
		}

		// Check if double points are finished so we can change multiplier to 1
		if (LevelManager.doublePointsActive && LevelManager.levelManager.doublePointsStartTime + LevelManager.levelManager.doublePointsDuration < Time.time)
		{
			LevelManager.doublePointsActive = false;
			LevelManager.levelManager.pointsMultiplier = 1;
			LevelManager.levelManager.doubleCoinsIndicator.SetActive(false);
		}
	}
}
