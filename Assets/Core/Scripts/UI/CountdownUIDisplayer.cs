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

    void Awake()
    {
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

        if (remaining > 0)
        {
            _countdownText.text = remaining.ToString();
            return;
        }

        _countdownText.text = _goText;
        StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(_goDisplayDuration);
        _countdownText.gameObject.SetActive(false);
    }
}
