using System.Collections;
using Core.Enums;
using FMODUnity;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Scripts
{
    public class AngrySliderHandler : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        [SerializeField] private TimeSettings _timerSettings;
        [SerializeField] private CoreGameSettings _globalSettings;

        [Header("Couleurs de la barre")] [SerializeField]
        private Color _warningColor = Color.yellow;

        [SerializeField] private Color _dangerColor = Color.red;

        [Tooltip("Remplissage (0-1) en dessous duquel la barre passe au jaune")] [SerializeField, Range(0f, 1f)]
        private float _warningThreshold = 0.5f;

        [Tooltip("Remplissage (0-1) en dessous duquel la barre passe au rouge")] [SerializeField, Range(0f, 1f)]
        private float _dangerThreshold = 0.2f;

        [Tooltip("Durée (en secondes) de la transition entre deux couleurs")] [SerializeField]
        private float _colorTransitionDuration = 0.5f;

        private Image _fillImage;
        private Color _normalColor;
        private Color _transitionStartColor;
        private Color _targetColor;
        private float _transitionTime;

        private Coroutine _sliderLifeRoutine;

        void Awake()
        {
            // La couleur de départ du Fill sert de couleur "normale"
            _fillImage = _slider.fillRect.GetComponent<Image>();
            _normalColor = _fillImage.color;
            _targetColor = _normalColor;
            _transitionTime = _colorTransitionDuration;

            InitEvents();
        }

        void Update()
        {
            UpdateFillColor();
        }

        void OnDestroy()
        {
            RevokeEvents();
        }

        private void StartSliderLifeRoutine()
        {
            _sliderLifeRoutine ??= StartCoroutine(SliderLife());
        }

        private void StopSlideRoutine()
        {
            if (_sliderLifeRoutine == null)
                return;

            StopCoroutine(_sliderLifeRoutine);
            _sliderLifeRoutine = null;
        }

        private void RaiseSlider()
        {
            // Slider.value est déjà clampé à maxValue
            _slider.value += _globalSettings.AngrySliderRaiseAmount;
        }

        private void DecreaseSlider()
        {
            _slider.value -= _globalSettings.AngrySliderDicreaseAmount;
            if (_slider.value <= 0f)
            {
                _slider.value = 0f;
                UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.GameOver));
            }
        }

        /// <summary>
        /// Fait baisser le slider en continu : il passe de plein à vide en angrySliderEmptyDuration secondes.
        /// </summary>
        private IEnumerator SliderLife()
        {
            while (true)
            {
                float decreasePerSecond =
                    (_slider.maxValue - _slider.minValue) / _timerSettings.AngrySliderEmptyDuration;
                _slider.value -= decreasePerSecond * Time.deltaTime;
                if (_slider.value <= 0f)
                    UnityEventManager.TriggerEvent(nameof(EnumUnityEventName.GameOver));

                yield return new WaitForEndOfFrame();
            }
        }

        /// <summary>
        /// Lerp vers la couleur de la zone où se trouve la barre (normale, jaune, rouge).
        /// Dépend seulement du remplissage : la transition se fait aussi dans l'autre sens quand la barre remonte.
        /// </summary>
        private void UpdateFillColor()
        {
            Color zoneColor = GetZoneColor();
            if (zoneColor != _targetColor)
            {
                // Changement de zone : nouvelle transition depuis la couleur actuelle
                _transitionStartColor = _fillImage.color;
                _targetColor = zoneColor;
                _transitionTime = 0f;
            }

            if (_transitionTime >= _colorTransitionDuration)
                return;

            _transitionTime += Time.deltaTime;
            float t = _colorTransitionDuration > 0f ? Mathf.Clamp01(_transitionTime / _colorTransitionDuration) : 1f;
            _fillImage.color = Color.Lerp(_transitionStartColor, _targetColor, t);
        }

        private Color GetZoneColor()
        {
            float fill = Mathf.InverseLerp(_slider.minValue, _slider.maxValue, _slider.value);
            if (fill <= _dangerThreshold)
                return _dangerColor;
            if (fill <= _warningThreshold)
                return _warningColor;
            return _normalColor;
        }

        #region UnityEvents

        void InitEvents()
        {
            UnityEventManager.AddListener(nameof(EnumUnityEventName.GoodDishDeposit), RaiseSlider);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.ShitDishDeposit), DecreaseSlider);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.GameOver), StopSlideRoutine);
            UnityEventManager.AddListener(nameof(EnumUnityEventName.StartTime), StartSliderLifeRoutine);
        }

        void RevokeEvents()
        {
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GoodDishDeposit), RaiseSlider);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.ShitDishDeposit), DecreaseSlider);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameOver), StopSlideRoutine);
            UnityEventManager.RemoveListener(nameof(EnumUnityEventName.StartTime), StartSliderLifeRoutine);
        }

        #endregion UnityEvents
    }
}