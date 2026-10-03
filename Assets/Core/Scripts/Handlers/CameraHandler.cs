using System;
using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.Events;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Transform _spawnPosKitchen;
    [SerializeField] private Transform _spawnPosStorage;
    [Tooltip("Temps approximatif (en secondes) pour atteindre la nouvelle position")]
    [SerializeField] private float _smoothTime = 0.3f;

    // If true, means that the camera is at _spawnPosStorage
    private bool _switched;

    // Position vers laquelle la caméra se déplace
    private Vector3 _targetPosition;
    private Vector3 _velocity;

    void Awake()
    {
        InitEvents();
    }

    private void Start()
    {
        transform.position = _spawnPosKitchen.position;
        _targetPosition = transform.position;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, _smoothTime);
    }

    void OnDestroy()
    {
        RevokeEvents();
    }

    /// <summary>
    /// Switches the camera between rooms
    /// </summary>
    /// <param name="playerInStorage">If true, player is in the storage</param>
    private void Switch(bool playerInStorage)
    {
        switch (playerInStorage)
        {
            case true when !_switched:
                _targetPosition = _spawnPosStorage.position;
                _switched = true;
                break;
            case false when _switched:
                _targetPosition = _spawnPosKitchen.position;
                _switched = false;
                break;
        }
    }

    #region UnityEvents

    private void InitEvents()
    {
        UnityEventManager.AddListener<bool>(nameof(EnumUnityEventName.SwitchCameraPosition), Switch);
    }

    private void RevokeEvents()
    {
        UnityEventManager.RemoveListener<bool>(nameof(EnumUnityEventName.SwitchCameraPosition), Switch);
    }

    #endregion UnityEvents
}