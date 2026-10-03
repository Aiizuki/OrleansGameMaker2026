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

    // Commande prise au tableau (null tant que le joueur n'a pas de bon de commande)
    private Plat _currentOrder;

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
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.OrderPrepared), OnOrderPrepared);
    }

    private void OnOrderTaken(Plat plat)
    {
        _currentOrder = plat;
    }

    private void OnOrderPrepared()
    {
        _currentOrder = null;
    }

    private void OnEnable()
    {
        _InteractAction.action.Enable();
    }

    private void Interact(InputAction.CallbackContext obj)
    {
        if (Interactible == null)
        {
            Debug.Log("Nothing to interact with");
            return;
        }

        if (Interactible.gameObject.CompareTag("Board"))
        {
            if (_currentOrder != null)
            {
                Debug.Log("Impossible de prendre un ticket : une commande est déjà en cours (" + _currentOrder.Nom + ")");
                return;
            }

            Debug.Log("Interact board");
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.TakeOrder));
        }
        else if  (Interactible.gameObject.CompareTag("Product"))
        {
            if (_currentOrder == null)
            {
                Debug.Log("Impossible de prendre un produit sans bon de commande");
                return;
            }

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
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.OrderPrepared), OnOrderPrepared);
    }
}