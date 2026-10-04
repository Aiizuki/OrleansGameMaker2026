using System.Collections.Generic;
using System.Linq;
using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Instancie le joueur à la position définie dans CoreGameSettings.
/// À chaque GameStart, l'ancien joueur est détruit et un nouveau est instancié.
/// Compte les plats servis et les PNJ tapés, et les enregistre (avec le chrono du TimeManager) au GameOver.
/// </summary>
public class GameMaster : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private CoreGameSettings _settings;
    [SerializeField] private TimeManager _timeManager;

    private GameObject _playerInstance;

    private readonly Dictionary<Plat, DishStat> _dishStats = new Dictionary<Plat, DishStat>();
    private int _npcWhackCount;
    // GameOver peut être déclenché plusieurs fois (AngrySliderHandler) : on n'enregistre qu'une fois
    private bool _statsSaved;

    void Awake()
    {
        InitEvents();
    }

    private void OnDestroy()
    {
        CancelEvents();
    }

    private void SpawnPlayer()
    {
        if (_player == null || _settings == null)
        {
            Debug.LogError($"[GameMaster] Prefab du joueur ou CoreGameSettings non assigné sur {name} !");
            return;
        }

        // Un joueur déjà présent (instance précédente ou posé à la main dans la scène) est remplacé
        if (_playerInstance == null)
            _playerInstance = GameObject.FindWithTag("Player");
        if (_playerInstance != null)
            Destroy(_playerInstance);

        _playerInstance = Instantiate(_player, _settings.SpawnPosition, Quaternion.identity);
    }

    #region Stats

    private void ResetStats()
    {
        _dishStats.Clear();
        _npcWhackCount = 0;
        _statsSaved = false;
    }

    private void OnDishServed(PreparedDish dish)
    {
        if (dish == null || dish.Plat == null)
            return;

        if (!_dishStats.TryGetValue(dish.Plat, out DishStat stat))
        {
            stat = new DishStat { DishName = dish.Plat.Nom };
            _dishStats.Add(dish.Plat, stat);
        }

        // Même condition que OrderFinisher.Deposit
        if (dish.IsFailed || dish.DishStatus != EnumDishStatus.Dressed)
            stat.FailCount++;
        else
            stat.SuccessCount++;
    }

    private void OnNpcWhacked(GameObject npc)
    {
        _npcWhackCount++;
    }

    private void SaveStats()
    {
        if (_statsSaved)
            return;
        _statsSaved = true;

        if (_timeManager == null)
            Debug.LogWarning($"[GameMaster] TimeManager non assigné sur {name} : chrono enregistré à 0 !");

        GameStatsFile.Save(new GameStats
        {
            Dishes = _dishStats.Values.ToList(),
            NpcWhackCount = _npcWhackCount,
            ElapsedTime = _timeManager != null ? _timeManager.ElapsedTime : 0f,
        });
    }

    #endregion Stats

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.GameStart), SpawnPlayer);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.GameStart), ResetStats);
        UnityEventManager.AddListener<PreparedDish>(nameof(EnumUnityEventName.DishServed), OnDishServed);
        UnityEventManager.AddListener<GameObject>(nameof(EnumUnityEventName.NpcWhacked), OnNpcWhacked);
        UnityEventManager.AddListener(nameof(EnumUnityEventName.GameOver), SaveStats);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameStart), SpawnPlayer);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameStart), ResetStats);
        UnityEventManager.RemoveListener<PreparedDish>(nameof(EnumUnityEventName.DishServed), OnDishServed);
        UnityEventManager.RemoveListener<GameObject>(nameof(EnumUnityEventName.NpcWhacked), OnNpcWhacked);
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameOver), SaveStats);
    }

    #endregion UnityEvents
}
