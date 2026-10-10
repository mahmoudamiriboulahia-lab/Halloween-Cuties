using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class Baloon : MonoBehaviour {

	public GameObject toyObjectHolder; // Toy inside of baloon
	public float movingSpeed; // Speed that baloon is moving upwards

	public GameObject particleHolder;

	/// 0 - regular baloon, no inner object
	/// 1 - regular baloon, one inner object
	/// 2 - when this baloon pops it creates more baloons
	/// 3 - baloon that adds time
	/// 4 - baloon that substract time
	/// 5 - bomb baloon, destroyes baloons in radius
	/// 6 - double points baloon

	public int baloonType;

	void Awake()
	{
		// Set baloon type FIXME za sada stavljam da bude to random vrednost
		if (tag == "Baloon")
		{
//			baloonType = Random.Range(0, 6);
//
//			switch(baloonType)
//			{
//			case 0:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//				break;
//			case 1:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				// Create toy object
//				int r = Random.Range(0, LevelManager.levelManager.listOfToys.Length);
//
//				GameObject toy = Instantiate(LevelManager.levelManager.listOfToys[r], transform.localPosition, LevelManager.levelManager.listOfToys[r].transform.rotation) as GameObject;
//				toy.transform.SetParent(transform);
//				toy.transform.SetAsFirstSibling();
//				toy.transform.localScale = Vector3.one;
//				toy.transform.localPosition = Vector3.zero;
//				break;
//			case 2:
//				// Destroying uneccessary objects
//				break;
//			case 3:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				GetComponent<Image>().sprite = LevelManager.levelManager.addTimeSprite;
//				GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//				break;
//			case 4:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				GetComponent<Image>().sprite = LevelManager.levelManager.loseTimeSprite;
//				GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//				break;
//			case 5:
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				GetComponent<Image>().sprite = LevelManager.levelManager.bombSprite;
//				GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//				break;
//			default:
//				break;
//			}

			int val = Random.Range(0, 60);

			if (val == 17)
			{
				// Set baloon type
				baloonType = 5; // bomb

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Set BaloonTypeImage
				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.bombSprite;
				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			else if (val == 27)
			{
				// Set baloon type
				baloonType = 3; // add time

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Set BaloonTypeImage
				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.addTimeSprite;
				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			// FIXME Double points
//			else if (val == 37)
//			{
//				// Set baloon type
//				baloonType = 6; // double coins
//
//				// Destroying uneccessary objects
//				Destroy(transform.GetChild(0).gameObject);
//
//				// Set BaloonTypeImage
//				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.doublePointsSprite;
//				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
//			}
			else if (val == 4 || val == 25 || val == 43)
			{
				// Set baloon type
				baloonType = 4; // substract time

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Set BaloonTypeImage
				transform.GetChild(2).GetComponent<Image>().sprite = LevelManager.levelManager.loseTimeSprite;
				transform.GetChild(2).GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			}
			else if (val == 5 || val == 28 || val == 41)
			{
				// Set baloon type
				baloonType = 2; // baloons inside this baloon
			}
			else if (val > 34 && val % 2 == 0)
			{
				// Set baloon type
				baloonType = 1; // toy inside this baloon

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);

				// Create toy object
				int r = Random.Range(0, LevelManager.levelManager.listOfToys.Length);

				GameObject toy = Instantiate(LevelManager.levelManager.listOfToys[r], transform.localPosition, LevelManager.levelManager.listOfToys[r].transform.rotation) as GameObject;
				toy.transform.SetParent(transform);
				// toy.transform.SetAsFirstSibling();
				toy.transform.localScale = Vector3.one;
				toy.transform.localPosition = Vector3.zero;
			}
			else
			{
				// Set baloon type
				baloonType = 0; // regular baloon

				// Destroying uneccessary objects
				Destroy(transform.GetChild(1).gameObject);
			}

			movingSpeed = Random.Range(LevelManager.levelManager.minBaloonSpeed, LevelManager.levelManager.maxBaloonSpeed);
		}
		else if (tag == "InnerBaloon")
		{
			baloonType = Random.Range(0, 2);

			switch(baloonType)
			{
			case 0:
				break;
			case 1:
				// Create toy object
				int r = Random.Range(0, LevelManager.levelManager.listOfToys.Length);

				GameObject toy = Instantiate(LevelManager.levelManager.listOfToys[r], Vector3.zero, LevelManager.levelManager.listOfToys[r].transform.rotation) as GameObject;
				toy.transform.SetParent(transform);
				// toy.transform.SetAsFirstSibling();
				toy.transform.localScale = Vector3.one;
				toy.transform.localPosition = Vector3.zero;
				break;
			default:
				break;
			}
		}
	}

	void LateUpdate()
	{
		 transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + movingSpeed * Time.deltaTime, transform.localPosition.z);
	}
}
