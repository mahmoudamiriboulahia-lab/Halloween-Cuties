using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class idobject : MonoBehaviour {
	public bool terpas;

	public int idnya;

	public	Vector2 gcz ;
	private void Start()
	{
		// gcz = GetComponent<GridLayoutGroup>().cellSize;
	}

	void FixedUpdate()
	{
		if(terpas){
			Vector2 jead = GetComponent<RectTransform>().localPosition ;
			GetComponent<RectTransform>().localPosition =  new Vector2(Mathf.Lerp(jead.x,0,0.1125f),Mathf.Lerp(jead.y,0,0.1125f));
			GetComponent<RectTransform>().localScale = new Vector2(1,1);
			GetComponent<RectTransform>().sizeDelta = new Vector2(1,1);
			GetComponent<RectTransform>().localRotation = Quaternion.Euler(Mathf.Lerp(GetComponent<RectTransform>().localRotation.x , 0,0.125f),Mathf.Lerp(GetComponent<RectTransform>().localRotation.x , 0,0.125f),0);
			// GetComponent<RectTransform>().localRotation =  
			


			// int nisn = 100 * GameSystem.ukuran / 150; 
			// Teksnya.GetComponent<Text>().fontSize = (int)Mathf.Lerp(this.Teksnya.GetComponent<Text>().fontSize , nisn , 0.125f);
		}
	}

	public void Destroyed(){
		Destroy(this.gameObject);
	}


	public void GantiGambar (int Back)
    {
        if (Back == 0)
        {
			GetComponent<Image>().sprite = GameSystem.Instance.Blank_Image;
        }
        else
        {
			GetComponent<Image>().sprite = GameSystem.Instance.Game_Image[GameSystem.IDGame].Image[idnya];
		}
    }
}
