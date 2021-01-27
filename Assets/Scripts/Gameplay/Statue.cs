using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : Interactible
{
    private bool _isActivated = false;
    private StatuePiece _keyPiece = null;
    [SerializeField] private GameObject _keyPrefab;
    [SerializeField] private Transform _keySpawnTransform;

    public void Start()
    {
        _keyPiece =  Instantiate(_keyPrefab, _keySpawnTransform).GetComponent<StatuePiece>();
    }

    public override void Interact(GameObject playerObject)
    {
        base.Interact(playerObject);

        Debug.Log("Interact");
        if(_keyPiece.GetCurrentPlayer() != null && _keyPiece.GetCurrentPlayer() == playerObject.GetComponent<PlayerController>())
        {
            Activate();
        }
    }

    private void Activate()
    {
        Debug.Log("Statue '" + gameObject.name + "' activated");
        
        Destroy(_keyPiece.gameObject);

        _isActivated = true;
    }
}
