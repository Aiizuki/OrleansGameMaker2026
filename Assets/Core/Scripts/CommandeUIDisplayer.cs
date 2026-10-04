using System.Collections.Generic;
using Core.Enums;
using Core.Scripts;
using TMPro;
using UnityEngine;

public class CommandeUIDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI platName;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject ingredientPanel;
    [SerializeField] private Color wrongIngredientColor = Color.red;
    
    private CanvasGroup _canvasGroup;

    // Lignes du bon pas encore rayées (une entrée par exemplaire d'ingrédient)
    private readonly List<(Plat ingredient, TextMeshProUGUI text)> _pendingIngredients = new List<(Plat, TextMeshProUGUI)>();

    void Awake()
    {
        if (!TryGetComponent(out _canvasGroup))
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        SetVisible(false);

        InitEvents();
    }

    private void SetVisible(bool visible)
    {
        _canvasGroup.alpha = visible ? 1f : 0f;
        _canvasGroup.interactable = visible;
        _canvasGroup.blocksRaycasts = visible;
    }

    private void OnDestroy()
    {
        CancelEvents();
    }

    private void RefreshOrderDisplayed(Plat plat)
    {
        platName.text = plat.Nom;
        
        ClearIngredients();

        foreach (var ingredient in plat.Ingredients)
        {
            var ingredientUI = Instantiate(ingredientPrefab, ingredientPanel.transform);
            var ingredientText = ingredientUI.GetComponent<TextMeshProUGUI>();
            ingredientText.text = ingredient.Nom;
            _pendingIngredients.Add((ingredient, ingredientText));
        }

        SetVisible(true);
    }

    private void HideOrder()
    {
        SetVisible(false);
        platName.text = string.Empty;
        ClearIngredients();
    }

    private void ClearIngredients()
    {
        _pendingIngredients.Clear();
        foreach (Transform child in ingredientPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private void StrikeIngredient(Plat ingredient)
    {
        int index = _pendingIngredients.FindIndex(x => x.ingredient == ingredient);
        if (index < 0)
            return;

        // Retiré de la liste pour qu'un doublon raye la ligne suivante
        _pendingIngredients[index].text.text = "<s>" + ingredient.Nom + "</s>";
        _pendingIngredients.RemoveAt(index);
    }

    private void OnWrongIngredient(Plat ingredient)
    {
        // Même prefab que les ingrédients du bon, mais en rouge et hors de _pendingIngredients
        var ingredientUI = Instantiate(ingredientPrefab, ingredientPanel.transform);
        var ingredientText = ingredientUI.GetComponent<TextMeshProUGUI>();
        ingredientText.text = ingredient.Nom;
        ingredientText.color = wrongIngredientColor;

        ShowError(ingredient.Nom + " n'est pas dans la commande");
    }

    // TODO : afficher le message à l'écran plutôt que dans la console
    public void ShowError(string message)
    {
        Debug.LogWarning(message);
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.OrderTaken), RefreshOrderDisplayed);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.OrderPrepared), HideOrder);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.IngredientCollected), StrikeIngredient);
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.WrongIngredient), OnWrongIngredient);
        // Inventaire vidé : on réaffiche le bon sans aucune ligne rayée ni ingrédient en rouge
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.InventoryFlushed), RefreshOrderDisplayed);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.OrderTaken), RefreshOrderDisplayed);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.OrderPrepared), HideOrder);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.IngredientCollected), StrikeIngredient);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.WrongIngredient), OnWrongIngredient);
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.InventoryFlushed), RefreshOrderDisplayed);
    }

    #endregion UnityEvents
}
