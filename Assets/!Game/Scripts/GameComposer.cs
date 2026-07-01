using System;
using System.Collections;
using System.Collections.Generic;
using Luna.Unity;
using UnityEngine;

namespace Game
{
    public class GameComposer : MonoBehaviour
    {
        private readonly List<IDisposable> _disposables = new List<IDisposable>();

        [SerializeField] private Player _player;
        [SerializeField] private List<Enemy> _enemies;
        [SerializeField] private List<Script> _scripts;
        [SerializeField] private Camera _camera;
        [SerializeField] private EventRoute _route;
        [SerializeField] private Chest _chest;
        [SerializeField] private CameraMover _cameraMover;
        [SerializeField] private Packshot _packshot;
        [SerializeField] private float _packshotShowDelay = 3f;

        private Interaction _interaction;
        private Coroutine _showPackshotCoroutine;

        private void Awake() => Compose();
        private void Update() => _interaction?.Update();
        private void OnDestroy() => Dispose();

        private void Compose()
        {
            List<Entity> entities = new List<Entity>();
            entities.Add(_chest);
            _enemies.ForEach(entity => entities.Add(entity));
            _interaction = new Interaction(entities, _camera, _player);
            _cameraMover.Construct(_player, _camera);
            _player.Construct(_route, _camera, _cameraMover);
            _player.Died += OnPlayerDied;
            _chest.Construct(_camera);
            _enemies.ForEach(e => e.Construct(_player, _camera, _cameraMover));
            _scripts.ForEach(s => s.Construct(_player, _interaction));
            LifeCycle.GameStarted();
        }

        private void OnPlayerDied()
        {

            _showPackshotCoroutine ??= StartCoroutine(ShowPackshotRoutine());
        }

        private IEnumerator ShowPackshotRoutine()
        {
            yield return new WaitForSeconds(_packshotShowDelay);

            _packshot.Show();
            _showPackshotCoroutine = null;
        }

        private void Dispose()
        {
            if (_player != null)
                _player.Died -= OnPlayerDied;

            _disposables.ForEach(x => x.Dispose());
        }
    }
}
