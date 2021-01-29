using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerUIHandler : MonoBehaviour
{
    [SerializeField] private Image _normalCrossHair;
    [SerializeField] private Image _selectedCrossHair;
    [SerializeField] private GameObject _uiPause;

    private bool _isPaused;


    public bool IsPaused { get => _isPaused; }

	private void Start()
	{
		_uiPause.SetActive(false);
		PhotonNetwork.AutomaticallySyncScene = false;
	}


	public void SwitchCrosshair()
    {
        if (_normalCrossHair.gameObject.activeSelf)
        {
            SelectedCrosshair();
        }
        else
        {
            NormalCrosshair();
        }
    }

    public void SelectedCrosshair()
    {
        _normalCrossHair.gameObject.SetActive(false);
        _selectedCrossHair.gameObject.SetActive(true);
    }

    public void NormalCrosshair()
    {
        _normalCrossHair.gameObject.SetActive(true);
        _selectedCrossHair.gameObject.SetActive(false);
    }

	

	public void OnGamePaused()
	{
		Debug.Log("GAMEPAUSED");
		if (!_isPaused)
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
		SceneManager.LoadScene(0);
	}

	public void ExitToDesktop()
	{
		PhotonNetwork.Disconnect();
		Application.Quit();
	}
}
