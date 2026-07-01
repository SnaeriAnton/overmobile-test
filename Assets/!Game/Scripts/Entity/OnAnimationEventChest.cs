using System;
using UnityEngine;

namespace Game
{
    public class OnAnimationEventChest : MonoBehaviour
    {
        private Action _onAttack;
        public void Construct(Action onAttack) => _onAttack = onAttack;
        public void OnEvent() => _onAttack.Invoke();
    }
}
