using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //based on this tutorial https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys

    [SerializeField] private CharacterController controller;

    //movement

    [SerializeField] private float _movementSpeed = 12f;
    [SerializeField] private float _sprintingSpeed = 16f;
    [SerializeField] private float _jumpHeight = 2f;

    //gravity

    private Vector3 _gravity = Physics.gravity;

    private Vector3 _velocity;

    //ground check
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundMask;

    private float _groundDistance = 0.1f;
    private bool _isGrounded;

    [SerializeField] private Animator _animator;

    public void Update()
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

        //sprinting
        if (Input.GetButton("Sprint") && _isGrounded)
        {
            controller.Move(move * _sprintingSpeed * Time.deltaTime);
            _animator.SetFloat("vertical", z * 2);
            _animator.SetFloat("horizontal", x * 2);
        }
        else
        {
            controller.Move(move * _movementSpeed * Time.deltaTime);
            _animator.SetFloat("vertical", z);
            _animator.SetFloat("horizontal", x);
        }

        //jumping
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity.y);
            _animator.SetTrigger("Jump");
        }

        //gravity
        _velocity += _gravity * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);
    }
}
