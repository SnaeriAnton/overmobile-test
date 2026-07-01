using UnityEngine;

namespace Game
{
    public class PlayerAnimatorController : MonoBehaviour
    {
        private const string ATTACK = "Attack";
        private const string MOVE = "Move";
        private const string GET_SWORD = "GetSword";
        private const string DEATH = "Death";
        private const string IDLE = "Armature_001|Knight_Shop_Idle";
        
        [SerializeField] private Animator _animator;
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private AudioClip _getSwordClip;
        [SerializeField] private AudioClip _runClip;

        public void StartMove()
        {
            _audioSource.loop = true;
            _audioSource.clip = _runClip;
            _audioSource.Play();
            _animator.SetBool(MOVE, true);
        }

        public void StopMove()
        {
            _audioSource.Stop();   
            _audioSource.loop = false;
            _audioSource.clip = null;
            _animator.SetBool(MOVE, false);
        }

        public void Attack() =>
            _animator.SetTrigger(ATTACK);

        public void GetSword()
        {
            _audioSource.PlayOneShot(_getSwordClip);        
            _animator.SetTrigger(GET_SWORD);
        }
        
        public void Die() =>
            _animator.SetTrigger(DEATH);

        public bool IsIdle() =>
            !_animator.IsInTransition(0) && _animator.GetCurrentAnimatorStateInfo(0).IsName(IDLE);
        
    }
}
