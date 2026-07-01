using DG.Tweening;
using UnityEngine;

namespace Game
{
    public class EnemyScript : Script
    {
        [SerializeField] private Enemy _enemy;
        [SerializeField] private float _textFlyDuration = 0.35f;
        [SerializeField] private float _textArcHeight = 1.5f;
        [SerializeField] private float _playerTextPunchScale = 0.25f;
        [SerializeField] private float _playerTextPunchDuration = 0.25f;

        public override void StartScript()
        {
            if (_player.Power >= _enemy.Power)
            {
                _enemy.HitStarted += PlayRewardSequence;
                _player.Attack();
                return;
            }

            LookEnemyAtPlayer();
            _enemy.Attack();
        }

        private void LookEnemyAtPlayer()
        {
            Vector3 direction = _player.transform.position - _enemy.transform.position;
            direction.y = 0f;
            _enemy.transform.rotation = Quaternion.LookRotation(-direction);
        }

        private void PlayRewardSequence()
        {
            _enemy.HitStarted -= PlayRewardSequence;

            Transform enemyTextTransform = _enemy.TextTransform;
            Transform playerTextTransform = _player.TextTransform;
            Vector3 textStartPosition = enemyTextTransform.position;
            Vector3 textEndPosition = playerTextTransform.position;

            enemyTextTransform.SetParent(null, true);

            Sequence sequence = DOTween.Sequence();
            sequence
                .Append(DOTween
                    .To(
                        () => 0f,
                        value => enemyTextTransform.position = GetTextArcPosition(textStartPosition, textEndPosition, value),
                        1f,
                        _textFlyDuration)
                    .SetEase(Ease.Linear))
                .AppendCallback(ApplyReward)
                .Append(playerTextTransform
                    .DOPunchScale(Vector3.one * _playerTextPunchScale, _playerTextPunchDuration))
                .OnComplete(EnableInteractionAfterPlayerIdle);
        }

        private Vector3 GetTextArcPosition(Vector3 startPosition, Vector3 endPosition, float value)
        {
            Vector3 position = Vector3.Lerp(startPosition, endPosition, value);
            position += Vector3.up * (_textArcHeight * 4f * value * (1f - value));
            return position;
        }

        private void ApplyReward()
        {
            _enemy.Point.ReleasePoint();
            _enemy.TextTransform.gameObject.SetActive(false);
            _player.AddPower(_enemy.Power);
        }
    }
}
