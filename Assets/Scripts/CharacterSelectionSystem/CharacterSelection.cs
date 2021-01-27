using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using ExitGames.Client.Photon;

public class CharacterSelection : MonoBehaviour, IOnEventCallback
{
    [SerializeField] private GameObject[] _characterPrefabs;
    [SerializeField] private int _selectedCharacter;
    [SerializeField] private Vector3 _spawnPos;
    private GameObject[] _characters;

    [SerializeField] private bool _isSurvivor;
    [SerializeField] private bool _isKiller;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _isReadyButton;
    [SerializeField] private Button _isDefaultClassButton;

    [SerializeField] private int _indexOfFirstKiller;

    [SerializeField] private bool _isReady;
    [SerializeField] private bool _isDefaultClass = true;

    private bool[] _readyStates;
    private bool[] _classStates;

    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);

        _startGameButton.gameObject.SetActive(false);

        _characters = new GameObject[_characterPrefabs.Length];
        _readyStates = new bool[PhotonNetwork.CurrentRoom.PlayerCount];
        _classStates = new bool[PhotonNetwork.CurrentRoom.PlayerCount];

        for (int i = 0; i < _characterPrefabs.Length; i++)
        {
            _characters[i] = Instantiate(_characterPrefabs[i], _spawnPos, Quaternion.identity, gameObject.transform);
            _characters[i].SetActive(false);
        }

        PlayAsSurvivor();
    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void PlayAsSurvivor()
    {
        UnreadyIfReady();

        _isKiller = false;
        _isSurvivor = true;
        Debug.Log("You play as a survivor");

        _characters[_selectedCharacter].SetActive(false);
        _selectedCharacter = 0;
        _characters[_selectedCharacter].SetActive(true);

        //object[] content = new object[]
        //{
        //    PhotonNetwork.LocalPlayer.ActorNumber,
        //    _isSurvivor
        //};
        //RaiseEventOptions options = new RaiseEventOptions();
        //options.Receivers = ReceiverGroup.All;
        //
        //PhotonNetwork.RaiseEvent((byte)CustomEventCode.CharacterSelectionClass, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public void PlayAsKiller()
    {
        UnreadyIfReady();

        _isSurvivor = false;
        _isKiller = true;
        Debug.Log("You play as a killer");

        _characters[_selectedCharacter].SetActive(false);
        _selectedCharacter = _indexOfFirstKiller;
        _characters[_selectedCharacter].SetActive(true);

        //object[] content = new object[]
        //{
        //    PhotonNetwork.LocalPlayer.ActorNumber,
        //    _isKiller
        //};
        //RaiseEventOptions options = new RaiseEventOptions();
        //options.Receivers = ReceiverGroup.All;
        //
        //PhotonNetwork.RaiseEvent((byte)CustomEventCode.CharacterSelectionClass, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public void SwitchClass()
    {
        UnreadyIfReady();

        if (_isDefaultClass)
        {
            _isDefaultClass = false;
            Debug.Log("You play as a killer");
            _isDefaultClassButton.GetComponentInChildren<TMP_Text>().text = "Play as survivor";

            _characters[_selectedCharacter].SetActive(false);
            _selectedCharacter = _indexOfFirstKiller;
            _characters[_selectedCharacter].SetActive(true);
        }
        else
        {
            _isDefaultClass = true;
            Debug.Log("You play as a survivor");
            _isDefaultClassButton.GetComponentInChildren<TMP_Text>().text = "Play as killer";

            _characters[_selectedCharacter].SetActive(false);
            _selectedCharacter = 0;
            _characters[_selectedCharacter].SetActive(true);
        }

        object[] content = new object[]
        {
            PhotonNetwork.LocalPlayer.ActorNumber,
            _isDefaultClass
        };
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent((byte)CustomEventCode.CharacterSelectionClass, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public void NextCharacter()
    {
        UnreadyIfReady();

        _characters[_selectedCharacter].SetActive(false);

        if (_isSurvivor)
        {
            _selectedCharacter = (_selectedCharacter + 1) % (_indexOfFirstKiller);
        }
        else if (_isKiller)
        {
            _selectedCharacter++;

            if (_selectedCharacter >= _characters.Length)
            {
                _selectedCharacter = _indexOfFirstKiller;
            }
        }

        _characters[_selectedCharacter].SetActive(true);
    }

    public void PreviousCharacter()
    {
        UnreadyIfReady();

        _characters[_selectedCharacter].SetActive(false);
        _selectedCharacter--;

        if (_isSurvivor)
        {
            if (_selectedCharacter < 0)
            {
                _selectedCharacter = _indexOfFirstKiller - 1;
            }
        }
        else if (_isKiller)
        {
            if (_selectedCharacter < _indexOfFirstKiller)
            {
                _selectedCharacter = _characters.Length - 1;
            }
        }

        _characters[_selectedCharacter].SetActive(true);
    }

    public void StartGame()
    {
        if (_isSurvivor || _isKiller)
        {
            PlayerPrefs.SetInt("selectedCharacter", _selectedCharacter);
            SceneManager.LoadScene(2, LoadSceneMode.Single);
        }
        else
        {
            Debug.Log("No faction was selected");
        }
    }

    public void ToggleReady()
    {
        if (_isReady)
        {
            _isReady = false;
            _isReadyButton.GetComponentInChildren<TMP_Text>().text = "Ready";
        }
        else
        {
            _isReady = true;
            _isReadyButton.GetComponentInChildren<TMP_Text>().text = "Not ready";
        }

        object[] content = new object[2]
        {
            PhotonNetwork.LocalPlayer.ActorNumber,
            _isReady
        };
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent((byte)CustomEventCode.CharacterSelectionReady, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        if (photonEvent.Code == (byte)CustomEventCode.CharacterSelectionReady)
        {
            object[] content = (object[])photonEvent.CustomData;

            bool isReady = (bool)content[1];
            int actorNumber = (int)content[0];

            _readyStates[actorNumber - 1] = isReady;

            if (PhotonNetwork.IsMasterClient)
            {
                if (AreAllPlayersReady() && ExistsAKiller())
                {
                    _startGameButton.gameObject.SetActive(true);
                }
                else
                {
                    _startGameButton.gameObject.SetActive(false);
                }
            }
        }

        if (photonEvent.Code == (byte)CustomEventCode.CharacterSelectionClass)
        {
            object[] content = (object[])photonEvent.CustomData;

            //bool isSurvivor = (bool)content[1];
            //bool isKiller = (bool)content[2];
            //int actorNumber = (int)content[0];

            bool isDefaultClass = (bool)content[1];
            int actorNumber = (int)content[0];

            //_classStates[actorNumber - 1] = isSurvivor || isKiller;
            _classStates[actorNumber - 1] = isDefaultClass;
        }
    }

    private void OnGUI()
    {
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            GUILayout.Label(player.Value.NickName + " " + _classStates[player.Key - 1] + " " + _readyStates[player.Key - 1]);
        }
    }

    private bool AreAllPlayersReady()
    {
        for (int i = 0; i < _readyStates.Length; i++)
        {
            if (!_readyStates[i])
            {
                return false;
            }
        }

        return true;
    }

    private bool ExistsAKiller()
    {
        for (int i = 0; i < _classStates.Length; i++)
        {
            if (!_readyStates[i])
            {
                return true;
            }
        }

        return false;
    }

    private void UnreadyIfReady()
    {
        if (_isReady)
        {
            ToggleReady();
        }
    }
}