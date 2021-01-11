using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddServerButton : MonoBehaviour
{
	[SerializeField] GameObject _buttonPrefab;

	public void SpawnButton()
	{
		GameObject btn = Instantiate(_buttonPrefab, transform);
		btn.SetActive(true);
	}
}
