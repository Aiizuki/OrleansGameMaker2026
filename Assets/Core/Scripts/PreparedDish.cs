using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Plat préparé posé dans la scène : affiche le modèle FBX correspondant à son état (étape + échec).
/// Le modèle est instancié en enfant de _visualRoot (et non un simple swap de Mesh) pour garder
/// les matériaux, les sous-objets et la rotation / échelle d'import du FBX.
/// Les events DishSucceeded / DishFailed ne font que rafraîchir le modèle, sans changer l'état.
/// </summary>
public class PreparedDish : MonoBehaviour
{
    public Plat Plat;

    [Tooltip("Parent du modèle instancié (porte l'échelle du visuel). Utilise ce transform si vide.")]
    [SerializeField] private Transform _visualRoot;

    [Header("Effet de changement de modèle")]
    [Tooltip("Placeholder : à remplacer par le vrai effet woosh. Ne pas le mettre sous Visual.")]
    [SerializeField] private ParticleSystem _wooshParticles;

    // État propre à cette instance : Plat est un ScriptableObject partagé par toutes les commandes,
    // le modifier ferait échouer tous les plats suivants (et persisterait dans l'asset en éditeur)
    public bool IsFailed { get; private set; }
    public EnumDishStatus DishStatus { get; private set; } = EnumDishStatus.Raw;

    private bool _overFailed = false;

    private GameObject _currentModelPrefab;
    private GameObject _currentModelInstance;

    void Awake()
    {
        if (_visualRoot == null)
            _visualRoot = transform;

        InitEvents();
    }

    private void OnDestroy()
    {
        CancelEvents();
    }

    // ReSharper disable Unity.PerformanceAnalysis
    public void SetMesh()
    {
        if (Plat == null)
        {
            Debug.LogWarning($"[PreparedDish] Aucun Plat assigné sur {name} !");
            return;
        }

        GameObject modelPrefab = Plat.GetModelFromState(IsFailed, DishStatus, _overFailed);
        if (modelPrefab == null)
        {
            Debug.LogWarning($"[PreparedDish] Pas de modèle pour {Plat.Nom} (état {DishStatus}, raté : {IsFailed}) !");
            return;
        }

        // Les events DishSucceeded / DishFailed touchent tous les plats : on ne recrée que si le modèle change
        if (modelPrefab == _currentModelPrefab)
            return;

        bool hadModel = _currentModelInstance != null;
        if (hadModel)
            Destroy(_currentModelInstance);

        // worldPositionStays = false : garde la rotation / échelle locales d'import du FBX
        _currentModelInstance = Instantiate(modelPrefab, _visualRoot, false);
        _currentModelPrefab = modelPrefab;

        // Le BoxCollider racine gère les interactions : un collider dans le FBX les perturberait
        foreach (Collider modelCollider in _currentModelInstance.GetComponentsInChildren<Collider>())
            Destroy(modelCollider);

        // Pas de woosh à l'apparition du plat, seulement quand il change de modèle
        if (hadModel)
            PlayModelChangeEffect();
    }

    // L'échec / overfail ne change qu'à la fin d'une étape : FailDish (fin de la préparation par le joueur)
    // et ApplyStationResult (fin du travail d'une station). Une fois raté, le plat le reste :
    // un nouvel échec à une étape suivante (ou à la 1re station après une préparation ratée) donne l'overfail.

    /// <summary>Fin de la préparation par le joueur avec un mauvais ingrédient (compte comme un premier échec).</summary>
    public void FailDish()
    {
        MarkFailed();
        SetMesh();
    }

    /// <summary>
    /// Résultat du travail d'une station : change l'étape et l'échec en un seul rafraîchissement,
    /// pour ne pas afficher un modèle intermédiaire (ni jouer deux wooshs).
    /// </summary>
    public void ApplyStationResult(EnumDishStatus status, bool failed)
    {
        DishStatus = status;
        if (failed)
            MarkFailed();
        SetMesh();
    }

    private void MarkFailed()
    {
        if (IsFailed)
            _overFailed = true;
        else
            IsFailed = true;
    }

    private void PlayModelChangeEffect()
    {
        // TODO: brancher le vrai particle system de woosh
        if (_wooshParticles == null)
        {
            return;
        }
        _wooshParticles.Play();
        if (_wooshParticles.isPlaying)
        {
            Debug.Log("playing");
        }
        if (_wooshParticles.isStopped)
        {
            Debug.Log("stopped");
        }
        //_wooshParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
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
