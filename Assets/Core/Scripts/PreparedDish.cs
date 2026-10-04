using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Plat préparé posé dans la scène : affiche un matériau vert (succès) ou rouge (échec).
/// L'état peut être changé à tout moment via les events DishSucceeded / DishFailed.
/// </summary>
public class PreparedDish : MonoBehaviour
{
    public Plat Plat;

    // État propre à cette instance : Plat est un ScriptableObject partagé par toutes les commandes,
    // le modifier ferait échouer tous les plats suivants (et persisterait dans l'asset en éditeur)
    public bool IsFailed { get; private set; }
    public EnumDishStatus DishStatus { get; private set; } = EnumDishStatus.Raw;

    private bool _overFailed = false;

    void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        CancelEvents();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetMesh()
    {
        gameObject.GetComponent<MeshFilter>().mesh =  Plat.GetMeshFromState(IsFailed, DishStatus, _overFailed);
    }

    public void SetState(EnumDishStatus status)
    {
        DishStatus = status;
    }

    public void FailDish()
    {
        if (IsFailed)
            _overFailed = true;
        else
            IsFailed = true;
        SetMesh();
    }

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.DishSucceeded), SetMesh);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.DishFailed), SetMesh);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.DishSucceeded), SetMesh);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.DishFailed), SetMesh);
    }

    #endregion UnityEvents
}
