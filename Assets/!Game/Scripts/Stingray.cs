using UnityEngine;

namespace Game
{
    public class Stingray : MonoBehaviour
    {
        [SerializeField] private float _moveDistance = 5f;
        [SerializeField] private float _moveDuration = 2f;

        private Vector3 _startPosition;
        private Vector3 _targetPosition;
        private float _elapsedTime;
        private bool _isMoving;

        private void OnEnable()
        {
            StartMove();
        }

        private void Update()
        {
            if (!_isMoving)
                return;

            _elapsedTime += Time.deltaTime;

            float progress = Mathf.Clamp01(_elapsedTime / _moveDuration);
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);

            transform.position = Vector3.Lerp(_startPosition, _targetPosition, easedProgress);

            if (progress < 1f)
                return;

            transform.position = _targetPosition;
            _isMoving = false;
        }

        private void StartMove()
        {
            _startPosition = transform.position;
            _targetPosition = _startPosition + transform.right * _moveDistance;
            _elapsedTime = 0f;

            if (_moveDuration <= 0f)
            {
                transform.position = _targetPosition;
                _isMoving = false;
                return;
            }

            _isMoving = true;
        }
    }
}
