using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class CameraMover : MonoBehaviour
    {
        private Camera _camera;
        private Player _player;
        private Quaternion _startLocalRotation;
        private Tween _shakeTween;
        private float _previousPlayerPositionX;
        private int _centerMoveDirection;

        [SerializeField] private float _shakeDuration = 0.2f;
        [SerializeField] private float _shakeStrength = 8f;
        [SerializeField] private int _shakeVibrato = 12;
        [SerializeField] private float _shakeRandomness = 45f;
        [SerializeField] private float _centerViewportThreshold = 0.02f;
        [SerializeField] private float _horizontalViewportPadding = 0.08f;

        public void Construct(Player player, Camera camera)
        {
            _camera = camera;
            _player = player;
            _startLocalRotation = _camera.transform.localRotation;
            _previousPlayerPositionX = _player.transform.position.x;
        }

        public void LateUpdate()
        {
            if (_player == null || _camera == null)
                return;

            Vector3 playerPosition = _player.transform.position;
            float playerDeltaX = playerPosition.x - _previousPlayerPositionX;
            Vector3 playerViewportPosition = _camera.WorldToViewportPoint(playerPosition);
            Vector3 cameraPosition = _camera.transform.position;
            int playerMoveDirection = GetMoveDirection(playerDeltaX);

            if (playerMoveDirection != 0 && ShouldMoveByCenterLine(playerViewportPosition.x, playerMoveDirection))
            {
                cameraPosition.x = GetPositionForPlayerViewportX(
                    cameraPosition.x,
                    playerPosition,
                    playerViewportPosition,
                    0.5f);

                _camera.transform.position = cameraPosition;
            }

            playerViewportPosition = _camera.WorldToViewportPoint(playerPosition);
            cameraPosition = _camera.transform.position;
            cameraPosition.x = GetPositionKeepingPlayerInFrame(cameraPosition.x, playerPosition, playerViewportPosition);

            _camera.transform.position = cameraPosition;
            _previousPlayerPositionX = playerPosition.x;
        }

        public void Shake()
        {
            _shakeTween?.Kill();
            _camera.transform.localRotation = _startLocalRotation;
            _shakeTween = _camera.transform
                .DOShakeRotation(
                    _shakeDuration,
                    new Vector3(0f, 0f, _shakeStrength),
                    _shakeVibrato,
                    _shakeRandomness,
                    true,
                    ShakeRandomnessMode.Harmonic)
                .OnComplete(() => _camera.transform.localRotation = _startLocalRotation)
                .OnKill(() => _shakeTween = null);
        }

        private bool ShouldMoveByCenterLine(float playerViewportX, int playerMoveDirection)
        {
            if (_centerMoveDirection != 0)
            {
                if (_centerMoveDirection == playerMoveDirection)
                    return true;

                _centerMoveDirection = 0;
                return false;
            }

            float centerOffset = playerViewportX - 0.5f;

            if (Mathf.Abs(centerOffset) > _centerViewportThreshold)
                return false;

            bool movesToCenterFromLeft = centerOffset < 0f && playerMoveDirection > 0;
            bool movesToCenterFromRight = centerOffset > 0f && playerMoveDirection < 0;
            bool isOnCenter = Mathf.Approximately(centerOffset, 0f);

            if (!movesToCenterFromLeft && !movesToCenterFromRight && !isOnCenter)
                return false;

            _centerMoveDirection = playerMoveDirection;
            return true;
        }

        private int GetMoveDirection(float playerDeltaX)
        {
            if (Mathf.Abs(playerDeltaX) <= Mathf.Epsilon)
                return 0;

            return playerDeltaX > 0f ? 1 : -1;
        }

        private float GetPositionKeepingPlayerInFrame(float cameraPositionX, Vector3 playerPosition, Vector3 playerViewportPosition)
        {
            if (playerViewportPosition.x >= _horizontalViewportPadding &&
                playerViewportPosition.x <= 1f - _horizontalViewportPadding)
                return cameraPositionX;

            float targetViewportX = playerViewportPosition.x < _horizontalViewportPadding
                ? _horizontalViewportPadding
                : 1f - _horizontalViewportPadding;

            return GetPositionForPlayerViewportX(cameraPositionX, playerPosition, playerViewportPosition, targetViewportX);
        }

        private float GetPositionForPlayerViewportX(
            float cameraPositionX,
            Vector3 playerPosition,
            Vector3 playerViewportPosition,
            float targetViewportX)
        {
            Vector3 targetWorldPosition = _camera.ViewportToWorldPoint(new Vector3(
                targetViewportX,
                playerViewportPosition.y,
                playerViewportPosition.z));

            return cameraPositionX + playerPosition.x - targetWorldPosition.x;
        }
    }
}
