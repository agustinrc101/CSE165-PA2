using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HandAction {
	EMPTY,
	TOUCHING,
	HOLDING
}

public class Hands : MonoBehaviour {
	//Ideas from http://www.rgbschemes.com/blog/oculus-touch-and-finger-stuff-part-1/

	//TODO:
	//	- Implement a second grabbing/manipulation methods
	//	- Implement group selection

	[Tooltip("Oculus controller")]
	[SerializeField] private OVRInput.Controller _controller;    //LTouch or RTouch
	[Tooltip("Where the picked up objects will be attached to")]
	[SerializeField] private Rigidbody _attachPoint = null;
	//[Tooltip("Allows picking up objects and have them stay on the user hand, or to center objects that are picked up")]
	//[SerializeField] private bool _ignoreContactPoint = false;
	[Tooltip("Used for picking up multiple objects")]
	[SerializeField] private Transform _groupSelection;
	[Tooltip("Button press offset needed to pick up an object")]
	[SerializeField] private float _grabOffset = 0.55f;
	[Tooltip("Button press offset needed to let go of an object")]
	[SerializeField] private float _letGoOffset = 0.35f;
	//[Tooltip("Highlight Color")]
	//[SerializeField] private Color _highlightColor;
	[SerializeField] private float selectionAlphaMax = 0.3f;


	private HandAction _handAction = HandAction.EMPTY;
	private Rigidbody _heldObject = null;
	private FixedJoint _curJoint = null;
	

	void Start() {
		if (_attachPoint == null)
			warningRigidBody("" + _controller);
	}

	void Update() {
		switch (_handAction) {
			case HandAction.TOUCHING:
				if (_curJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) >= _grabOffset){
					_heldObject.velocity = Vector3.zero;
					_curJoint = _heldObject.gameObject.AddComponent<FixedJoint>();
					_curJoint.connectedBody = _attachPoint;
					_handAction = HandAction.HOLDING;
					
				}

				break;
			case HandAction.HOLDING:
				highlight(_heldObject.gameObject, selectionAlphaMax);

				if (_curJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < _letGoOffset){
					Object.DestroyImmediate(_curJoint);
					_curJoint = null;
					throwObject();
					_handAction = HandAction.EMPTY;
				}

				break;
		}
	}

	private void throwObject() {
		_heldObject.velocity = OVRInput.GetLocalControllerVelocity(_controller);
		_heldObject.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(_controller);
		_heldObject.maxAngularVelocity = _heldObject.angularVelocity.magnitude;
		highlight(_heldObject.gameObject, 0.0f);
	}

	void OnTriggerEnter(Collider other) {
		if (_handAction == HandAction.EMPTY) {
			GameObject obj = other.gameObject;

			if (obj.CompareTag("Furniture") && obj != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < _grabOffset) {
				_heldObject = obj.GetComponent<Rigidbody>();
				_handAction = HandAction.TOUCHING;

				if (_heldObject == null) {
					warningRigidBody(obj.name);
					_heldObject = null;
					return;
				}

				highlight(_heldObject.gameObject, selectionAlphaMax);
			}
		}
	}

	void OnTriggerExit(Collider other) {
		if (_handAction != HandAction.HOLDING) {
			if (other.gameObject.CompareTag("Furniture")) {
				if(_heldObject != null)
					highlight(_heldObject.gameObject, 0.0f);

				_heldObject = null;
				_handAction = HandAction.EMPTY;

			}
		}
	}


	//Highlights/unhighlights object 
	private void highlight(GameObject obj, float alpha) {
		if (obj.GetComponent<MeshRenderer>() != null) {
			int matSize = obj.GetComponent<MeshRenderer>().materials.Length;
			Color c = obj.GetComponent<MeshRenderer>().materials[matSize - 1].GetColor("_OutlineColor"); ;
			obj.GetComponent<MeshRenderer>().materials[matSize - 1].SetColor("_OutlineColor", new Color(c.r, c.g, c.b, alpha));
		}

		Component[] meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
		foreach (MeshRenderer mesh in meshRenderers) {
			int l = mesh.materials.Length;
			Color c = mesh.materials[l - 1].GetColor("_OutlineColor");
			mesh.materials[l - 1].SetColor("_OutlineColor", new Color(c.r, c.g, c.b, alpha));
		}
	}

	//Helper for printing out missing rigidbody messages
	private void warningRigidBody(string s) {
		Debug.Log(s + " does not have a rigidbody attached");
	}

	//Returns whether or not this object is already holding something
	public bool isHolding() {
		return (_handAction == HandAction.HOLDING);
	}
}
