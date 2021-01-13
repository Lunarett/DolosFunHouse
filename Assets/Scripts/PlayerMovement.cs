using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private CharacterController controller;

    //input map
    private InputMap _inputMap;

    //movement
    [SerializeField] private float _movementSpeed = 12f;
    [SerializeField] private float _jumpHeight = 2f;

    //gravity
    private Vector3 _gravity = Physics.gravity;
    private Vector3 _velocity;

    //ground check
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;

    private float _groundDistance = 0.1f;
    private bool _isGrounded;

    //moving camera
    [SerializeField] private float _mouseSensitivity = 100f;
    private float _xRotation = 0f;
    private Vector2 _look;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        Move();
        //FollowMouse();
    }

    private void InitInput()
    {
        _inputMap = new InputMap();
        _inputMap.Player.Movement.performed += context => Move();
    }

    private void Move()
    {
        if (controller.enabled)
        {
            //ground check
            _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

            if (_isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            //movement
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            controller.Move(move * _movementSpeed * Time.deltaTime);

            //jumping
            if (Input.GetButtonDown("Jump") && _isGrounded)
            {
                _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity.y);
            }

            //gravity
            _velocity += _gravity * Time.deltaTime;
            controller.Move(_velocity * Time.deltaTime);
        }
    }
}
