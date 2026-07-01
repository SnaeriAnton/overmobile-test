using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class Interaction
    {
        private readonly List<Entity> _entities;
        private readonly Camera _camera;
        private readonly Player _player;

        private bool _isActive = true;

        public Interaction(List<Entity> entities, Camera camera, Player player)
        {
            _entities = new List<Entity>(entities);
            _camera = camera;
            _player = player;
        }

        public void SetActive(bool value) => _isActive = value;

        public void SetActiveAvailableIndicators(bool value)
        {
            for (var i = _entities.Count - 1; i >= 0; i--)
            {
                Entity entity = _entities[i];

                if (entity == null)
                {
                    _entities.RemoveAt(i);
                    continue;
                }

                entity.SetActiveIndicator(value && entity.CanShowIndicator);
            }
        }

        public void Update()
        {
            if (!_isActive) return;
            if (Input.GetMouseButtonUp(0)) Click(Input.mousePosition);
        }

        private void Click(Vector2 position)
        {
            Ray ray = _camera.ScreenPointToRay(position);

            if (!Physics.Raycast(ray, out RaycastHit hit)) return;
            if (!hit.collider.TryGetComponent(out Entity entity))
                entity = hit.collider.GetComponentInParent<Entity>();

            if (entity == null || !entity.CanShowIndicator) return;
            SetActiveAvailableIndicators(false);
            entity.SetActiveIndicator(true);
            SetActive(false);
            _player.Move(entity);
        }
    }
}
