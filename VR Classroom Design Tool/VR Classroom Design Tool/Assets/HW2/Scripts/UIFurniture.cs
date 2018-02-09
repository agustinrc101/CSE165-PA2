using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIFurniture : MonoBehaviour {
	[SerializeField] private GameObject furniture;
	[SerializeField] private Transform parent;
	//[SerializeField] private Vector3 spawnLocation;

	public void spawn(Vector3 handPosition) {
		Instantiate(furniture, handPosition, furniture.transform.rotation, parent);
	}
}
