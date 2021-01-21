using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float _mouseSensitivity = 100f;
    [SerializeField] private float _xRotClamp = 90f;
    [SerializeField] private Transform _followTarget;

    //input
    private InputMap _inputMap;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
        //we receive a Vector2 where x is the horizontal and y the vertical movement of the mouse over time (delta)
        float moveX = mouseInput.x * _mouseSensitivity * Time.deltaTime;
        float moveY = -mouseInput.y * _mouseSensitivity * Time.deltaTime;

        Vector3 rotation = _followTarget.eulerAngles + new Vector3(moveY, moveX, 0f);

        rotation.x = ClampAngle(rotation.x, -_xRotClamp, _xRotClamp);

        _followTarget.eulerAngles = rotation;
    }

    public void OnEnable()
    {
        _inputMap.Enable();
    }

    public void OnDisable()
    {
        _inputMap.Disable();
    }

    //source: https://answers.unity.com/questions/659932/how-do-i-clamp-my-rotation.html
    float ClampAngle(float angle, float from, float to)
    {
        if (angle < 0f)
        {
            angle += 360;
        }

        if (angle > 180f)
        {
            return Mathf.Max(angle, 360 + from);
        }

        return Mathf.Min(angle, to);
    }
}
