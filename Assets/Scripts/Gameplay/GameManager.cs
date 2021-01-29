using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviourPunCallbacks
{
    private PlayerController[] _players;
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Transform _spawnPoint;


    void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.OfflineMode = true;
        }
        else
        {
            SpawnPlayer();
        }

    }

    public GameObject SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected)
        {
            return PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("couldn't connect to photon network");
            return Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
        }
    }

    public override void OnConnectedToMaster()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PhotonNetwork.CreateRoom("OfflineMode");
        }
    }

    public override void OnCreatedRoom()
    {
        if (PhotonNetwork.OfflineMode)
        {
            PlayerController player = SpawnPlayer().GetComponent<PlayerController>();
        }
    }
}
