using UnityEngine;
using Photon.Pun;
using TMPro;

public class UpdateRoomData : MonoBehaviour
{
	[SerializeField] private TMP_Text _title;
	[SerializeField] private TMP_Text _slots;

	private string roomName;

	public void Setup(string title, int currentPlayers, int maxPlayers, GameObject enterNickname)
	{
		_title.text = title;
		roomName = title;

		_slots.text = $"{currentPlayers}/{maxPlayers}";


	}

	public void JoinRoom()
	{
		PhotonNetwork.JoinRoom(roomName);
	}

}

