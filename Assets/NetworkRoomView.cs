using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkRoomView : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject _playerButtonPrefab;
	[SerializeField] TMP_InputField _nickname;

	
	private void RefreshRoomList()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		if(PhotonNetwork.InRoom)
		{
			foreach (var player in PhotonNetwork.CurrentRoom.Players)
			{
				GameObject obj = Instantiate(_playerButtonPrefab, transform);
				obj.GetComponent<NetworkRoomPlayerView>().Setup(player.Value, _nickname);
			}
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		RefreshRoomList();
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
	{
		RefreshRoomList();
	}

	public override void OnJoinedRoom()
	{
		RefreshRoomList();
	}

	public override void OnLeftRoom()
	{
		RefreshRoomList();
	}
}
