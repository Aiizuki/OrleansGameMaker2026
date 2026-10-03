using System;
using System.Collections.Generic;
using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractionHandler : MonoBehaviour
{
    [Header("Inputs")] [SerializeField] private InputActionReference _InteractAction;

    [Header("Order")] [SerializeField] private int _maxPickedIngredients = 5;

    public GameObject Interactible = null;
    private bool _isInteracting = false;

    // Commande prise au tableau (null tant que le joueur n'a pas de bon de commande)
    private Plat _currentOrder;

    // Ingrédients de la commande pas encore ramassés (un doublon = une entrée par exemplaire)
    private readonly List<Plat> _remainingIngredients = new List<Plat>();

    // Ingrédients ramassés pour la commande en cours (bons et mauvais)
    private readonly List<Plat> _inventory = new List<Plat>();

    // Plat terminé porté par le joueur (masqué dans la scène, il garde son état réussi / raté)
    private PreparedDish _finishedDish;

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
        UnityEventManager.AddListener(nameof(EnumUnityEventName.FlushPlayerInventory), FlushPlayerInventory);
    }

    private void OnOrderTaken(Plat plat)
    {
        _currentOrder = plat;
        ResetRemainingIngredients();
        _inventory.Clear();
    }

    private void OnOrderPrepared()
    {
        _currentOrder = null;
        _remainingIngredients.Clear();
        _inventory.Clear();
    }

    private void ResetRemainingIngredients()
    {
        _remainingIngredients.Clear();
        _remainingIngredients.AddRange(_currentOrder.Ingredients);
    }

    /// <summary>
    /// Replace tous les ingrédients ramassés dans le stockage. La commande reste en cours,
    /// mais repart de zéro (UI du bon et suivi de préparation remis à l'état initial).
    /// </summary>
    public void FlushPlayerInventory()
    {
        if (_currentOrder == null || _inventory.Count == 0)
        {
            Debug.Log("Inventaire vide : rien à replacer dans le stockage");
            return;
        }

        foreach (var ingredient in _inventory)
        {
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.ReturnToStorage), ingredient);
        }

        _finishedDish = null;
        _inventory.Clear();
        
        ResetRemainingIngredients();
        UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.InventoryFlushed), _currentOrder);
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

        if (Interactible.gameObject.CompareTag("OrderPreparer"))
        {
            Debug.Log("Interaction with OrderPreparer");
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.OrderPrepared));
        }

        if (Interactible.gameObject.CompareTag("Board"))
        {
            if (_currentOrder != null)
            {
                Debug.Log(
                    "Impossible de prendre un ticket : une commande est déjà en cours (" + _currentOrder.Nom + ")");
                return;
            }

            Debug.Log("Interact board");
            UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.TakeOrder));
        }
        else if (Interactible.gameObject.CompareTag("Product"))
        {
            if (_currentOrder == null)
            {
                Debug.Log("Impossible de prendre un produit sans bon de commande");
                return;
            }

            if (_inventory.Count >= _maxPickedIngredients)
            {
                Debug.Log("Impossible de prendre plus de " + _maxPickedIngredients + " ingrédients");
                return;
            }

            var stock = Interactible.gameObject.GetComponent<ProductStockHandler>();
            Plat selectedProduct = stock.TakeProduct();
            _inventory.Add(selectedProduct);
            Debug.Log("Récupération du produit" + selectedProduct.Nom);

            // Remove n'enlève qu'une occurrence : gère les ingrédients demandés plusieurs fois
            if (_remainingIngredients.Remove(selectedProduct))
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.IngredientCollected), selectedProduct);
            else
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.WrongIngredient), selectedProduct);

            // Carton vide détruit : OnTriggerExit ne sera pas appelé, on oublie l'interactible ici
            if (stock.Quantity <= 0)
                Interactible = null;
        }
        else if (Interactible.gameObject.CompareTag("Bin"))
        {
            Debug.Log("Interact bin");
            FlushPlayerInventory();
        }
        else if (Interactible.gameObject.CompareTag("OrderStation"))
        {
            if (_finishedDish != null)
                Debug.Log("Tu ne peux pas ramasser un plat, tu en as déjà un !");
            else if (Interactible.GetComponent<WaitingStation>().HasObjectWaiting)
            {
                GameObject dish = Interactible.GetComponent<WaitingStation>().Take();
                _finishedDish = dish.GetComponent<PreparedDish>();
                dish.SetActive(false);
                Debug.Log($"Tu as ramassé {_finishedDish.Plat.Nom}");
            }
        }
        else if (Interactible.gameObject.CompareTag("OrderDeposit"))
        {
            if (_finishedDish == null)
                Debug.Log("Tu n'as pas de plat à déposer !");
            else
            {
                Debug.Log($"Tu as déposé {_finishedDish.Plat.Nom} !");
                Interactible.GetComponent<OrderFinisher>().Deposit(_finishedDish);
                Destroy(_finishedDish.gameObject);
                _finishedDish = null;
            }
        }
    }

    private void RevokeEvents()
        {
            _InteractAction.action.performed -= Interact;
            UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.OrderPrepared), OnOrderPrepared);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.FlushPlayerInventory), FlushPlayerInventory);
        }
    }