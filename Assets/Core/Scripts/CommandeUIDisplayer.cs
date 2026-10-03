using Core.Enums;
using Core.Scripts;
using TMPro;
using UnityEngine;

public class CommandeUIDisplayer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI platName;
    [SerializeField] private GameObject ingredientPrefab;
    [SerializeField] private GameObject ingredientPanel;

    void Awake()
    {
        InitEvents();
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

        if(!gameObject.activeSelf)
        gameObject.SetActive(true);
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener<Plat>(nameof(EnumUnityEventName.RefreshOrderUI), RefreshOrderDisplayed);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener<Plat>(nameof(EnumUnityEventName.RefreshOrderUI), RefreshOrderDisplayed);
    }

    #endregion UnityEvents
}
