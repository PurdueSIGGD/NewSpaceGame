﻿using UnityEngine;
using System.Collections;

public class JointScript : MonoBehaviour {
	private LineRenderer lr;
	private SpringJoint2D sp;
	private bool broken;
	public bool severed;
	private GameObject focus;
	// Use this for initialization
	void Start () {
		severed = false;
		lr = this.GetComponent<LineRenderer>();
		sp = this.GetComponent<SpringJoint2D>();
		lr.SetVertexCount(2);

	}
	
	// Update is called once per frame
	void Update () {
		if (!broken) {
			lr = this.GetComponent<LineRenderer>();
			sp = this.GetComponent<SpringJoint2D>();
			lr.SetVertexCount(2);
			lr.SetPosition(0,this.transform.position);
			if (sp.connectedBody != null) {
				lr.SetPosition(1,new Vector3(sp.connectedBody.transform.position.x, sp.connectedBody.transform.position.y, this.transform.position.z));
				SpringJoint2D attempt;
				if ((attempt = sp.connectedBody.GetComponent<SpringJoint2D>()) != null) {
					if (attempt.connectedBody != null) {
						//print("I am " + this.name + "Name of third is: " + attempt.name);
						lr.SetVertexCount(3);
						lr.SetPosition(2, attempt.connectedBody.transform.position);
					}
				}
			}
		}

	}
	void BrokenJoint() {

		Destroy(this.GetComponent<SpringJoint2D>());
		lr.SetVertexCount(0);
		broken = true;
		if (!severed && focus != null) {
			focus.BroadcastMessage ("BrokenRope");
		}

	}
	void GiveFocus(GameObject g) {
		focus = g;
	}
	void ReconnectJoint() {
		broken = false;
		//SpringJoint2D end = this.gameObject.AddComponent<SpringJoint2D>();

	}
}
