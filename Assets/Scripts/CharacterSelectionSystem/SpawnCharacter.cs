using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] private GameObject[] _characterPrefabs;
    [SerializeField] private GameObject[] _spawnPoints;
    [SerializeField] private GameObject _currentSpawnPoint;

    private void Start()
    {
        _spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
        int index = Random.Range(0, _spawnPoints.Length);
        _currentSpawnPoint = _spawnPoints[index];

        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");

        GameObject pref = _characterPrefabs[selectedCharacter];
        Instantiate(pref, _currentSpawnPoint.transform.position, Quaternion.identity);
    }
}
