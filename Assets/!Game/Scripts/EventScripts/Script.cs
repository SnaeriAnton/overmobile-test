using System.Collections;
using UnityEngine;

namespace Game
{
    public class Script : MonoBehaviour
    {
        protected Player _player;
        protected Interaction _interaction;

        private Coroutine _coroutine;

        public virtual void Construct(Player player, Interaction interaction)
        {
            _player = player;
            _interaction = interaction;
        }

        public virtual void StartScript() =>
            EnableInteraction();

        protected void EnableInteractionAfterPlayerIdle()
        {
            if (_coroutine == null)
                _coroutine = StartCoroutine(EnableInteractionAfterPlayerIdleRoutine());
        }

        private void EnableInteraction()
        {
            _interaction.SetActiveAvailableIndicators(true);
            _interaction.SetActive(true);
        }

        private IEnumerator EnableInteractionAfterPlayerIdleRoutine()
        {
            while (!_player.IsIdleAnimation())
                yield return null;

            EnableInteraction();
            _coroutine = null;
        }
    }
}
