using System;
using DG.Tweening;
using UnityEngine;
using TMPro;

namespace Game
{
    public class Chest : Entity
    {
        private const string OPEN = "Open";

        [SerializeField] private Animator _animator;
        [SerializeField] private Transform _textTransform;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private OnAnimationEventChest _onAnimationEventChest;
        [SerializeField] private GameObject _animatorParticle;
        [SerializeField] private Transform _swordTransform;

        private Camera _camera;

        public event Action Opened;
        public Transform TextTransform => _textTransform;
        public Transform SwordTransform => _swordTransform;
        public float SwordRoll { get; set; }
        public override bool CanShowIndicator => !_isOpened;

        private bool _isOpened;

        private void LateUpdate()
        {
            _textTransform.forward = _camera.transform.forward;
            _swordTransform.rotation = Quaternion.LookRotation(_camera.transform.forward) * Quaternion.Euler(0f, 0f, SwordRoll);
        }

        public void Construct(Camera camera)
        {
            _camera = camera;
            _text.text = Power.ToString();
            _onAnimationEventChest.Construct(OnOpened);
        }

        public override void Hit()
        {
            _isOpened = true;
            _animator.SetTrigger(OPEN);
            transform.DOLocalMove(new Vector3(7.7f, 0f, 5.62f), .2f);
            SetActiveIndicator(false);
        }

        private void OnOpened()
        {
            _animatorParticle.SetActive(true);
            Opened?.Invoke();
        }
    }
}
