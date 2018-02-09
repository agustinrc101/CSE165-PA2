using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenuButton : MonoBehaviour {
	[SerializeField] private GameObject _nextMenu;

	public void openMenu(){
		_nextMenu.SetActive(true);
		transform.parent.gameObject.SetActive(false);
	}
}
