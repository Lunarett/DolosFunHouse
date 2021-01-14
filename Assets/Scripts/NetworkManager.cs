using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;


public class NetworkManager : MonoBehaviourPunCallbacks
{
	[SerializeField] private TMP_InputField _title;
	[SerializeField] private TMP_Text _maxPlayers;

	private RoomOptions _options = new RoomOptions();

	private void Start()
	{
		PhotonNetwork.ConnectUsingSettings();
	}

	public override void OnConnectedToMaster()
	{
		PhotonNetwork.JoinLobby();
	}

	public void SetMaxPlayers()
	{
		byte result;

		if(byte.TryParse(_maxPlayers.text, out result))
		{
			Debug.Log(result);
			_options.MaxPlayers = result;
		}

	}

	public void CreateRoom()
	{
		if (!PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
			return;

		PhotonNetwork.CreateRoom(_title.text, _options, TypedLobby.Default);
	}

	public void DisconnectFromRoom()
	{
		if(PhotonNetwork.InRoom)
			PhotonNetwork.LeaveRoom();
	}

	public void JoinRoom(string name)
	{
		PhotonNetwork.JoinRoom(name);
	}
	
	public override void OnCreatedRoom()
	{
		Debug.Log("Room has been created!");
	}

	public override void OnCreateRoomFailed(short returnCode, string message)
	{
		Debug.LogWarning($"Failed to create a room! Code: {returnCode}, Message: {message}.");
	}

	public override void OnLeftRoom()
	{
		Debug.Log($"Left Room");
	}

	private void OnGUI()
	{
		GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
	}
}
