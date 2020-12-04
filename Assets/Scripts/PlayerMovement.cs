using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //based on this tutorial https://www.youtube.com/watch?v=_QajrabyTJc&ab_channel=Brackeys

    [SerializeField] private CharacterController controller;

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
