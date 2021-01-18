using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //selection
    [SerializeField] private Material highlightMaterial;
    private Material defaultMaterial;
    private Renderer _selectionRenderer;
    private Transform _selection;
    [SerializeField] private float _maxInsteractDistance = 2f;

    //camera
    [SerializeField] Camera _playerCam;
    [SerializeField] Transform _followTransform;


    //character controller
    private CharacterController controller;

    //input
    private InputMap _inputMap;

    //movement
    [SerializeField] private float _movementSpeed = 12f;
    [SerializeField] private float _jumpHeight = 10f;
    private bool _isMoving = false;

    //gravity
    private Vector3 _gravity = Physics.gravity;
    private Vector3 _velocity;

    //ground check
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    private float _groundDistance = 0.1f;
    private bool _isGrounded = false;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        controller = gameObject.GetComponent<CharacterController>();
        InitInput();
    }

    private void FixedUpdate()
    {
        #region Interaction

        RemoveHighlight();

        if (_playerCam.gameObject.activeSelf)
        {
            Ray ray = new Ray(transform.position, _playerCam.transform.forward * _maxInsteractDistance);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.distance <= _maxInsteractDistance && hit.transform.CompareTag("Selectable"))
                {
                    //show UI
                    //....
                    RemoveHighlight();
                    ApplyHighlight(hit);
                }
            }
        }

        #endregion

        #region movement

        IsGroundedCheck();

        if (_isMoving)
        {
            float x = _inputMap.Player.Movement.ReadValue<Vector2>().x;
            float z = _inputMap.Player.Movement.ReadValue<Vector2>().y;

            float camX = _followTransform.rotation.eulerAngles.x;

            //set player rotation to be the same as the follow target
            transform.rotation = Quaternion.Euler(0, _followTransform.rotation.eulerAngles.y, 0);

            //reset target rotation
            _followTransform.localEulerAngles = new Vector3(camX, _followTransform.rotation.y, _followTransform.rotation.z);

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * _movementSpeed * Time.deltaTime);
        }

        ApplyGravity();

        #endregion
    }

    private void Update()
    {
        
    }
    private void InitInput()
    {
        _inputMap = new InputMap();
        _inputMap.Player.Movement.performed += context => Move();
        _inputMap.Player.Jump.performed += context => Jump();
        _inputMap.Player.Interact.performed += context => Interact();
    }

    private void ApplyHighlight(RaycastHit hit)
    {
        _selection = hit.transform;

        _selectionRenderer = _selection.GetComponent<Renderer>();

        if (_selectionRenderer != null)
        {
            defaultMaterial = _selectionRenderer.material;
            _selectionRenderer.material = highlightMaterial;
        }
    }

    private void RemoveHighlight()
    {
        if (_selection != null)
        {
            _selectionRenderer.material = defaultMaterial;
            _selection = null;
            _selectionRenderer = null;
        }
    }

    public void ToggleCharacterActive()
    {
        if (GetComponent<CharacterController>().enabled)
        {
            GetComponent<CharacterController>().enabled = false;
            OnDisable();
        }
        else
        {
            GetComponent<CharacterController>().enabled = true;
            OnEnable();
        }
    }

    public void SwitchActiveCam(Camera otherCam)
    {
        _playerCam.gameObject.SetActive(!_playerCam.gameObject.activeSelf);
        otherCam.gameObject.SetActive(!otherCam.gameObject.activeSelf);
    }

    private void Move()
    {
        if (controller.enabled)
        {
            _isMoving = !_isMoving;
            //ground check
            //_isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
            /*
            if (_isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }
            */
            //movement
            //float x = _inputMap.Player.Movement.ReadValue<Vector2>().x;
            //float z = _inputMap.Player.Movement.ReadValue<Vector2>().y;
            //float x = direction.x;
            //float z = direction.y;

            //Vector3 move = transform.right * x + transform.forward * z;

            //controller.Move(move * _movementSpeed * Time.deltaTime);
        }
    }

    private void IsGroundedCheck()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
    }

    private void Jump()
    {
        IsGroundedCheck();

        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity.y);
        }
    }

    private void ApplyGravity()
    {
        if (controller.enabled)
        {
            _velocity += _gravity * Time.deltaTime;
            controller.Move(_velocity * Time.deltaTime);
        }

    }

    private void OnEnable()
    {
        _inputMap.Enable();
    }

    private void OnDisable()
    {
        _inputMap.Disable();
    }

    private void Interact()
    {
        Interactible interactible;
        if (_selection != null)
        {
            if ((interactible = _selection.GetComponent<Interactible>()) != null)
            {
                interactible.Interact(gameObject);
            }
        }
    }


}
