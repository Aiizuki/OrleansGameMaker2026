using Core.Enums;
using Core.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MovementScript : MonoBehaviour
{
    [Header("Inputs")] [SerializeField] private InputActionReference _moveLeftAction;
    [SerializeField] private Camera _camera;
    [SerializeField] private InputActionReference _moveRightAction;
    [SerializeField] private InputActionReference _moveForwardAction;
    [SerializeField] private InputActionReference _moveBackwardAction;
    [SerializeField] private Animator _animatorController;

    [SerializeField] private CoreGameSettings _settings;

    private Vector3 _moveDirection;

    // Bloqué pendant le décompte de début de partie, débloqué au StartTime
    private bool _canMove = false;

    private void Awake()
    {
        InitEvents();
    }

    void OnDestroy()
    {
        RevokeEvents();
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    private void EnableMovement()
    {
        _canMove = true;
    }

    private void HandleMovement()
    {
        _moveDirection = Vector3.zero;

        if (!_canMove)
        {
            gameObject.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
            _animatorController.SetBool("Walking", false);
            return;
        }

        // On lit l'état réel des touches à chaque frame plutôt que d'inverser un booléen à chaque
        // event "Press and Release" : un seul event perdu (spam, perte de focus) bloquait la direction
        if (_moveLeftAction.action.IsPressed())
        {
            _moveDirection.x -= 1;
        }

        if (_moveRightAction.action.IsPressed())
        {
            _moveDirection.x += 1;
        }

        if (_moveForwardAction.action.IsPressed())
        {
            _moveDirection.z += 1;
        }

        if (_moveBackwardAction.action.IsPressed())
        {
            _moveDirection.z -= 1;
        }

        _moveDirection.Normalize();
        Vector3 CorrectedDirectionVector = Quaternion.AngleAxis(_camera.transform.eulerAngles.y, Vector3.up) * new Vector3(_moveDirection.x, 0, _moveDirection.z).normalized;
        gameObject.GetComponent<Rigidbody>().linearVelocity = CorrectedDirectionVector * _settings.Speed;
        if (_moveDirection.sqrMagnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(CorrectedDirectionVector), 0.4f);
            _animatorController.SetBool("Walking", true);
        }
        else
        {
            _animatorController.SetBool("Walking", false);
        }
    }

    #region UnityEvents

    private void InitEvents()
    {
        UnityEventManager.AddListener(nameof(EnumUnityEventName.StartTime), EnableMovement);
    }

    private void RevokeEvents()
    {
        UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StartTime), EnableMovement);
    }

    #endregion UnityEvents
}
