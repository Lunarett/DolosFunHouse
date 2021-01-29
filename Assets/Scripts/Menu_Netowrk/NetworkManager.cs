using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Photon.Realtime;
using Photon.Pun;

public class NetworkManager : MonoBehaviourPunCallbacks
{
	[Header("Room Options Settings")]
	[SerializeField] private TMP_InputField _title;
	[SerializeField] private TMP_Text _maxPlayers;
	[Space]
	[Header("MainMenu Modal Window")]
	[SerializeField] private UnityEvent OpenMainMenu;
	[Header("FindLobby Modal Window")]
	[SerializeField] private UnityEvent OpenFindLobby;
	[Header("CreateLobby Modal Window")]
	[SerializeField] private UnityEvent OpenCreateLobby;
	[Header("Lobby Modal Window")]
	[SerializeField] private UnityEvent OpenLobby;
	[SerializeField] private NetworkRoomView _networkRoomView;
	[SerializeField] private TMP_InputField _inputNickname;
	[Space]
	[Header("Error Message Handler")]
	[SerializeField] private TMP_Text _description;
	[SerializeField] private UnityEvent OpenErrorMessage;
	
	private RoomOptions _options = new RoomOptions();
	private Dictionary<string, RoomInfo> _cachedRoomList = new Dictionary<string, RoomInfo>();

	public Dictionary<string, RoomInfo> CachedRoomList { get => _cachedRoomList; }

	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings();

		byte mp;

		if (byte.TryParse(_maxPlayers.text, out mp))
			_options.MaxPlayers = mp;
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public void SetPlayerName()
	{
		PhotonNetwork.NickName = _inputNickname.text;
	}

	public void FindLobbyButton()
	{
		if(PhotonNetwork.IsConnected)
		{
			OpenFindLobby.Invoke();
		}
		else
		{
			_description.text = ErrorMsg.Instance.NoConnection();
		}
	}

	public void CreateLobbyButton()
	{
		if (PhotonNetwork.IsConnected)
		{
			OpenCreateLobby.Invoke();
		}
		else
		{
			_description.text = ErrorMsg.Instance.NoConnection();
		}
	}

	public void RetryConnection()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public void SetMaxPlayers()
	{
		byte result;

		if(byte.TryParse(_maxPlayers.text, out result))
		{
			_options.MaxPlayers = result;
		}

	}

	public void CreateRoom()
	{
		if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
		{
			PhotonNetwork.CreateRoom(_title.text, _options, TypedLobby.Default);
		}
		else if(!PhotonNetwork.IsConnected)
		{
			_description.text = ErrorMsg.Instance.NoConnection();
			OpenErrorMessage.Invoke();
		}
		else if(!PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom();
			CreateRoom();
		}
	}

	public void JoinRoom(string roomName)
	{
		if (PhotonNetwork.IsConnected && !PhotonNetwork.InRoom)
		{
			PhotonNetwork.JoinRoom(roomName);
		}
		else if (!PhotonNetwork.IsConnected)
		{
			_description.text = ErrorMsg.Instance.NoConnection();
			OpenErrorMessage.Invoke();
		}
		else if (!PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom();
			JoinRoom(roomName);
		}
	}

	public void DisconnectFromRoom()
	{
		if(PhotonNetwork.InRoom)
		{
			PhotonNetwork.LeaveRoom();
		}
	}
	
	public bool CheckIsTileTaken(string name)
	{
		foreach (KeyValuePair<string, RoomInfo> roomList in _cachedRoomList)
		{
			if (roomList.Value.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	public override void OnCreatedRoom()
	{
		OpenLobby.Invoke();
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		_description.text = $"{returnCode}: {message}";
		OpenErrorMessage.Invoke();
	}

	public override void OnLeftRoom()
	{
		OpenMainMenu.Invoke();
	}

	public override void OnJoinedRoom()
	{
		OpenLobby.Invoke();
	}

	public override void OnDisconnected(DisconnectCause cause)
	{
		if(PhotonNetwork.LocalPlayer != null)
			RetryConnection();
	}

	private void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
	}

	public void OnExitGame()
	{
		Application.Quit();
	}
}
