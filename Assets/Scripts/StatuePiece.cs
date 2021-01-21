using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatuePiece : Interactible
{
    private Statue _parent = null;
    private PlayerController _playerController = null;
    //[SerializeField] private GameObject _mesh;

    public StatuePiece(Statue parent)
    {
        _parent = parent;
    }

    public override void Interact(GameObject playerObject)
    {
        base.Interact(playerObject);

        _playerController = playerObject.GetComponent<PlayerController>();
        //_mesh.SetActive(false);
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        Debug.Log("Piece picked up");
        Debug.Log("Player " + _playerController);

    }

    public PlayerController GetCurrentPlayer()
    {
        return _playerController;
    }
}
