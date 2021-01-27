using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class RoleIdentifierTimer : MonoBehaviour
{
	[SerializeField] private float _duration;
	[SerializeField] private UnityEvent _eventIn;
	[SerializeField] private UnityEvent _eventOut;

	private void Start()
	{
		_eventIn.Invoke();
		StartCoroutine(Duration());
	}

	IEnumerator Duration()
	{
		yield return new WaitForSeconds(_duration);
		_eventOut.Invoke();
	}
}
