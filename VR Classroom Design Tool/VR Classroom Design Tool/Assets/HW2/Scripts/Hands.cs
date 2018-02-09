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

	[Tooltip("Oculus controller")]
	[SerializeField] private OVRInput.Controller _controller;    //LTouch or RTouch
	[Tooltip("Where the picked up objects will be attached to")]
	[SerializeField] private Rigidbody _attachPoint = null;
	[Tooltip("Used for picking up multiple objects")]
	[SerializeField] private Transform _groupSelection;
	[Tooltip("Button press offset needed to pick up an object")]
	[SerializeField] private float _grabOffset = 0.55f;
	[Tooltip("Button press offset needed to let go of an object")]
	[SerializeField] private float _letGoOffset = 0.35f;
	[Tooltip("Highlighting material")]
	[SerializeField] private Material _highlightMat;
	[SerializeField] private float _waitTimeAfterDrop = 1.5f;

	private HandAction _handAction = HandAction.EMPTY;
	private Rigidbody _heldObject = null;
	private FixedJoint _curJoint = null;
	
	private float _letGoTimer = 0.0f;

	private bool grab = true;

	void Start() {
		if (_attachPoint == null)
			warningRigidBody("" + _controller);
	}

	void Update() {
		_groupSelection = GameObject.FindGameObjectWithTag("groupSelectionObject").transform;


		//if (grab)
			physicalGrab();
		//else
		//	raycastGrab();
	}

	private void physicalGrab() {
		switch (_handAction) {
			case HandAction.TOUCHING:
				if (_curJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) >= _grabOffset) {
					//Checks if this object is part of a group
					if (_groupSelection.transform.childCount > 0) {
						bool isInThere = false;

						for (int i = 0; i < _groupSelection.transform.childCount; i++) {
							if (_groupSelection.transform.GetChild(0).Equals(_heldObject.transform))
								isInThere = true;
						}

						if (isInThere) {
							_heldObject = _groupSelection.GetComponent<Rigidbody>();
						}
					}

					_heldObject.velocity = Vector3.zero;
					_curJoint = _heldObject.gameObject.AddComponent<FixedJoint>();
					_curJoint.connectedBody = _attachPoint;
					_handAction = HandAction.HOLDING;
					_heldObject.transform.rotation = _heldObject.GetComponent<GrabbableObject>().getOriginalRotation();
				}

				break;
			case HandAction.HOLDING:

				if (_curJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < _letGoOffset) {
					Object.DestroyImmediate(_curJoint);
					_curJoint = null;
					throwObject();
					//_heldObject.GetComponent<GrabbableObject>().setDown();
					_handAction = HandAction.EMPTY;
				}

				break;
		}
	}

	private void raycastGrab() {
		switch (_handAction) {
			case HandAction.TOUCHING:
				if (_curJoint == null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) >= _grabOffset) {
					_heldObject.velocity = Vector3.zero;
					//_curJoint = _heldObject.gameObject.AddComponent<FixedJoint>();
					//_curJoint.connectedBody = _attachPoint;
					_handAction = HandAction.HOLDING;
					_heldObject.transform.rotation = _heldObject.GetComponent<GrabbableObject>().getOriginalRotation();

					if (_heldObject.transform.parent.GetInstanceID() == _groupSelection.GetInstanceID()) {
						_heldObject = _groupSelection.GetComponent<Rigidbody>();
					}
				}

				break;
			case HandAction.HOLDING:
				highlight(_heldObject.gameObject);

				if (_curJoint != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < _letGoOffset) {
					Object.DestroyImmediate(_curJoint);
					_curJoint = null;
					throwObject();
					_heldObject.GetComponent<GrabbableObject>().setDown();
					_handAction = HandAction.EMPTY;
					grab = false;
				}

				else if (_curJoint != null && OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, _controller).y != 0) {
					_heldObject.transform.Translate(OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, _controller).y * Vector3.Normalize(transform.position - _heldObject.transform.position) *Time.deltaTime);
				}				

				break;
		}
	}

	private void throwObject() {
		_heldObject.velocity = OVRInput.GetLocalControllerVelocity(_controller);
		_heldObject.angularVelocity = OVRInput.GetLocalControllerAngularVelocity(_controller);
		_heldObject.maxAngularVelocity = _heldObject.angularVelocity.magnitude;
		highlight(_heldObject.gameObject, true);
	}

	void OnTriggerEnter(Collider other) {
		if (_handAction == HandAction.EMPTY) {
			pickUp(other.gameObject);
		}
	}

	void OnTriggerExit(Collider other) {
		if (_handAction != HandAction.HOLDING) {
			if (other.gameObject.CompareTag("Furniture")) {
				if(_heldObject != null)
					highlight(_heldObject.gameObject, true);

				_heldObject = null;
				_handAction = HandAction.EMPTY;

			}
		}
	}

	private void OnTriggerStay(Collider other) {
		if (_handAction == HandAction.EMPTY && other.CompareTag("Furniture")) {
			_letGoTimer += Time.deltaTime;
			if (_letGoTimer >= _waitTimeAfterDrop) {
				pickUp(other.gameObject);
				_letGoTimer = 0f;
			}
		}
	}

	//Picks up object
	private void pickUp(GameObject obj) {
		if (obj.CompareTag("Furniture") && obj != null && OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) < _grabOffset) {
			_heldObject = obj.GetComponent<Rigidbody>();
			_handAction = HandAction.TOUCHING;

			if (_heldObject == null) {
				warningRigidBody(obj.name);
				_heldObject = null;
				return;
			}

			highlight(_heldObject.gameObject);
		}
	}

	//Highlights/unhighlights object 
	public void highlight(GameObject obj, bool reset = false) {
		MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();

		if (reset) {
			List<Material[]> mat = obj.GetComponent<GrabbableObject>().getMaterials();

			for(int i = 0; i < meshes.Length; i++)
				meshes[i].materials = mat[i];
		}
		else {
			foreach (MeshRenderer mesh in meshes) {
				int size = mesh.materials.Length;
				Material[] mat = new Material[size];

				for (int i = 0; i < size; i++)
					mat[i] = _highlightMat;

				mesh.materials = mat;
			}
			
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

	public void setGrab(bool b) {
		grab = b;
	}

	public void setRaycastGrab(GameObject obj) {
		if (_handAction == HandAction.EMPTY) {
			_handAction = HandAction.TOUCHING;
			_heldObject = obj.GetComponent<Rigidbody>();
		}
	}
}
