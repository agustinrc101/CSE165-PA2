using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbableObject : MonoBehaviour {
	private List<Material[]> _materials;
	private Quaternion _originalRotation;

	//private bool inBounds = true;
	private bool isInGroup = false;

	// Use this for initialization
	void Start () {
		_materials = new List<Material[]>();
		_originalRotation = transform.rotation;

		storeMaterials();
	}

	//Stores the materials for highlighting
	private void storeMaterials() {
		MeshRenderer[] meshes = gameObject.GetComponentsInChildren<MeshRenderer>();

		if (meshes != null) {
			foreach (MeshRenderer mesh in meshes)
				_materials.Add(mesh.materials);
		}
	}

	//Getters
	public List<Material[]> getMaterials() {return _materials;}
	public  void setMaterials(List<Material[]> m) { _materials = m; }
	public Quaternion getOriginalRotation() { return _originalRotation; }
	public void setDown() { }//inBounds = false; }
	public bool getIsInGroup() { return isInGroup; }
	public void setIsInGroup(bool b) { isInGroup = b; }

	//private void OnCollisionStay(Collision other) {
	//	if (!inBounds && other.collider.bounds.Contains(transform.position)) {
	//		transform.Translate(Vector3.Normalize(Vector3.zero - transform.position) * Time.deltaTime);
	//	}
	//	else {
	//		inBounds = true;
	//	}
	//}

	//private void OnCollisionExit(Collision collision) {
	//	inBounds = true;
	//}
}
