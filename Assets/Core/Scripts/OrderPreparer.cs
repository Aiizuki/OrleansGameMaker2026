using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Suit les ingrédients ramassés pour la commande en cours et, à OrderPrepared,
/// instancie le plat préparé en succès ou en échec.
/// </summary>
public class OrderPreparer : MonoBehaviour
{
    [SerializeField] private PreparedDish _preparedDishPrefab;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private GameObject _waitingStation;

    private Plat _currentOrder;
    private int _collectedCount;
    private bool _hasWrongIngredient;
    private bool _orderReady = false;

    void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        CancelEvents();
    }

    private void OnOrderTaken(Plat plat)
    {
        _currentOrder = plat;
        _collectedCount = 0;
        _hasWrongIngredient = false;
        GetComponent<Outliner>().HideOutline();
    }

    private void OnIngredientCollected(Plat ingredient)
    {
        _collectedCount++;
        if (!_hasWrongIngredient && _collectedCount == _currentOrder.Ingredients.Count)
        {
            GetComponent<Outliner>().ShowOutline();
        }
        
    }

    private void OnWrongIngredient(Plat ingredient)
    {
        _hasWrongIngredient = true;
        UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.HighlightBin));
    }

    private void PrepareOrder()
    {
        if (_currentOrder == null)
        {
            Debug.LogWarning("Aucune commande en cours : rien à préparer");
            return;
        }

        var dish = Instantiate(_preparedDishPrefab, _spawnPoint.position, _spawnPoint.rotation);
        // Avant SetMesh / FailDish : le modèle affiché dépend du Plat
        dish.Plat = _currentOrder;

        // Appel direct sur l'instance pour ne pas modifier les autres plats déjà posés
        bool success = !_hasWrongIngredient && _collectedCount == _currentOrder.Ingredients.Count;
        if (success)
            dish.SetMesh();
        else
            dish.FailDish();

        _waitingStation.GetComponent<WaitingStation>().AddObjectToWaiting(dish.gameObject);
        
        GetComponent<Outliner>().HideOutline();
        _currentOrder = null;
        _collectedCount = 0;
        _hasWrongIngredient = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = this.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerInteractionHandler>().Interactible = null;
        }
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.IngredientCollected), OnIngredientCollected);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.WrongIngredient), OnWrongIngredient);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.OrderPrepared), PrepareOrder);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.InventoryFlushed), OnOrderTaken);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.IngredientCollected), OnIngredientCollected);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.WrongIngredient), OnWrongIngredient);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.OrderPrepared), PrepareOrder);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.InventoryFlushed), OnOrderTaken);
    }

    #endregion UnityEvents
}
