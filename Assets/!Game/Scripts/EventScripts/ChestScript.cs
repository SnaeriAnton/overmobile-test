using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class ChestScript : Script
    {
        [SerializeField] private Chest _chest;
        [SerializeField] private Vector3 _playerRotation;
        [SerializeField] private float _swordFlyDuration = 0.65f;
        [SerializeField] private float _swordArcHeight = 1.5f;
        [SerializeField] private float _swordSpinAngle = 360f;
        [SerializeField] private float _swordTargetScale = 0.4f;
        [SerializeField] private float _swordScaleDuration = 0.2f;
        [SerializeField] private float _swordHideDuration = 0.25f;
        [SerializeField] private float _textFlyDuration = 0.35f;
        [SerializeField] private float _playerTextPunchScale = 0.25f;
        [SerializeField] private float _playerTextPunchDuration = 0.25f;
        
        public override void StartScript()
        {
            _chest.Opened += PlayRewardSequence;
            _player.Attack();
        }

        private void PlayRewardSequence()
        {
            _chest.Opened -= PlayRewardSequence;

            Transform swordTransform = _chest.SwordTransform;
            Transform chestTextTransform = _chest.TextTransform;
            Transform playerTextTransform = _player.TextTransform;
            Vector3 swordStartPosition = swordTransform.position;
            Vector3 swordEndPosition = _player.Center.position;

            float rewardTime = _swordFlyDuration;
            float textStartTime = rewardTime - _textFlyDuration;

            _chest.SwordRoll = 0f;

            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(DOTween
                    .To(
                        () => 0f,
                        value => swordTransform.position = GetSwordArcPosition(swordStartPosition, swordEndPosition, value),
                        1f,
                        _swordFlyDuration)
                    .SetEase(Ease.Linear))
                .Insert(0f, DOTween
                    .To(() => _chest.SwordRoll, value => _chest.SwordRoll = value, _swordSpinAngle, rewardTime)
                    .SetEase(Ease.Linear))
                .Insert(0f, swordTransform
                    .DOScale(Vector3.one * _swordTargetScale, _swordScaleDuration)
                    .SetEase(Ease.OutBack))
                .Insert(textStartTime, chestTextTransform
                    .DOMove(playerTextTransform.position, _textFlyDuration)
                    .SetEase(Ease.InOutQuad))
                .Insert(textStartTime, swordTransform
                    .DOScale(Vector3.zero, _swordHideDuration)
                    .SetEase(Ease.InBack))
                .InsertCallback(rewardTime, ApplyReward)
                .Insert(rewardTime, playerTextTransform
                    .DOPunchScale(Vector3.one * _playerTextPunchScale, _playerTextPunchDuration))
                .OnComplete(EnableInteractionAfterPlayerIdle);
        }

        private Vector3 GetSwordArcPosition(Vector3 startPosition, Vector3 endPosition, float value)
        {
            Vector3 position = Vector3.Lerp(startPosition, endPosition, value);
            position += Vector3.up * (_swordArcHeight * 4f * value * (1f - value));
            return position;
        }

        private void ApplyReward()
        {
            _chest.Point.ReleasePoint();
            _chest.TextTransform.gameObject.SetActive(false);
            _player.AddPower(_chest.Power);
            _player.transform.rotation = Quaternion.Euler(_playerRotation);
            _player.GetSword();
        }
    }
}
