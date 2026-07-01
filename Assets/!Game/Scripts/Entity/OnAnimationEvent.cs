using System;
using UnityEngine;

namespace Game
{
    public class OnAnimationEvent : MonoBehaviour
    {
        private Action _onAttack;
        public void Construct(Action onAttack) => _onAttack = onAttack;
        public void OnAttack() => _onAttack.Invoke();
    }
}