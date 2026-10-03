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

    private Plat _currentOrder;
    private int _collectedCount;
    private bool _hasWrongIngredient;

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
    }

    private void OnIngredientCollected(Plat ingredient)
    {
        _collectedCount++;
    }

    private void OnWrongIngredient(Plat ingredient)
    {
        _hasWrongIngredient = true;
    }

    private void PrepareOrder()
    {
        if (_currentOrder == null)
        {
            Debug.LogWarning("Aucune commande en cours : rien à préparer");
            return;
        }

        var dish = Instantiate(_preparedDishPrefab, _spawnPoint.position, _spawnPoint.rotation);

        // Appel direct sur l'instance pour ne pas recolorer les autres plats déjà posés
        bool success = !_hasWrongIngredient && _collectedCount == _currentOrder.Ingredients.Count;
        if (success)
            dish.SetSuccess();
        else
            dish.SetFailed();

        _currentOrder = null;
        _collectedCount = 0;
        _hasWrongIngredient = false;
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.IngredientCollected), OnIngredientCollected);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.WrongIngredient), OnWrongIngredient);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.OrderPrepared), PrepareOrder);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.OrderTaken), OnOrderTaken);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.IngredientCollected), OnIngredientCollected);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.WrongIngredient), OnWrongIngredient);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.OrderPrepared), PrepareOrder);
    }

    #endregion UnityEvents
}
