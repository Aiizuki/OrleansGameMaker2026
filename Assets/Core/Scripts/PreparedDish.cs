using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Plat préparé posé dans la scène : affiche un matériau vert (succès) ou rouge (échec).
/// L'état peut être changé à tout moment via les events DishSucceeded / DishFailed.
/// </summary>
public class PreparedDish : MonoBehaviour
{
    [SerializeField] private Material _successMaterial;
    [SerializeField] private Material _failureMaterial;
    public Plat Plat;

    void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        CancelEvents();
    }

    public void SetSuccess()
    {
        ApplyMaterial(_successMaterial);
    }

    public void SetFailed()
    {
        ApplyMaterial(_failureMaterial);
    }

    public void SetState(EnumDishStatus status)
    {
        Plat.DishStatus = status;
    }

    public void FailDish()
    {
        Plat.IsFailed = true;
    }

    private void ApplyMaterial(Material material)
    {
        foreach (var meshRenderer in GetComponentsInChildren<Renderer>())
        {
            meshRenderer.sharedMaterial = material;
        }
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.DishSucceeded), SetSuccess);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.DishFailed), SetFailed);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.DishSucceeded), SetSuccess);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.DishFailed), SetFailed);
    }

    #endregion UnityEvents
}
