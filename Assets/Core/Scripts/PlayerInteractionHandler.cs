using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference _InteractAction;

    public GameObject Interactible = null; 
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
        if (Interactible.gameObject.CompareTag("Board"))
        {
            Debug.Log("Interact board");
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.TakeOrder));
        }
    }

    private void RevokeEvents()
    {
        _InteractAction.action.performed -= Interact;
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
