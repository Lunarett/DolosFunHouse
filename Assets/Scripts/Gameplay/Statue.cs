using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Statue : Interactible
{
    private bool _isActivated;
    [SerializeField] private StatuePiece _key;
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private Transform _keySpawnTransform;

    public void Start()
    {
        _key.gameObject.SetActive(true);
    }

    public override void StartInteract(int playerViewID)
    {
        photonView.RPC("RPC_StartInteract", RpcTarget.All, playerViewID);
    }
    //very important! "You cannot pass Transforms through an RPC. You can only pass through basic types like ints, floats, strings, Vector2s,3s and Quaternions." -https://www.reddit.com/r/Unity3D/comments/f05tcz/photon_pun_2_error/
    [PunRPC]
    private void RPC_StartInteract(int playerViewID)
    {
        base.StartInteract(playerViewID);

        Debug.Log("Interact");
        if (_key.GetPlayerViewID() != 0)
        {
            if (_key.GetPlayerViewID() == playerViewID && !_isActivated)
                StartActivate();
        }
        else
        {
            Debug.Log("Key missing player");
        }
    }

    private void StartActivate()
    {
        photonView.RPC("RPC_Activate", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_Activate()
    {
        Debug.Log("Statue '" + gameObject.name + "' activated");

        PhotonNetwork.Destroy(_key.gameObject);

        _isActivated = true;
    }

    private void SpawnKey()
    {
        _key = PhotonNetwork.Instantiate(_keyPrefab.name, _keySpawnTransform.position, Quaternion.identity).gameObject.GetComponent<StatuePiece>();
    }
}
