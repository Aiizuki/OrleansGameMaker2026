using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Inputs")] [SerializeField] private InputActionReference _InteractAction;

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
        if (Interactible is null)
        {
            Debug.Log("Nothing to interact with");
            return;
        }

        if (Interactible.gameObject.CompareTag("Board"))
        {
            Debug.Log("Interact board");
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.TakeOrder));
        }
        else if  (Interactible.gameObject.CompareTag("Product"))
        {
            var stock = Interactible.gameObject.GetComponent<ProductStockHandler>();
            Plat selectedProduct = stock.TakeProduct();
            Debug.Log("Récupération du produit" + selectedProduct.Nom);

            // Carton vide détruit : OnTriggerExit ne sera pas appelé, on oublie l'interactible ici
            if (stock.Quantity <= 0)
                Interactible = null;
            
            // TODO : ajouter le produit dans l'inventaire du joueur
        }
    }

    private void RevokeEvents()
    {
        _InteractAction.action.performed -= Interact;
    }
}