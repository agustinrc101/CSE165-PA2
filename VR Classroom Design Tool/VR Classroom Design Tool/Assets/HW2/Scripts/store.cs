using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class store : MonoBehaviour {
	public static store _store;
	[SerializeField] GameObject g0;
	[SerializeField] GameObject g1;
	[SerializeField] GameObject g2;
	[SerializeField] GameObject g3;
	[SerializeField] GameObject g4;
	[SerializeField] GameObject g5;

	[SerializeField] private GameObject _objToStore;

	void Awake () {
		if (_store == null) {
			DontDestroyOnLoad(gameObject);
			_store = this;
		}
		else if (_store != this)
			Destroy(gameObject);
	}

	public void Save() {
		BinaryFormatter bf = new BinaryFormatter();
		FileStream file = File.Open(Application.persistentDataPath + "/furniture.dat", FileMode.Create);

		FurnitureData data = new FurnitureData();
		data._objToStore = _objToStore;

		bf.Serialize(file, data);
		file.Close();
	}

	public void Load() {
		if (File.Exists(Application.persistentDataPath + "/furniture.dat")) {
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(Application.persistentDataPath + "/furniture.dat", FileMode.Open);
			FurnitureData data = (FurnitureData)bf.Deserialize(file);
			file.Close();

			for (int i = 0; i < data._objToStore.transform.childCount; i++) {
				data._objToStore.transform.GetChild(i).parent = _objToStore.transform;
			}
		}
	}
}

[Serializable]
class FurnitureData {
	public GameObject _objToStore;
}
