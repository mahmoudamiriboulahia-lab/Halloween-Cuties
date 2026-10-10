using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DragHandler : MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {
	public static GameObject itemdragged;
	public bool candrag;
[SerializeField]	Vector3 startpos;
[SerializeField]	Transform startparent;
	// Use this for initialization
	void Start () {
		candrag = true;
		startpos = transform.position;
	}

	void Update(){
		// Debug.Log(itemdragged);
	}
	
	#region IBeginDragHandler implementation
		public void OnBeginDrag (PointerEventData eventData){
			if(candrag && GameSystem.Instance.GameActive){
				// Music_Singleton.Instance.s_play(0);
				// Music_Singleton.Instance.s_play(6);
				aktif = false;
				
				startparent = transform.parent;
				itemdragged = gameObject;
				Music_Singleton.Instance.s_play(2);
				
				GetComponent<CanvasGroup>().blocksRaycasts = false;
			}
		}
	#endregion

	#region IDragHandler implementation
		public void OnDrag (PointerEventData eventData){
        if (GameSystem.Instance.GameActive)
        {
			if (candrag)
			{
				transform.position = Input.mousePosition;
			}
		}
			
		}

	#endregion

	#region IEndDragHandler implementation
		public void OnEndDrag (PointerEventData eventData){
        if (GameSystem.Instance.GameActive)
        {
			if (candrag)
			{
				itemdragged = null;
				GetComponent<CanvasGroup>().blocksRaycasts = true;
				if (transform.parent != startparent)
				{
					// aktif = true;
					transform.position = new Vector2(0, 0);


					Debug.Log("di dalam");

				}
				else
				{
					// Music_Singleton.Instance.s_play(7);
					// Music_Singleton.Instance.source[7].volume = 0.3f;
					Music_Singleton.Instance.s_play(3);


					aktif = true;
					Debug.Log("di luar");
				}
			}
		}
			
			
			
			
			
		}	

	#endregion

	bool aktif ;
	bool membesar;

	void FixedUpdate(){
		if(aktif){
			transform.position = new Vector3(Mathf.Lerp(this.transform.position.x , startpos.x ,0.1f),Mathf.Lerp(this.transform.position.y , startpos.y ,0.1f),0);
		}

	}
}
