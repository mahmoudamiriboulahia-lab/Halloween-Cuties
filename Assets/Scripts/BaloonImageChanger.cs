using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BaloonImageChanger : MonoBehaviour {

	void Awake()
	{
		int r = Random.Range(0, LevelManager.levelManager.baloonImages.Length);

		GetComponent<Image>().sprite = LevelManager.levelManager.baloonImages[r];
	}
}
