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
        _selectedSkin = selectedSkin;
        _currentSkin.text = "Skin: " + selectedSkin;
    }

    public void SetName(string name)
    {
        _playerName.text = name;
    }
}
