using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class KegScript : Interactible
{
    private GameObject _player = null;
    private bool _isOccupied = false;

    [SerializeField] private Transform _exitTransform;
    [SerializeField] private Camera _peekCam;


    public override void Interact(GameObject playerObject)
    {
        base.Interact(playerObject);

        if (!_isOccupied)
        {
            MovePlayer(playerObject, transform);
            _player = playerObject;
            PlayerController playerController = _player.GetComponent<PlayerController>();

            _isOccupied = true;

            
            playerController.ToggleCharacterActive();

            playerController.SwitchActiveCam(_peekCam);

            playerController.GetInteractEvent().AddListener(ExitBarrel);
        }
    }

    private void MovePlayer(GameObject playerObject, Transform destination)
    {
        playerObject.transform.position = destination.position;
        playerObject.transform.rotation = destination.rotation;
    }

    void ExitBarrel()
    {
        if (_exitTransform != null)
        {
            MovePlayer(_player, _exitTransform);

            _isOccupied = false;
            PlayerController playerController = _player.GetComponent<PlayerController>();

            playerController.ToggleCharacterActive();

            playerController.SwitchActiveCam(_peekCam);

            playerController.GetInteractEvent().RemoveListener(ExitBarrel);
            _player = null;
        }
        else
        {
            Debug.Log(gameObject.name + " missing exit transform!");
        }
    }
}
