using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 100f;
    [SerializeField] private float _xRotClamp = 90f;
    [SerializeField] private Transform _followTarget;

    private float _xRotation = 0f;
    private float _yRotation = 0f;
    public float rotationPower = 3f;
    public float rotationLerp = 0.5f;

    //input
    private InputMap _inputMap;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Awake()
    {
        InitInput();
    }

    private void InitInput()
    {
        _inputMap = new InputMap();
        _inputMap.Player.Look.performed += context => FollowMouse(context.ReadValue<Vector2>());
    }

    private void FollowMouse(Vector2 mouseInput)
    {
        //we receive a Vector 2 where x is the horizontal and y the vertical movement of the mouse over time (delta)
        float moveX = mouseInput.x * _mouseSensitivity * Time.deltaTime;
        float moveY = mouseInput.y * _mouseSensitivity * Time.deltaTime;


        _xRotation = _xRotation + moveX;
        _yRotation = Mathf.Clamp(_yRotation + moveY, -_xRotClamp, _xRotClamp);

        _followTarget.gameObject.transform.Rotate(Vector3.up * _yRotation * Time.deltaTime);
        _followTarget.gameObject.transform.localRotation = Quaternion.Euler(_yRotation, _xRotation, 0f);
    }

    public void OnEnable()
    {
        _inputMap.Enable();
    }

    public void OnDisable()
    {
        _inputMap.Disable();
    }
}
