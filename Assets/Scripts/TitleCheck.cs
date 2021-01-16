using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Realtime;
using TMPro;

public class TitleCheck : MonoBehaviour
{
	[Header("Error Message Modal")]
	[SerializeField] private UnityEvent _openErrorMessage;
	[SerializeField] private TMP_Text _errorDescription;
	[Header("Enter Nickname Modal")]
	[SerializeField] private UnityEvent _procceedToEnterName;
	[Space]
	[SerializeField] private string _noTitle;
	[SerializeField] private string _titleExists;
	[SerializeField] private NetworkManager _networkManager;

	public TMP_InputField TitleText;

	public void OpenWindow()
	{
		if (string.IsNullOrWhiteSpace(TitleText.text))
		{
			_errorDescription.text = _noTitle;
			_openErrorMessage.Invoke();
		}
		else if(_networkManager.CheckIsTileTaken(TitleText.text))
		{
			_errorDescription.text = _titleExists;
			_openErrorMessage.Invoke();
		}
		else
		{
			_networkManager.CreateRoom();
		}
	}
 }

