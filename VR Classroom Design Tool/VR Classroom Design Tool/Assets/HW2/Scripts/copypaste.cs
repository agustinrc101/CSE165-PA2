using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class copypaste : MonoBehaviour {
	[SerializeField] private bool _isCopy;
	[SerializeField] private GameObject _grouping;
	[SerializeField] private GameObject _furnitureParent;
	[SerializeField] private copypaste _copy;

	private GameObject copiedObject;

	public void  copy() {
		copiedObject = _grouping;
	}

	public void paste() {
		copiedObject = _copy.getCopiedObject();

		if (copiedObject != null) {
			for (int i = 0; i < _grouping.transform.childCount; i++) {
				GameObject obj = copiedObject.transform.GetChild(i).gameObject;
				StartCoroutine(spawnLocation(obj));
			}
		}
	}

	private IEnumerator spawnLocation(GameObject obj) {
		yield return new WaitForSeconds(0.0f);
		Vector3 pos = obj.transform.position;
		GameObject newobj = Instantiate(obj, pos, obj.transform.rotation, _furnitureParent.transform);
		newobj.GetComponent<GrabbableObject>().setMaterials(obj.GetComponent<GrabbableObject>().getMaterials());

		MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
		List<Material[]> mat = obj.GetComponent<GrabbableObject>().getMaterials();

		for (int i = 0; i < meshes.Length; i++)
			meshes[i].materials = mat[i];

		bool notOut = true;

		while (notOut) {
			foreach (Collider col in obj.GetComponents<Collider>()) {
				if (col.bounds.Contains(newobj.transform.position))
					newobj.transform.position += obj.transform.forward;
				else
					notOut = false;
			}
		}

	}

	public bool getIsCopy() { return _isCopy; }
	public GameObject getCopiedObject() {return copiedObject; }
}
