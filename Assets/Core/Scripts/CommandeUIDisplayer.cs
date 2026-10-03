using Core.Enums;
using Core.Scripts;
using TMPro;
using UnityEngine;

public class CommandeUIDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI platName;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject ingredientPanel;

    // Le GameObject doit rester actif pour écouter les events : on masque l'UI via le CanvasGroup
    private CanvasGroup _canvasGroup;

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
        
        foreach (Transform child in ingredientPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var ingredient in plat.Ingredients)
        {
            var ingredientUI = Instantiate(ingredientPrefab, ingredientPanel.transform);
            ingredientUI.GetComponent<TextMeshProUGUI>().text = ingredient.Nom;
        }

        SetVisible(true);
    }

    private void HideOrder()
    {
        SetVisible(false);
        platName.text = string.Empty;

        foreach (Transform child in ingredientPanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.OrderTaken), RefreshOrderDisplayed);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.OrderPrepared), HideOrder);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.OrderTaken), RefreshOrderDisplayed);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.OrderPrepared), HideOrder);
    }

    #endregion UnityEvents
}
