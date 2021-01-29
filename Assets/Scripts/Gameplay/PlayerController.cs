using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun, IPunObservable, IPunInstantiateMagicCallback
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
    [SerializeField] GameObject _virtualCam;
    [SerializeField] Transform _followTransform;
    [SerializeField] CameraController _playerCamController;


    //character controller
    private CharacterController _characterController;

    //input
    private InputMap _inputMap;
    private bool _isEnabled = true; //determines wether code accepts input

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

    //UI
    [SerializeField] PlayerUIHandler _playerUI;

    //test
    [SerializeField] private string myName;

    [SerializeField] private GameObject _skinsParent;

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        _playerCam.gameObject.SetActive(true);
        _virtualCam.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        _characterController = gameObject.GetComponent<CharacterController>();

        myName = photonView.Owner.NickName;

        InitInput();
    }

    private void Start()
    {
        if (photonView.IsMine)
        {
            int playerPref = PlayerPrefs.GetInt("selectedCharacter", 9);

            photonView.RPC("RPC_SetSkin", RpcTarget.AllBuffered, playerPref);
            Debug.Log("Found skin" + playerPref);
        }
    }
    [PunRPC]
    private void RPC_SetSkin(int index)
    {
        _skinsParent.transform.GetChild(index).gameObject.SetActive(true);
    }

    private void FixedUpdate()
    {
        if (!photonView.IsMine)
            return;
        if (_isEnabled)
        {
            #region Interaction

            RemoveHighlight();

            if (_playerCam.gameObject.activeSelf)
            {
                Ray ray = new Ray(_playerCam.transform.position, _followTransform.forward * _maxInsteractDistance);

                //ray cast will ignore everything on the player layer because of the ~
                if (Physics.Raycast(ray, out RaycastHit hit, _maxInsteractDistance, ~_playerLayer))
                {
                    if (hit.distance <= _maxInsteractDistance)
                    {
                        if (hit.transform.CompareTag("Selectable"))
                        {
                            ApplyHighlight(hit);
                        }
                        else if (hit.transform.CompareTag("Player"))
                        {
                            _playerUI.SelectedCrosshair();
                        }
                        else
                        {
                            _playerUI.NormalCrosshair();
                        }
                    }
                    else
                    {
                        _playerUI.NormalCrosshair();
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

                Vector3 xMove = new Vector3(_followTransform.right.x, 0, _followTransform.right.z) * x;
                Vector3 zMove = new Vector3(_followTransform.forward.x, 0, _followTransform.forward.z) * z;

                Vector3 move = xMove + zMove;

                if (_isSprinting)
                {
                    _characterController.Move(move * _movementSpeed * _sprintModifier * Time.deltaTime);

                    if (!_isJumping)
                    {
                        _animator.SetFloat("vertical", z * _sprintModifier);
                        _animator.SetFloat("horizontal", x * _sprintModifier);
                    }
                }
                else
                {
                    _characterController.Move(move * _movementSpeed * Time.deltaTime);

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

            StartApplyGravity();

            #endregion
        }
    }

    private void InitInput()
    {
        _inputMap = new InputMap();

        //is survivor
        _inputMap.Player.Movement.performed += context => Move();
        _inputMap.Player.Jump.performed += context => StartJump();
        _inputMap.Player.Interact.performed += context => StartInteract();
        _inputMap.Player.Sprint.performed += context => StartSprint();
        _inputMap.Player.Sprint.canceled += context => StartSprint();
        _inputMap.Player.Escape.performed += context => TestEscape();

        //is killer
        //...
        _inputMap.Killer.Kill.performed += context => Kill();
    }

    private void TestEscape()
    {
        Debug.Log("ESCAPE");
        _playerUI.OnGamePaused();
        ToggleCharacterActive();
    }

    private void ApplyHighlight(RaycastHit hit)
    {
        _selection = hit.transform;

        _selectionRenderer = _selection.GetComponent<Renderer>();

        if (_selectionRenderer != null)
        {
            _defaultMaterial = _selectionRenderer.material;
            _selectionRenderer.material = _highlightMaterial;

            _playerUI.SelectedCrosshair();
        }
    }

    private void RemoveHighlight()
    {
        if (_selection != null)
        {
            _selectionRenderer.material = _defaultMaterial;
            _selection = null;
            _selectionRenderer = null;

            _playerUI.NormalCrosshair();
        }
    }

    public void ToggleCharacterActive()
    {
        if (GetComponent<CharacterController>().enabled)
        {
            GetComponent<CharacterController>().enabled = false;
            OnDisable();
            _isEnabled = false;
        }
        else
        {
            GetComponent<CharacterController>().enabled = true;
            _playerCamController.OnEnable();
            _isEnabled = true; ;
        }
    }

    public void SwitchActiveCam(Camera otherCam)
    {
        _playerCam.gameObject.SetActive(!_playerCam.gameObject.activeSelf);
        otherCam.gameObject.SetActive(!otherCam.gameObject.activeSelf);
    }

    private void Move()
    {
        _isMoving = !_isMoving;
    }

    private void IsGroundedCheck()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded)
        {
            _isJumping = false;
        }
    }

    private void StartJump()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_Jump", RpcTarget.All);
        }
    }
    [PunRPC]
    private void RPC_Jump()
    {

        IsGroundedCheck();

        if (_isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity.y);
            _animator.SetTrigger("Jump");
            _isJumping = true;
        }

    }

    private void StartSprint()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_Sprint", RpcTarget.All);
        }
    }
    [PunRPC]
    private void RPC_Sprint()
    {
        _isSprinting = !_isSprinting;
    }

    private void StartApplyGravity()
    {
        if (photonView.IsMine)
            photonView.RPC("RPC_ApplyGravity", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_ApplyGravity()
    {
        if (photonView.IsMine)
        {
            _velocity += _gravity * Time.deltaTime;
            // probably causes an error
            _characterController.Move(_velocity * Time.deltaTime);

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


    private void StartInteract()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_Interact", RpcTarget.All);
        }
    }
    [PunRPC]
    private void RPC_Interact()
    {
        Interactible interactible;
        if (_selection != null)
        {
            if ((interactible = _selection.GetComponent<Interactible>()) != null)
            {
                interactible.StartInteract(photonView.ControllerActorNr);
            }
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }

    //here we give the PhotonPlayer the gameObject as TagObject so that we can get the player gameObject through the viewActorNumber later
    //some links:
    //https://forum.photonengine.com/discussion/12564/pun-2-onphotoninstantiate-isnt-being-called
    //https://forum.photonengine.com/discussion/6432/tagobject-usage
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.Sender.TagObject = gameObject;
    }

    private void Kill()
    {
        if (_playerCam.gameObject.activeSelf)
        {
            Ray ray = new Ray(_playerCam.transform.position, _followTransform.forward * _maxInsteractDistance);

            if (Physics.Raycast(ray, out RaycastHit hit, _maxInsteractDistance, _playerLayer))
            {
                if (hit.distance <= _maxInsteractDistance && hit.transform.CompareTag("Player"))
                {
                    Debug.Log("Kill Player");
                    //since we might hit different parts of the player we first go to the parent and then search it's children. 
                    //This way we also find the controller when it is in a sibling
                    PlayerController victimController = hit.transform.gameObject.GetComponent<PlayerController>();

                    //this is if we hit the player from the front, the ray will hit the physical interaction zone
                    if (victimController == null)
                    {
                        victimController = hit.transform.GetComponentInParent<PlayerController>();
                    }

                    if (victimController != this)
                    {
                        victimController.StartDie();
                    }
                }
            }
        }
    }

    public void StartDie()
    {

        photonView.RPC("RPC_Die", RpcTarget.All);
    }
    [PunRPC]
    private void RPC_Die()
    {

        Debug.Log("Oh no I just died!!!");
    }
}
