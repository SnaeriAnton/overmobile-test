using UnityEngine;

namespace Game
{
    public class Shark : MonoBehaviour
    {
        [SerializeField] private Vector3 _moveDirection = Vector3.forward;
        [SerializeField] private float _moveDistance = 5f;
        [SerializeField] private float _moveSpeed = 2.5f;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private bool _isMoving;

        private void OnEnable()
        {
            StartMove();
        }

        private void Update()
        {
            if (!_isMoving)
                return;

            transform.position = Vector3.MoveTowards(
                transform.position,
                _targetPosition,
                _moveSpeed * Time.deltaTime);

            if (Vector3.Distance(transform.position, _targetPosition) > 0f)
                return;

            transform.position = _targetPosition;
            _isMoving = false;
        }

        private void StartMove()
        {
            _startPosition = transform.position;
            _targetPosition = _startPosition + GetMoveDirection() * _moveDistance;

            if (_moveSpeed <= 0f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
                return;
            }

            _isMoving = true;
        }

        private Vector3 GetMoveDirection()
        {
            if (_moveDirection.sqrMagnitude <= Mathf.Epsilon)
                return transform.forward;

            return _moveDirection.normalized;
        }
    }
}
