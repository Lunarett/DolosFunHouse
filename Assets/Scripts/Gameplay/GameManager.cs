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

    /*
    public void SpawnPlayers(int ammount)
    {
        _players = new PlayerController[ammount];

        Vector3 spacing = new Vector3(2, 0, 0);

        if (PhotonNetwork.OfflineMode)
        {
            for (int i = 0; i < ammount; i++)
            {
                _players[i] = Instantiate(_playerPrefab, _spawnPoint.position + spacing * i, Quaternion.identity).GetComponentInChildren<PlayerController>();
                //_players[i].transform.parent.gameObject.transform.position += spacing * i;
                Debug.Log(_players[i].transform.parent.gameObject.name);
            }
        }

        Debug.Log("Spawned " + ammount + " players.");
    } */

    public void SpawnPlayer()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Instantiate(_playerPrefab.name, _spawnPoint.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_playerPrefab, _spawnPoint.position, Quaternion.identity);
            Debug.Log("couldn't connect to photon network");
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
            SpawnPlayer();
        }
    }
}
