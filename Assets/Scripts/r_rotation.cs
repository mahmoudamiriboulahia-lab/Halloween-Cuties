using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class r_rotation : MonoBehaviour {

	public float nilai;

	void Start(){
		
	}
	void FixedUpdate(){

			transform.Rotate(0,0,Time.deltaTime * nilai);
		
		
	}
}
