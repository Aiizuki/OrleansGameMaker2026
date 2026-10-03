using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementScript : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference _moveLeftAction;
    [SerializeField] private InputActionReference _moveRightAction;
    [SerializeField] private InputActionReference _moveForwardAction;
    [SerializeField] private InputActionReference _moveBackwardAction;

    public int speed;
    
    private Vector3 _moveDirection;
    
    private bool _isMovingLeft = false;
    private bool _isMovingRight = false;
    private bool _isMovingForward = false;
    private bool _isMovingBackward = false;

    private void Awake()
    {
        var actionMap =  InputSystem.actions.FindActionMap("Inpute");
        InitEvents();
    }

    private void InitEvents()
    {
        _moveLeftAction.action.performed += MoveLeft;
        _moveRightAction.action.performed += MoveRight;
        _moveForwardAction.action.performed += MoveForward;
        _moveBackwardAction.action.performed += MoveBackward;
    }

    void OnEnable()
    {
        _moveLeftAction.action.Enable();
        _moveRightAction.action.Enable();
        _moveForwardAction.action.Enable();
        _moveBackwardAction.action.Enable();
    }

    private void MoveLeft(InputAction.CallbackContext obj)
    {
        _isMovingLeft = !_isMovingLeft;
    }

    private void MoveRight(InputAction.CallbackContext obj)
    {
        _isMovingRight = !_isMovingRight;
    }

    private void MoveForward(InputAction.CallbackContext obj)
    {
        _isMovingForward = !_isMovingForward;
    }

    private void MoveBackward(InputAction.CallbackContext obj)
    {
        _isMovingBackward = !_isMovingBackward;
    }

    private void RevokeEvents()
    {
        _moveLeftAction.action.performed -= MoveLeft;
        _moveRightAction.action.performed -= MoveRight;
        _moveForwardAction.action.performed -= MoveForward;
        _moveBackwardAction.action.performed -= MoveBackward;
    }

    private void HandleMovement()
    {
        _moveDirection = Vector3.zero;
        if (_isMovingLeft)
        {
            _moveDirection.x += 1;
        }
        if (_isMovingRight)
        {
            _moveDirection.x -= 1;
        }
        if (_isMovingForward)
        {
            _moveDirection.z -= 1;
        }
        if (_isMovingBackward) 
        {
            _moveDirection.z += 1;
        }
        _moveDirection.Normalize();
        gameObject.GetComponent<Rigidbody>().linearVelocity = _moveDirection * speed;
        
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        HandleMovement();
    }
}
