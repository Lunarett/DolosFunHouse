using UnityEngine;
using Photon.Pun;
using TMPro;

public class UpdateRoomData : MonoBehaviour
{
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _slots;

	private UpdateRoomList _updateRoomList;
	private string roomName = "";

	public void Setup(string title, int currentPlayers, int maxPlayers, UpdateRoomList updateRoomList)
	{
		_title.text = title;
		roomName = title;
		_updateRoomList = updateRoomList;

		_slots.text = $"{currentPlayers}/{maxPlayers}";
	}

	public void JoinRoom()
	{
		
		_updateRoomList.JoinRoom(roomName);
	}

}

