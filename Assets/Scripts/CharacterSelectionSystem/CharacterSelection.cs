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
    [SerializeField] private int _selectedCharacter;

    [SerializeField] private bool _isSurvivor;
    [SerializeField] private bool _isKiller;

    [SerializeField] private Button _startGameButton;
    [SerializeField] private Button _isReadyButton;

    [SerializeField] private int _indexOfFirstKiller;

    [SerializeField] private bool _isReady;

    private VisualizeClient[] _clients;
    [SerializeField] private GameObject _clientPrefab;
    [SerializeField] private GameObject _clientParent;

    [SerializeField] private int _totalSkins;

    private void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);

        _startGameButton.gameObject.SetActive(false);

        _clients = new VisualizeClient[PhotonNetwork.CurrentRoom.PlayerCount];

        for (int i = 0; i < _clients.Length; i++)
        {
            _clients[i] = Instantiate(_clientPrefab, Vector3.zero, Quaternion.identity, _clientParent.transform).GetComponent<VisualizeClient>();
            _clients[i].Setup(PhotonNetwork.CurrentRoom.Players[i + 1].NickName, i);
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

        _selectedCharacter = 0;

        RaiseCharacterClassChangeEvent();
        RaiseCharacterChangeEvent();
    }

    public void PlayAsKiller()
    {
        UnreadyIfReady();

        _isSurvivor = false;
        _isKiller = true;
        Debug.Log("You play as a killer");

        _selectedCharacter = _indexOfFirstKiller;

        RaiseCharacterClassChangeEvent();
        RaiseCharacterChangeEvent();
    }

    public void NextCharacter()
    {
        UnreadyIfReady();


        if (_isSurvivor)
        {
            _selectedCharacter = (_selectedCharacter + 1) % (_indexOfFirstKiller);
        }
        else if (_isKiller)
        {
            _selectedCharacter++;

            if (_selectedCharacter >= _totalSkins)
            {
                _selectedCharacter = _indexOfFirstKiller;
            }
        }


        RaiseCharacterChangeEvent();
    }

    public void PreviousCharacter()
    {
        UnreadyIfReady();

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
                _selectedCharacter = _totalSkins - 1;
            }
        }

        RaiseCharacterChangeEvent();
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

            _clients[actorNumber - 1].SetReady(isReady);

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

            bool isKiller = (bool)content[1];
            int actorNumber = (int)content[0];

            _clients[actorNumber - 1].SetKiller(isKiller);
        }

        if (photonEvent.Code == (byte)CustomEventCode.CharacterSelcetionChangedCharacter)
        {
            object[] content = (object[])photonEvent.CustomData;

            int actorNumber = (int)content[0];
            int newSkin = (int)content[1];

            _clients[actorNumber - 1].SetSelectedSkin(newSkin);
        }
    }

    private bool AreAllPlayersReady()
    {
        for (int i = 0; i < _clients.Length; i++)
        {
            if (!_clients[i].IsReady())
            {
                return false;
            }
        }

        return true;
    }

    private bool ExistsAKiller()
    {
        for (int i = 0; i < _clients.Length; i++)
        {
            if (_clients[i].IsKiller())
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

    private void RaiseCharacterClassChangeEvent()
    {
        object[] content = new object[]
        {
            PhotonNetwork.LocalPlayer.ActorNumber,
            _isKiller
        };
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent((byte)CustomEventCode.CharacterSelectionClass, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }

    private void RaiseCharacterChangeEvent()
    {
        object[] content = new object[]
        {
            PhotonNetwork.LocalPlayer.ActorNumber,
            _selectedCharacter
        };
        RaiseEventOptions options = new RaiseEventOptions();
        options.Receivers = ReceiverGroup.All;

        PhotonNetwork.RaiseEvent((byte)CustomEventCode.CharacterSelcetionChangedCharacter, content, options, ExitGames.Client.Photon.SendOptions.SendReliable);
    }
}