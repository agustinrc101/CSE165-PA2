using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grabbable : MonoBehaviour {
	[SerializeField] private Material highlight;

	public void Start() {
		//Get the highlight material
	}

	public void GrabObject() {
		gameObject.GetComponent<Rigidbody>().isKinematic = true;
	}

	public void LetGo(Vector3 linearVelocity, Vector3 angularVelocity) {
		Rigidbody rb = gameObject.GetComponent<Rigidbody>();
		rb.isKinematic = false;
		rb.velocity = linearVelocity;
		rb.angularVelocity = angularVelocity;
	}

	public void GroupSelected() {
		
	}

	public void GroupDeselected() {

	}
}
