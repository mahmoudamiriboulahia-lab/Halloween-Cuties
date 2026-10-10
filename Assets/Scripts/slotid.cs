using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class slotid : MonoBehaviour , IDropHandler{
	public bool terisi;
	public int containerID;
	Vector2 pos;

	void Start(){
		
		terisi =false;
	}

	public GameObject item{
		get{
			if(transform.childCount > 0){
				return transform.GetChild(0).gameObject;
			}
			return null;
		}

	}

	public void hancurinimage(){
		Destroy(GetComponent<Image>());
	}


 public int idslot;

	#region IDragHandler implementation
		public void OnDrop (PointerEventData eventData){
        if (GameSystem.Instance.GameActive)
        {
			if (!item)
			{
				if (idslot == DragHandler.itemdragged.GetComponent<idobject>().idnya)
				{

					DragHandler.itemdragged.transform.SetParent(transform);
					// DragHandler.itemdragged.GetComponent<RectTransform>().localPosition = new Vector2 (0,0);
					pos = transform.position;
					terisi = true;
					Music_Singleton.Instance.s_play(1);
					DragHandler.itemdragged.GetComponent<idobject>().terpas = true;
					// DragHandler.itemdragged.GetComponent<Image>().enabled = false;
					DragHandler.itemdragged.GetComponent<DragHandler>().candrag = false;

					GameSystem.Instance.Container_ID[containerID].GetComponent<Animation>().Play("container_slot_anim");
					// GameSystem.Instance.Save_Spawn_ID.RemoveAt(GameSystem.Instance.deleted);
					GameSystem.Instance.ID_Answer++;
					GameSystem.Instance.Data_Score += 25;

					if (SceneManager.GetActiveScene().name == "Game1" || SceneManager.GetActiveScene().name == "Game2")
					{
						DragHandler.itemdragged.GetComponent<Image>().sprite = GameSystem.Instance.Game_Image[GameSystem.IDGame].Image[DragHandler.itemdragged.GetComponent<idobject>().idnya];
						DragHandler.itemdragged.GetComponent<Animation>().Stop();
						DragHandler.itemdragged.GetComponent<Image>().color = Color.white;

					}

					StartCoroutine(Spawn());

				}

			}
		}
			
		}

	#endregion

	int myScn = 0;
	IEnumerator Spawn(){
		
		
		if(GameSystem.Instance.ID_Answer < GameSystem.Instance.Container_ID.Count){
			// if score not same as max drag count
		}
		else{
			// if score reach max drag count
			GameSystem.Instance.GameActive = false ;
			
			yield return new WaitForSeconds(0.5f);
			Music_Singleton.Instance.s_play(Random.Range(6,8));
			StartCoroutine(GameSystem.Instance.ResetGame());
		}
	}

}
