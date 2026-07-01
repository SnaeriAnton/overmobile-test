using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;

namespace Game
{
    public class Packshot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private RectTransform _imageTransform;
        [SerializeField] private Image _image;
        [SerializeField] private AudioSource _audioSource;
        [Min(0f)]
        [SerializeField] private float _fadeDuration = 0.5f;
        [Range(0f, 1f)]
        [SerializeField] private float _targetAlpha = 1f;
        [Min(0f)]
        [SerializeField] private float _scaleIncreasePercent = 1f;
        [Min(0f)]
        [SerializeField] private float _scaleDuration = 10f;

        private Vector3 _startImageScale;
        private Tween _fadeTween;
        private Tween _scaleTween;

        private void Awake()
        {
            if (_imageTransform == null && _image != null)
                _imageTransform = _image.rectTransform;

            _startImageScale = GetValidStartScale();
        }

        public void Show()
        {
            _audioSource.Play();
            _fadeTween?.Kill();
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _fadeTween = DOTween
                .To(() => _canvasGroup.alpha, value => _canvasGroup.alpha = value, _targetAlpha, _fadeDuration)
                .OnKill(() => _fadeTween = null);


            _scaleTween?.Kill();
            _imageTransform.localScale = _startImageScale;
            _scaleTween = _imageTransform
                .DOScale(_startImageScale * GetTargetScaleMultiplier(), _scaleDuration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental)
                .OnKill(() => _scaleTween = null);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Luna.Unity.LifeCycle.GameEnded();
            Luna.Unity.Playable.InstallFullGame();
        }
        
        public void OnPointerDown(PointerEventData eventData) { }

        private float GetTargetScaleMultiplier() => 1f + _scaleIncreasePercent / 100f;

        private Vector3 GetValidStartScale()
        {
            if (_imageTransform == null)
                return Vector3.one;

            Vector3 scale = _imageTransform.localScale;

            if (scale == Vector3.zero)
                return Vector3.one;

            return scale;
        }
    }
}
