using UnityEngine;
using System.Collections.Generic;

namespace Game
{
    public class EventPoint : MonoBehaviour
    {
        public GameEventTypes Type;
        public Script EventScript;
        
        [SerializeField] private List<EventPoint> _nextPoints = new List<EventPoint>();

        public IReadOnlyList<EventPoint> NextPoints => _nextPoints;

        public void ReleasePoint() => Type = GameEventTypes.Free;

        public bool HasNextPoint(EventPoint point) => point != null && _nextPoints.Contains(point);

        public bool TryAddNextPoint(EventPoint point)
        {
            if (point == null || point == this || _nextPoints.Contains(point))
                return false;

            _nextPoints.Add(point);
            return true;
        }

        public bool RemoveNextPoint(EventPoint point) => _nextPoints.Remove(point);

        public void ClearNextPoints() => _nextPoints.Clear();

        public void NormalizeConnections()
        {
            for (var i = _nextPoints.Count - 1; i >= 0; i--)
            {
                EventPoint point = _nextPoints[i];

                if (point == null || point == this || _nextPoints.IndexOf(point) != i)
                    _nextPoints.RemoveAt(i);
            }
        }

        private void OnValidate() => NormalizeConnections();
    }
}
