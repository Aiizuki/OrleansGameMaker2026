using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference _InteractAction;

    private bool _isInteracting = false;

    private void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        RevokeEvents();
    }

    private void InitEvents()
    {
        _InteractAction.action.performed += Interact;
    }

    private void OnEnable()
    {
        _InteractAction.action.Enable();
    }
    
    private void Interact(InputAction.CallbackContext obj)
    {
        
        //Do your thing 
    }

    private void RevokeEvents()
    {
        if (_InteractAction)
        {
            _InteractAction.action.performed -= Interact;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
