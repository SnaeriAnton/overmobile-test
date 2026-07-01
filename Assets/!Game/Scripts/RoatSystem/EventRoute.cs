using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class EventRoute : MonoBehaviour
    {
        [SerializeField] private EventPoint _startPoint;
        [SerializeField] private List<EventPoint> _points = new List<EventPoint>();

        public EventPoint StartPoint => _startPoint;
        public IReadOnlyList<EventPoint> Points => _points;

        public void SetStartPoint(EventPoint point)
        {
            _startPoint = point;

            if (point != null && !_points.Contains(point))
                _points.Add(point);
        }

        public void AddPoint(EventPoint point)
        {
            if (point == null || _points.Contains(point))
                return;

            _points.Add(point);
        }

        public void RemovePoint(EventPoint point)
        {
            if (point == null)
                return;

            _points.Remove(point);

            if (_startPoint == point)
                _startPoint = null;
        }

        public void CollectChildPoints()
        {
            _points.Clear();
            GetComponentsInChildren(true, _points);
            NormalizePoints();
        }

        public List<EventPoint> BuildRouteByFirstConnections()
        {
            var route = new List<EventPoint>();

            if (_startPoint == null)
                return route;

            var visited = new HashSet<EventPoint>();
            var current = _startPoint;

            while (current != null && visited.Add(current))
            {
                route.Add(current);
                current = GetFirstAvailableNextPoint(current);
            }

            return route;
        }

        public bool TryBuildRouteTo(EventPoint targetPoint, List<EventPoint> route)
        {
            return TryBuildRouteTo(_startPoint, targetPoint, route);
        }

        public bool TryBuildRouteTo(EventPoint startPoint, EventPoint targetPoint, List<EventPoint> route)
        {
            if (route == null)
                return false;

            route.Clear();

            if (startPoint == null || targetPoint == null)
                return false;

            var queue = new Queue<EventPoint>();
            var previousPoints = new Dictionary<EventPoint, EventPoint>();

            queue.Enqueue(startPoint);
            previousPoints.Add(startPoint, null);

            while (queue.Count > 0)
            {
                var point = queue.Dequeue();

                if (point == targetPoint)
                {
                    FillRoute(targetPoint, previousPoints, route);
                    CutRouteAtFirstBusyPoint(startPoint, route);
                    return true;
                }

                EnqueueConnectedPoints(point, queue, previousPoints);
            }

            return false;
        }

        private void EnqueueConnectedPoints(
            EventPoint point,
            Queue<EventPoint> queue,
            Dictionary<EventPoint, EventPoint> previousPoints)
        {
            for (var i = 0; i < point.NextPoints.Count; i++)
                EnqueuePoint(point.NextPoints[i], point, queue, previousPoints);

            for (var i = 0; i < _points.Count; i++)
            {
                var connectedPoint = _points[i];

                if (connectedPoint != null && connectedPoint.HasNextPoint(point))
                    EnqueuePoint(connectedPoint, point, queue, previousPoints);
            }
        }

        private void EnqueuePoint(
            EventPoint nextPoint,
            EventPoint previousPoint,
            Queue<EventPoint> queue,
            Dictionary<EventPoint, EventPoint> previousPoints)
        {
            if (nextPoint == null || previousPoints.ContainsKey(nextPoint))
                return;

            queue.Enqueue(nextPoint);
            previousPoints.Add(nextPoint, previousPoint);
        }

        private void CutRouteAtFirstBusyPoint(EventPoint startPoint, List<EventPoint> route)
        {
            for (var i = 0; i < route.Count; i++)
            {
                var point = route[i];

                if (point == startPoint || point.Type == GameEventTypes.Free)
                    continue;

                route.RemoveRange(i + 1, route.Count - i - 1);
                return;
            }
        }

        private EventPoint GetFirstAvailableNextPoint(EventPoint point)
        {
            for (var i = 0; i < point.NextPoints.Count; i++)
            {
                var nextPoint = point.NextPoints[i];

                if (nextPoint != null)
                    return nextPoint;
            }

            return null;
        }

        private void FillRoute(EventPoint targetPoint, Dictionary<EventPoint, EventPoint> previousPoints, List<EventPoint> route)
        {
            var current = targetPoint;

            while (current != null)
            {
                route.Add(current);
                current = previousPoints[current];
            }

            route.Reverse();
        }

        private void NormalizePoints()
        {
            for (var i = _points.Count - 1; i >= 0; i--)
            {
                var point = _points[i];

                if (point == null || _points.IndexOf(point) != i)
                    _points.RemoveAt(i);
            }

            if (_startPoint != null && !_points.Contains(_startPoint))
                _points.Add(_startPoint);
        }

        private void OnValidate() => NormalizePoints();
    }
}
