using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using ExitGames.Client.Photon;
using System.Collections.Generic;

public enum CustomEventCode
{
	ForceRefresh = 100,
	CharacterSelectionReady = 101,
	CharacterSelcetionChangedCharacter = 102,
	CharacterSelectionClass = 103
}

public class NetworkRoomView : MonoBehaviourPunCallbacks, IOnEventCallback
{
	[SerializeField] GameObject _playerButtonPrefab;
	[SerializeField] GameObject _startButton;

	private bool Ready;

	private void Start()
	{
		PhotonNetwork.AutomaticallySyncScene = true;
	}

	public void ReadyUp()
	{
		Hashtable hash = new Hashtable();

		Ready = !Ready;
		hash.Add("Ready", Ready);
		PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

		RaiseEventOptions options = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.All;
		PhotonNetwork.RaiseEvent((byte)CustomEventCode.ForceRefresh, null, options, SendOptions.SendReliable);
	}

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

		if(PhotonNetwork.IsMasterClient && IsAllReady())
		{
			//Debug.Log("Worked");
			_startButton.SetActive(true);
		}
		else
		{
			//Debug.Log("Nope");
			_startButton.SetActive(false);
		}
	}

	public void LoadScene()
	{
		PhotonNetwork.LoadLevel(1);
	}

	private bool IsAllReady()
	{
		foreach(var player in PhotonNetwork.CurrentRoom.Players.Values)
		{
			if(!player.CustomProperties.ContainsKey("Ready") || !(bool)player.CustomProperties["Ready"])
			{
				return false;
			}
		}
		return true;
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

	public void OnEvent(EventData photonEvent)
	{
		if(photonEvent.Code == (byte)CustomEventCode.ForceRefresh)
			RefreshRoomList();
	}
}