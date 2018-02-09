using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pointing : MonoBehaviour {
	[SerializeField] private OVRInput.Controller _controller;
	[SerializeField] public OvrAvatar _avatar;
	[SerializeField] public OvrAvatar.HandType _handType;
	[SerializeField] private LayerMask _layerMask;
	[SerializeField] private LineRenderer _lineRenderer;
	[SerializeField] private float buttonPressWaitTime = 0.5f;
	[SerializeField] private Transform group;
	[SerializeField] private Transform furnitureParent;
	[SerializeField] private Text box;

	private float time = 0.0f;
	private bool tape = false;
	private Vector3 point1 = new Vector3(-100, -100, -100);
	private Vector3 point2 = new Vector3(-100, -100, -100);

	// Update is called once per frame
	void Update() {
		if (!tape) {
			detection();
		}
		else {
			tapeMeasure();
		}


	}

	private void grouping(GameObject obj) {
		if (obj.GetComponent<GrabbableObject>().getIsInGroup()) {
			obj.GetComponent<GrabbableObject>().setIsInGroup(false);
			obj.transform.SetParent(furnitureParent);
			StartCoroutine(unhighlight(obj));
		}
		else {
			obj.GetComponent<GrabbableObject>().setIsInGroup(true);
			obj.transform.SetParent(group);
			GetComponent<Hands>().highlight(obj);
		}
	}

	private void detection() {
		if (!OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, _controller)
	//&& OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) >= 0.85f
	&& (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, _controller)
	|| OVRInput.Get(OVRInput.Touch.SecondaryThumbRest, _controller)
	|| OVRInput.Get(OVRInput.Touch.One, _controller)
	|| OVRInput.Get(OVRInput.Touch.Two, _controller))
	&& _avatar.HandLeft != null && _avatar.HandRight != null) {
			Vector3 source = _avatar.GetHandTransform(_handType, OvrAvatar.HandJoint.IndexBase).position;
			Vector3 tip = _avatar.GetHandTransform(_handType, OvrAvatar.HandJoint.IndexTip).position;
			Ray ray = new Ray(tip, tip - source);
			RaycastHit hitInfo;


			if (Physics.Raycast(ray, out hitInfo, 100, _layerMask)) {
				_lineRenderer.SetPosition(0, tip);
				_lineRenderer.SetPosition(1, hitInfo.point);

				time += Time.deltaTime;
				if (OVRInput.Get(OVRInput.Button.One, _controller)) {
					if (time >= buttonPressWaitTime) {
						if (hitInfo.collider.CompareTag("UI Menu Button")) {
							hitInfo.collider.GetComponent<UIMenuButton>().openMenu();
							time = 0.0f;
						}
						else if (hitInfo.collider.CompareTag("UI Button")) {
							hitInfo.collider.GetComponent<UIFurniture>().spawn(transform.position);
							time = 0.0f;
						}
						else if (hitInfo.collider.CompareTag("UI CopyPaste")) {
							if (hitInfo.collider.GetComponent<copypaste>().getIsCopy())
								hitInfo.collider.GetComponent<copypaste>().copy();
							else
								hitInfo.collider.GetComponent<copypaste>().paste();

							time = 0.0f;
						}
						else if (hitInfo.collider.CompareTag("saveLoad")) {
							hitInfo.collider.GetComponent<saveload>().act();
							time = 0.0f;
						}
						else if (hitInfo.collider.CompareTag("TapeMeasurer")){
							tape = true;
							time = 0.0f;
						}
						else if (hitInfo.collider.CompareTag("Furniture")) {
							time = 0.4f;

							grouping(hitInfo.collider.gameObject);
						}
					}
					else {
						_lineRenderer.SetPosition(0, Vector3.zero);
						_lineRenderer.SetPosition(1, Vector3.zero);
					}
				}
				else if (OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, _controller) >= 0.55f) {
					//GetComponent<Hands>().setGrab(true);
					//GetComponent<Hands>().setRaycastGrab(hitInfo.collider.gameObject);
				}
			}
			else {
				_lineRenderer.SetPosition(0, Vector3.zero);
				_lineRenderer.SetPosition(1, Vector3.zero);
			}
		}
		else {
			_lineRenderer.SetPosition(0, Vector3.zero);
			_lineRenderer.SetPosition(1, Vector3.zero);
		}
	} 

	private void tapeMeasure() {
		time += Time.deltaTime;
		if (time >= 0.5f) {
			if (point1 == new Vector3(-100, -100, -100)) {
				if (OVRInput.Get(OVRInput.Button.One, _controller)) {
					Vector3 source = _avatar.GetHandTransform(_handType, OvrAvatar.HandJoint.IndexBase).position;
					Vector3 tip = _avatar.GetHandTransform(_handType, OvrAvatar.HandJoint.IndexTip).position;
					Ray ray = new Ray(tip, tip - source);
					RaycastHit hitInfo;

					if (Physics.Raycast(ray, out hitInfo, 100, _layerMask))
						point1 = hitInfo.point;
				}
				time = 0;
			}
			else if (point2 == new Vector3(-100, -100, -100)) {
				if (OVRInput.Get(OVRInput.Button.One, _controller)) {
					Vector3 source = _avatar.GetHandTransform(_handType, OvrAvatar.HandJoint.IndexBase).position;
					Vector3 tip = _avatar.GetHandTransform(_handType, OvrAvatar.HandJoint.IndexTip).position;
					Ray ray = new Ray(tip, tip - source);
					RaycastHit hitInfo;

					if (Physics.Raycast(ray, out hitInfo, 100, _layerMask))
						point2 = hitInfo.point;
				}
				else {
					_lineRenderer.startWidth = 0.01f;
					_lineRenderer.endWidth = 0.01f;

					_lineRenderer.SetPosition(0, point1);
					_lineRenderer.SetPosition(1, transform.position);
					box.text = "" + Vector3.Distance(point1, transform.position);
				}
			}
			else {
				box.text = "" + Vector3.Distance(point1, point2);
				_lineRenderer.SetPosition(0, point1);
				_lineRenderer.SetPosition(1, point2);

				StartCoroutine(tapeCreated());

				_lineRenderer.startWidth = 0.001f;
				_lineRenderer.endWidth = 0.001f;
				point1 = point2 = new Vector3(-100, -100, -100);
			}
		}
	}

	private IEnumerator tapeCreated() {
		yield return new WaitForSeconds(3.5f);
		tape = false;
	}

	private IEnumerator unhighlight(GameObject obj) {
		yield return new WaitForSeconds(0.25f);
		GetComponent<Hands>().highlight(obj, true);
	}
}
