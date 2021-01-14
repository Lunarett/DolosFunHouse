using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class UpdateRoomList : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject _lobbyButtonPrefab;
	private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

	public void RefreshRoomList()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		if(PhotonNetwork.InLobby && cachedRoomList != null)
		{
			foreach (var lobby in cachedRoomList.Values)
			{
				if(lobby.IsOpen)
				{
					GameObject obj = Instantiate(_lobbyButtonPrefab, transform);
					obj.GetComponent<UpdateRoomData>().Setup(lobby.Name, lobby.PlayerCount, lobby.MaxPlayers);
				}
			}
		}
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
				cachedRoomList.Remove(info.Name);
			}
			else
			{
				cachedRoomList[info.Name] = info;
			}
		}
	}

	public override void OnLeftLobby()
	{
		Debug.LogWarning("Left lobby!");
	}
}
