﻿using UnityEngine;
using System.Collections;

public class SmoothCamera2D : MonoBehaviour {

	public float dampTime = 0.15f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	public float bufferX = 0, bufferY = 0;
	private float shakeQuantity = 0;
	private float startingSize;
	private bool onoff = false;
	//static float timeIndex = .005f;

	void Start() {
		startingSize = this.GetComponent<Camera> ().orthographicSize;
	}

	void Update () 
	{
		if (target)
		{
			dampTime = 8/Vector2.Distance(this.transform.position, target.transform.position);
			Vector3 point = camera.WorldToViewportPoint(target.position);                                      //get the target's position
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(.05f, .05f, point.z));   //change in distance
			Vector3 destination = transform.position + delta;												   //destination vector (messy)
			destination.Set (destination.x + bufferX, destination.y + bufferY, destination.z);				   //destinatino vector (fixed)
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);  //function to move

		}


		if (shakeQuantity > 0) {

				this.transform.eulerAngles = new Vector3 (0, 0, UnityEngine.Random.Range (1 * shakeQuantity, -1 * shakeQuantity));
				this.GetComponent<Camera> ().orthographicSize = startingSize + UnityEngine.Random.Range (.05f * shakeQuantity, -.05f * shakeQuantity);
				if (onoff) {
					shakeQuantity = 1; 
				} else {

					shakeQuantity -= 10 * Time.deltaTime;
				}
			
		} else {
				this.transform.eulerAngles = new Vector3 (0, 0, 0);
				this.GetComponent<Camera> ().orthographicSize = startingSize;
		}
	
	}

	void Shake(float amount) {
		shakeQuantity = amount;
		//onoff = false;
	}

	void ShakeOnOff(bool b) {
		onoff = b;
	}
}