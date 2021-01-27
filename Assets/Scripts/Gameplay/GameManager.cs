using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerController[] _players;
    [SerializeField] private GameObject _playerPrefab;

    [SerializeField] private Transform _spawnPoint;


    void Start()
    {
        _players = new PlayerController[4];

        SpawnPlayers(4);
    }

    public void SpawnPlayers(int ammount)
    {
        Vector3 spacing = new Vector3(2, 0, 0);
        
        for(int i = 0; i < ammount; i++)
        {
            _players[i] = Instantiate(_playerPrefab, _spawnPoint.position + spacing * i, Quaternion.identity).GetComponentInChildren<PlayerController>();
            //_players[i].transform.parent.gameObject.transform.position += spacing * i;
            Debug.Log(_players[i].transform.parent.gameObject.name);
        }

        Debug.Log("Spawned " + ammount + " players.");
    }
}
