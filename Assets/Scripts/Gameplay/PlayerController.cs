using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerController : MonoBehaviour
{
    //selection
    [SerializeField] private Material _highlightMaterial;
    private Material _defaultMaterial;
    private Renderer _selectionRenderer;
    private Transform _selection;
    [SerializeField] private float _maxInsteractDistance = 2f;
    [SerializeField] private LayerMask _playerLayer;

    //camera
    [SerializeField] Camera _playerCam;
    [SerializeField] Transform _followTransform;


    //character controller
    private CharacterController controller;

    //input
    private InputMap _inputMap;

    //movement
    [SerializeField] private float _movementSpeed = 8f;
    [SerializeField] private float _jumpHeight = 2;
    private bool _isMoving = false;
    private bool _isSprinting = false;
    private bool _isJumping = false;
    private float _sprintModifier = 2;

    //gravity
    private Vector3 _gravity = Physics.gravity;
    private Vector3 _velocity;

    //ground check
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;
    private float _groundDistance = 0.1f;
    private bool _isGrounded = false;

    //animation
    [SerializeField] private Animator _animator;

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
            //Vector3 shoulderOffset = new Vector3(_followTransform.localPosition.x + 0.5f, 0, 0);

            //Ray ray = new Ray(_followTransform.position, _followTransform.forward * _maxInsteractDistance);
            Ray ray = new Ray(_playerCam.transform.position, _followTransform.forward * _maxInsteractDistance);

            //ray cast will ignore everything on the player layer because of the ~
            if (Physics.Raycast(ray, out RaycastHit hit, _maxInsteractDistance, ~_playerLayer))
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
            _followTransform.localEulerAngles = new Vector3(camX, 0, 0);


            Vector3 move = _followTransform.right * x + _followTransform.forward * z;


            if (_isSprinting)
            {
                controller.Move(move * _movementSpeed * _sprintModifier * Time.deltaTime);

                if (!_isJumping)
                {
                    _animator.SetFloat("vertical", z * _sprintModifier);
                    _animator.SetFloat("horizontal", x * _sprintModifier);
                }

            }
            else
            {
                controller.Move(move * _movementSpeed * Time.deltaTime);

                if (!_isJumping)
                {

                    _animator.SetFloat("vertical", z);
                    _animator.SetFloat("horizontal", x);
                }
            }
        }

        else
        {
            _animator.SetFloat("vertical", 0);
            _animator.SetFloat("horizontal", 0);
        }

        ApplyGravity();

        #endregion
    }

    private void InitInput()
    {
        _inputMap = new InputMap();
        _inputMap.Player.Movement.performed += context => Move();
        _inputMap.Player.Jump.performed += context => Jump();
        _inputMap.Player.Interact.performed += context => Interact();
        _inputMap.Player.Sprint.performed += context => Sprint();
        _inputMap.Player.Sprint.canceled += context => Sprint();
    }

    private void ApplyHighlight(RaycastHit hit)
    {
        _selection = hit.transform;

        _selectionRenderer = _selection.GetComponent<Renderer>();

        if (_selectionRenderer != null)
        {
            _defaultMaterial = _selectionRenderer.material;
            _selectionRenderer.material = _highlightMaterial;
        }
    }

    private void RemoveHighlight()
    {
        if (_selection != null)
        {
            _selectionRenderer.material = _defaultMaterial;
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
        }
    }

    private void IsGroundedCheck()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded)
        {
            _isJumping = false;
        }
    }

    private void Jump()
    {
        IsGroundedCheck();

        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity.y);
            _animator.SetTrigger("Jump");
            _isJumping = true;
        }
    }

    private void Sprint()
    {
        _isSprinting = !_isSprinting;
    }

    private void ApplyGravity()
    {
        if (controller.enabled)
        {
            _velocity += _gravity * Time.deltaTime;
            controller.Move(_velocity * Time.deltaTime);

            if (_isGrounded && _velocity.y < 0)
            {
                _velocity.y = 0;
            }
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

    public void OnDrawGizmos()
    {
        //Ray ray = new Ray(_followTransform.position, _followTransform.forward * _maxInsteractDistance);

        //Vector3 shoulderOffset = new Vector3(_followTransform.localPosition.x + 0.5f, 0, 0);

        Ray ray = new Ray(_playerCam.transform.position, _followTransform.forward * _maxInsteractDistance);

        Gizmos.DrawRay(ray);
    }
}
