using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class UpdateRoomList : MonoBehaviourPunCallbacks
{
	[Header("Nickname Modal")]
	[SerializeField] GameObject _lobbyButtonPrefab;
	[SerializeField] NetworkManager _networkManager;

	public void RefreshRoomList()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		if(PhotonNetwork.InLobby && _networkManager.CachedRoomList != null)
		{
			foreach (var lobby in _networkManager.CachedRoomList.Values)
			{
				if(lobby.IsOpen)
				{
					GameObject obj = Instantiate(_lobbyButtonPrefab, transform);
					obj.GetComponent<UpdateRoomData>().Setup(lobby.Name, lobby.PlayerCount, lobby.MaxPlayers, this);
				}
			}
		}
	}

	public void JoinRoom(string roomName)
	{
		_networkManager.JoinRoom(roomName);
	}

	public override void OnJoinedLobby()
	{
		Debug.Log("Joined Lobby!");
	}

	public override void OnRoomListUpdate(List<RoomInfo> roomList)
	{
		Debug.Log($"Room List Updated!   Time: {Time.time} Amount: {roomList.Count}");

		foreach(var room in roomList)
		{
			if(room.RemovedFromList)
			{
				Debug.Log($"Removed Room: {room.Name}");
			}
			else
			{
				Debug.Log($"Added Room: {room.Name}");
			}
		}

		UpdateCachedRoomList(roomList);
		RefreshRoomList();
	}

	private void UpdateCachedRoomList(List<RoomInfo> roomList)
	{
		for (int i = 0; i < roomList.Count; i++)
		{
			RoomInfo info = roomList[i];
			if (info.RemovedFromList)
			{
				_networkManager.CachedRoomList.Remove(info.Name);
			}
			else
			{
				_networkManager.CachedRoomList[info.Name] = info;
			}
		}
	}

	public override void OnLeftLobby()
	{
		Debug.LogWarning("Left lobby!");
	}
}
