using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class saveload : MonoBehaviour {
	[SerializeField] private bool isSave = false;
	[SerializeField] private store _store;

	public void act() {
		if (isSave)
			_store.Save();
		else
			_store.Load();
	}

}
