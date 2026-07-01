using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Enemy : Entity
    {
        private const string ATTACK = "Attack";
        private const string DEATH = "Death";

        [SerializeField] private Animator _animator;
        [SerializeField] private OnAnimationEvent _animationEvent;
        [SerializeField] private GameObject _model;
        [SerializeField] private GameObject _shadowObject;
        [SerializeField] private Transform _textTransform;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private GameObject _hitObject;

        private Camera _camera;
        private Player _player;
        private CameraMover _cameraMover;

        public event Action HitStarted;
        public Transform TextTransform => _textTransform;
        public override bool CanShowIndicator => !_isDead;

        private bool _isDead;

        private void LateUpdate()
        {
            _textTransform.forward = _camera.transform.forward;
            _hitObject.transform.forward = _camera.transform.forward;
        }

        public void Construct(Player player, Camera camera, CameraMover cameraMover)
        {
            _player = player;
            _camera = camera;
            _cameraMover = cameraMover;
            _text.text = Power.ToString();
            _animationEvent.Construct(OnAnimationEvent);
        }

        public void Attack() => _animator.SetTrigger(ATTACK);
        
        private void OnAnimationEvent()
        {
            _cameraMover.Shake();
            _player.Kill();
        }

        public override void Hit()
        {
            _isDead = true;
            _hitObject.SetActive(true);
            _animator.SetTrigger(DEATH);
            HitStarted?.Invoke();
            SetActiveIndicator(false);
            Destroy(_model);
            _shadowObject.SetActive(false);
            Destroy(gameObject, 1f);
        }
    }
}
