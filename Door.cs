using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
	public float stopArea;
	public void doorOpen(){
		
		InvokeRepeating("doorMove", 0.0f, .01f);

			
		
	}
	void doorMove(){
		transform.Translate(0,.01f,0);
		if (gameObject.transform.position.y >= stopArea)
		{
			CancelInvoke();
		}
			
	}


}
