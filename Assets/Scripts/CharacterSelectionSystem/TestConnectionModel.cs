using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TestConnectionModel : MonoBehaviourPunCallbacks
{
    List<RoomInfo> availableRooms;

    public event System.Action<string> ConnectionError;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRandom()
    {
        PhotonNetwork.CreateRoom(null);
    }

    internal void JoinRandomRoom()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void JoinRoom(string name)
    {
        PhotonNetwork.JoinRoom(name);
    }

    public void RenameLocalPlayerTo(string newName)
    {
        PhotonNetwork.LocalPlayer.NickName = newName;
    }

    internal void JoinDefaultLobby()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;
    }

    public List<RoomInfo> GetAllRooms()
    {
        return availableRooms;
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        ConnectionError?.Invoke("Create Room Failed: " + message);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        ConnectionError?.Invoke("Join Random Room Failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        ConnectionError?.Invoke("Join Room Failed: " + message);
    }

    internal void StartGame()
    {
        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
