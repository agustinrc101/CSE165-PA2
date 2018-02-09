using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointingDetector : MonoBehaviour {
	[SerializeField] private OVRInput.Controller _controller;
	[SerializeField] private Collider _sphereCollider;

	// Use this for initialization
	void Start () {
		_sphereCollider.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		_sphereCollider.enabled = !OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, _controller);
	}
}
