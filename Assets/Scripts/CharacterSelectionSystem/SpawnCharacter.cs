using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviour
{
    [SerializeField] private GameObject[] _characterPrefabs;
    [SerializeField] private Transform _spawnPoint;

    void Start()
    {
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");

        GameObject pref = _characterPrefabs[selectedCharacter];
        Instantiate(pref, _spawnPoint.position, Quaternion.identity);
    }
}
