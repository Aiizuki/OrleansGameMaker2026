using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Instancie le joueur à la position définie dans CoreGameSettings.
/// À chaque GameStart, l'ancien joueur est détruit et un nouveau est instancié.
/// </summary>
public class GameMaster : MonoBehaviour
{
    [SerializeField] private GameObject _player;
    [SerializeField] private CoreGameSettings _settings;

    private GameObject _playerInstance;

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

    #region UnityEvents

    void InitEvents()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.GameStart), SpawnPlayer);
    }

    void CancelEvents()
    {
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameStart), SpawnPlayer);
    }

    #endregion UnityEvents
}
