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
        if (Interactible != null)
        {
            if (Interactible.gameObject.CompareTag("Board"))
            {
                Debug.Log("Interact board");
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.TakeOrder));
            }
        }
        Debug.Log("Nothing to interact with");
    }

    private void RevokeEvents()
    {
        _InteractAction.action.performed -= Interact;
    }
}
