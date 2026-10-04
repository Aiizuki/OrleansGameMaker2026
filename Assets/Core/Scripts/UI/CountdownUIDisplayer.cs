using System.Collections;
using Core.Enums;
using Core.Scripts;
using TMPro;
using UnityEngine;

/// <summary>
/// Affiche le décompte de début de partie (3, 2, 1, GO!) à partir de l'event CountdownTick.
/// </summary>
public class CountdownUIDisplayer : MonoBehaviour
{
    [Tooltip("Texte TMP simple (UI > Text - TextMeshPro), pas le texte d'un InputField qui le réécrit à chaque frame")]
    [SerializeField] private TextMeshProUGUI _countdownText;
    [SerializeField] private string _goText = "GO!";
    [Tooltip("Durée (en secondes) d'affichage du texte GO avant de le masquer")]
    [SerializeField] private float _goDisplayDuration = 0.8f;

    [Header("Zoom out à chaque changement")]
    [Tooltip("Échelle de départ du texte, relative à son échelle de base")]
    [SerializeField] private float _zoomStartScale = 1.3f;
    [Tooltip("Durée (en secondes) du retour à l'échelle de base")]
    [SerializeField] private float _zoomDuration = 0.3f;

    private Vector3 _baseScale;
    private Coroutine _zoomRoutine;

    void Awake()
    {
        _baseScale = _countdownText.transform.localScale;
        _countdownText.gameObject.SetActive(false);
        UnityEventManager.AddListener<int>(nameof(EnumUnityEventName.CountdownTick), OnCountdownTick);
    }

    private void OnDestroy()
    {
        UnityEventManager.RemoveListener<int>(nameof(EnumUnityEventName.CountdownTick), OnCountdownTick);
    }

    private void OnCountdownTick(int remaining)
    {
        _countdownText.gameObject.SetActive(true);
        PlayZoomOut();

        if (remaining > 0)
        {
            _countdownText.text = remaining.ToString();
            return;
        }

        _countdownText.text = _goText;
        StartCoroutine(HideAfterDelay());
    }

    private void PlayZoomOut()
    {
        // Un nouveau changement relance le zoom depuis le début
        if (_zoomRoutine != null)
            StopCoroutine(_zoomRoutine);
        _zoomRoutine = StartCoroutine(ZoomOutRoutine());
    }

    private IEnumerator ZoomOutRoutine()
    {
        Vector3 startScale = _baseScale * _zoomStartScale;
        float elapsed = 0f;

        while (elapsed < _zoomDuration)
        {
            elapsed += Time.deltaTime;
            // Ease-out : rétrécit vite au début puis ralentit
            float t = 1f - Mathf.Pow(1f - Mathf.Clamp01(elapsed / _zoomDuration), 2f);
            _countdownText.transform.localScale = Vector3.Lerp(startScale, _baseScale, t);
            yield return null;
        }

        _countdownText.transform.localScale = _baseScale;
        _zoomRoutine = null;
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_goDisplayDuration);
        _countdownText.gameObject.SetActive(false);
    }
}
