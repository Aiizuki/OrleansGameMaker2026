using Core.Enums;
using Core.Scripts;
using UnityEngine;

/// <summary>
/// Fait basculer le PNJ comme un punching ball quand le joueur le tape (event NpcWhacked) :
/// il part à l'opposé du joueur puis oscille autour de son pivot (les pieds) jusqu'à se redresser.
/// L'inclinaison est un ressort amorti appliqué en LateUpdate par-dessus la rotation du PNJ
/// (écrite par CleaningIAScript ou l'Animator), sans la modifier.
/// </summary>
public class PunchingBallWobble : MonoBehaviour
{
    [Tooltip("Objet qui bascule autour de son pivot. Utilise ce transform si vide.")]
    [SerializeField] private Transform _target;

    [Header("Ressort")]
    [Tooltip("Vitesse de bascule (degrés / seconde) donnée par un coup")]
    [SerializeField] private float _hitImpulse = 300f;
    [Tooltip("Raideur : plus elle est haute, plus l'oscillation est rapide")]
    [SerializeField] private float _stiffness = 120f;
    [Tooltip("Amortissement : plus il est haut, moins il y a d'allers-retours")]
    [SerializeField] private float _damping = 6f;
    [Tooltip("Inclinaison maximale (degrés), même en enchaînant les coups")]
    [SerializeField] private float _maxAngle = 35f;

    private const float RestThreshold = 0.05f;

    // Vecteur de rotation horizontal : direction = axe d'inclinaison, longueur = angle en degrés
    private Vector3 _tilt;
    private Vector3 _tiltVelocity;
    private bool _isWobbling;

    private Quaternion _baseRotation;
    private Quaternion _lastWrittenRotation;
    private bool _hasWritten;

    void Awake()
    {
        if (_target == null)
            _target = transform;

        UnityEventManager.AddListener<GameObject>(nameof(EnumUnityEventName.NpcWhacked), OnNpcWhacked);
    }

    private void OnDestroy()
    {
        UnityEventManager.RemoveListener<GameObject>(nameof(EnumUnityEventName.NpcWhacked), OnNpcWhacked);
    }

    private void OnNpcWhacked(GameObject npc)
    {
        if (npc != gameObject)
            return;

        // Le PNJ part à l'opposé du joueur
        GameObject player = GameObject.FindWithTag("Player");
        Vector3 hitDirection = player != null ? transform.position - player.transform.position : -transform.forward;
        hitDirection.y = 0f;
        if (hitDirection.sqrMagnitude < 0.0001f)
            hitDirection = -transform.forward;

        Vector3 tiltAxis = Vector3.Cross(Vector3.up, hitDirection.normalized);
        _tiltVelocity += tiltAxis * _hitImpulse;
        _isWobbling = true;
    }

    // LateUpdate : passe après l'Animator et après les rotations de CleaningIAScript
    void LateUpdate()
    {
        if (!_isWobbling)
            return;

        // Si la rotation n'est plus celle écrite à la frame précédente, quelqu'un l'a réécrite : c'est la nouvelle base.
        // Sinon on garde l'ancienne base, pour ne pas cumuler l'inclinaison d'une frame sur l'autre.
        Quaternion currentRotation = _target.rotation;
        if (!_hasWritten || currentRotation != _lastWrittenRotation)
            _baseRotation = currentRotation;

        UpdateSpring(Time.deltaTime);

        if (_tilt.magnitude < RestThreshold && _tiltVelocity.magnitude < RestThreshold)
        {
            _tilt = Vector3.zero;
            _tiltVelocity = Vector3.zero;
            _target.rotation = _baseRotation;
            _isWobbling = false;
            _hasWritten = false;
            return;
        }

        float angle = _tilt.magnitude;
        _lastWrittenRotation = Quaternion.AngleAxis(angle, _tilt / angle) * _baseRotation;
        _target.rotation = _lastWrittenRotation;
        _hasWritten = true;
    }

    private void UpdateSpring(float deltaTime)
    {
        Vector3 acceleration = -_stiffness * _tilt - _damping * _tiltVelocity;
        _tiltVelocity += acceleration * deltaTime;
        _tilt += _tiltVelocity * deltaTime;

        // Butée : on bloque l'angle et on annule la vitesse qui pousse au-delà
        if (_tilt.magnitude > _maxAngle)
        {
            Vector3 tiltDirection = _tilt.normalized;
            _tilt = tiltDirection * _maxAngle;
            float outwardSpeed = Vector3.Dot(_tiltVelocity, tiltDirection);
            if (outwardSpeed > 0f)
                _tiltVelocity -= tiltDirection * outwardSpeed;
        }
    }
}
