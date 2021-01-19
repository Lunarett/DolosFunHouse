using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class NetworkRoomView : MonoBehaviourPunCallbacks
{
	[SerializeField] GameObject _playerButtonPrefab;

	public void RefreshRoomList()
	{
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		if(PhotonNetwork.InRoom)
		{
			foreach (var player in PhotonNetwork.PlayerList)
			{
				GameObject obj = Instantiate(_playerButtonPrefab, transform);
				obj.GetComponent<NetworkRoomPlayerView>().Setup(player);
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
//	// CreateLobby
//	foreach (var player in PhotonNetwork.CurrentRoom.Players)
//	{
//		if (player.Value.IsLocal)
//		{
//			player.Value.NickName = _inputPlayerNameCreate.text;
//		}
//		Debug.Log(player.Value.NickName);
//
//		GameObject obj = Instantiate(_playerButtonPrefab, transform);
//		obj.GetComponent<NetworkRoomPlayerView>().Setup(player.Value, _inputPlayerNameCreate.text);
//	}
