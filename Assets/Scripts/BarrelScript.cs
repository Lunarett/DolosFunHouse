using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class BarrelScript : Interactible
{
    private GameObject _player = null;
    private bool _isOccupied = false;

    [SerializeField] private Transform _exitTransform;
    [SerializeField] private Camera _peekCam;

    [SerializeField] private float _mouseSensitivity = 100f;
    [SerializeField] private float _lookAngle = 90f;


    private float _yRotation = 0f;

    public void Update()
    {
        if (_isOccupied)
        {
            CamFollowMouse();
        }
    }

    public override void Interact(GameObject playerObject)
    {
        base.Interact(playerObject);

        if (!_isOccupied)
        {
            EnterBarrel(playerObject);
        }
    }

    private void MovePlayer(GameObject playerObject, Transform destination)
    {
        playerObject.transform.position = destination.position;
        playerObject.transform.rotation = destination.rotation;
    }
    private void EnterBarrel(GameObject playerObject)
    {
        //move player character
        MovePlayer(playerObject, transform);
        _player = playerObject;
        PlayerController playerController = _player.GetComponent<PlayerController>();

        _isOccupied = true;

        //disable player control
        playerController.ToggleCharacterActive();

        playerController.SwitchActiveCam(_peekCam);
        //add an event listener so next time the player presses the interact button, they exit the barrel
        playerController.GetInteractEvent().AddListener(ExitBarrel);
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

    private void CamFollowMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity * Time.deltaTime;

        _yRotation = Mathf.Clamp(_yRotation + mouseX, -_lookAngle, _lookAngle);

        //transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        //_peekCam.gameObject.transform.Rotate(Vector3.up * _yRotation);
        _peekCam.gameObject.transform.localRotation = Quaternion.Euler(0f, _yRotation, 0f);
    }
}
