using UnityEngine;
using System.Collections;
using UnityEngine.UI;

///<summary>
///<para>Scene:All/NameOfScene/NameOfScene1,NameOfScene2,NameOfScene3...</para>
///<para>Object:N/A</para>
///<para>Description: Sample Description </para>
///</summary>

public class BaloonsDestroyer : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D coll)
	{
		if (coll.transform.parent.tag == "Baloon" || coll.transform.parent.tag == "InnerBaloon")
		{
			Destroy(coll.gameObject.transform.parent.gameObject);
		}
	}
}
