using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StatuePiece : Interactible
{
    private Statue _parent;
    private int _playerViewID;

    public StatuePiece(Statue parent)
    {
        _parent = parent;
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

        //_playerView = playerObject.GetComponent<PlayerController>();
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        Debug.Log("Piece picked up");
        _playerViewID = playerViewID;
        gameObject.GetComponent<MeshRenderer>().enabled = false;

    }

    public int GetPlayerViewID()
    {
        return _playerViewID;
    }
}
