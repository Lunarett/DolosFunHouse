using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VisualizeClient : MonoBehaviour
{
    private bool _isReady;
    private bool _isKiller;
    private int _selectedSkin;

    [SerializeField] private TMP_Text _playerName;
    [SerializeField] private TMP_Text _readyState;
    [SerializeField] private TMP_Text _killerState;
    [SerializeField] private TMP_Text _currentSkin;

    [SerializeField] private GameObject[] _characterPrefabs;
    private GameObject[] _characters;

    public void SetReady(bool readyState)
    {
        _isReady = readyState;

        if (_isReady)
        {
            _readyState.text = "Ready";
        }
        else
        {
            _readyState.text = "Not ready";
        }
    }

    public bool IsReady()
    {
        return _isReady;
    }

    public void SetKiller(bool killerSate)
    {
        _isKiller = killerSate;

        if (_isKiller)
        {
            _killerState.text = "Killer";
        }
        else
        {
            _killerState.text = "Survivor";
        }
    }

    public bool IsKiller()
    {
        return _isKiller;
    }

    public void SetSelectedSkin(int selectedSkin)
    {
        _characters[_selectedSkin].SetActive(false);

        _selectedSkin = selectedSkin;
        _currentSkin.text = "Skin: " + selectedSkin;

        _characters[_selectedSkin].SetActive(true);
    }

    public void Setup(string name, int index)
    {
        _playerName.text = name;

        _characters = new GameObject[_characterPrefabs.Length];

        for (int i = 0; i < _characterPrefabs.Length; i++)
        {
            _characters[i] = Instantiate(_characterPrefabs[i], Vector3.right * index, Quaternion.Euler(0, 180, 0));
            _characters[i].SetActive(false);
        }
    }
}
