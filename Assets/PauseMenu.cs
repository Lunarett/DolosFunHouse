using UnityEngine;
using Photon.Pun;

public class PauseMenu : MonoBehaviour
{
	[SerializeField] private GameObject _uiPause;

	private bool _isPaused;
	private InputMap _inputMap;

	public bool IsPaused { get => _isPaused; }


	private void Awake()
	{
		InitInput();
	}

	private void Start()
	{
		_uiPause.SetActive(false);
		PhotonNetwork.AutomaticallySyncScene = false;

		//OnGamePaused();
	}

	public void OnGamePaused()
	{
		if(!_isPaused)
		{
			_isPaused = true;

			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;

			_uiPause.SetActive(true);
		}
		else
		{
			_isPaused = false;

			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;

			_uiPause.SetActive(false);
		}
	}

	public void ResumeGame()
	{
		_isPaused = false;

		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;

		_uiPause.SetActive(false);
	}

	public void ExitToMenu()
	{
		PhotonNetwork.Disconnect();
		PhotonNetwork.LoadLevel(1);
	}

	public void ExitToDesktop()
	{
		PhotonNetwork.Disconnect();
		Application.Quit();
	}

	private void InitInput()
	{
		_inputMap = new InputMap();

		_inputMap.Player.Escape.performed += context => OnGamePaused();
	}
}
