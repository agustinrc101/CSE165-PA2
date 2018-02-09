using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroupingBehavior : MonoBehaviour {
	[SerializeField] private Material _highlightMat;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (transform.childCount > 0) {
			for (int i = 0; i < transform.childCount; i++)
				StartCoroutine(highlight(transform.GetChild(0).gameObject));
		}
	}

	//Highlights/unhighlights object 
	public IEnumerator highlight(GameObject obj, bool reset = false) {
		yield return new WaitForSeconds(0.0f);
		MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();

		if (reset) {
			List<Material[]> mat = obj.GetComponent<GrabbableObject>().getMaterials();

			for (int i = 0; i < meshes.Length; i++)
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
}
