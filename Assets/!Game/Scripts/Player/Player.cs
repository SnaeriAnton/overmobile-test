using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class Player : MonoBehaviour
    {
        public int Power;

        public Transform Center;
        public EventPoint Point;

        [SerializeField] private OnAnimationEvent _animationEvent;
        [SerializeField] private PlayerAnimatorController _animatorController;
        [SerializeField] private Transform _textTransform;
        [SerializeField] private TMP_Text _text;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioSource _kickSource;
        [SerializeField] private AudioSource _levelUpSource;
        [SerializeField] private float _moveSpeed = 3f;

        private EventRoute _route;
        private Entity _entity;
        private Camera _camera;
        private CameraMover _cameraMover;
        private readonly List<EventPoint> _path = new List<EventPoint>();
        private int _pathIndex;
        private bool _isMove;
        private bool _isDead;

        public event Action Died;
        public Transform TextTransform => _textTransform;

        private void Update()
        {
            if (!_isMove) return;

            MoveByPath();
        }

        private void LateUpdate() => _textTransform.forward = _camera.transform.forward;

        public void Construct(EventRoute route, Camera camera, CameraMover cameraMover)
        {
            _route = route;
            _camera = camera;
            _cameraMover = cameraMover;
            _text.text = Power.ToString();
            _animationEvent.Construct(OnAnimationEvent);
        }

        public void Move(Entity entity)
        {
            if (!_route.TryBuildRouteTo(Point, entity.Point, _path))
                throw new MissingReferenceException($"No route from {Point?.name ?? "null"} to {entity.Point?.name ?? "null"}.");

            _entity = entity;
            _pathIndex = 0;
            _isMove = true;
            _animatorController.StartMove();
        }

        public void Kill()
        {
            if (_isDead)
                return;

            _audioSource.Play();
            _isDead = true;
            _textTransform.gameObject.SetActive(false);
            _animatorController.Die();
            Died?.Invoke();
        }

        public void GetSword()
        {
            _animatorController.transform.localPosition = Vector3.zero;
            _animatorController.GetSword();
        }

        public void AddPower(int value)
        {
            _levelUpSource.Play();
            Power += value;
            _text.text = Power.ToString();
        }

        public void Attack()
        {
            _animatorController.Attack();
        }

        public bool IsIdleAnimation() => _animatorController.IsIdle();
        
        private void StopMove()
        {
            _isMove = false;
            _animatorController.StopMove();
        }
        
        private void OnAnimationEvent()
        {
            _kickSource.Play();
            _cameraMover.Shake();
            _animatorController.transform.localPosition = Vector3.zero;
            _entity.Hit();
            _entity = null;
        }

        private void MoveByPath()
        {
            if (_pathIndex >= _path.Count)
            {
                CompleteMove();
                return;
            }

            EventPoint targetPoint = _path[_pathIndex];

            Vector3 targetPosition = targetPoint.transform.position;
            Vector3 movementDirection = targetPosition - transform.position;

            LookAtMovementDirection(movementDirection);

            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPosition) > 0)
                return;

            transform.position = targetPosition;
            Point = targetPoint;
            _route.SetStartPoint(targetPoint);
            _pathIndex++;

            if (_pathIndex >= _path.Count)
            {
                CompleteMove();
                targetPoint.GetComponent<Script>().StartScript();
            }
        }

        private void LookAtMovementDirection(Vector3 movementDirection)
        {
            movementDirection.y = 0f;

            if (movementDirection.sqrMagnitude <= Mathf.Epsilon)
                return;

            transform.rotation = Quaternion.LookRotation(-movementDirection);
        }

        private void CompleteMove()
        {
            StopMove();
            _path.Clear();
        }
    }
}
