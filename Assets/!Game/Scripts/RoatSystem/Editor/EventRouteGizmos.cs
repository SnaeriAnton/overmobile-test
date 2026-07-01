using Game;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class EventRouteGizmos
    {
        private const float PointRadius = 0.35f;
        private const float LineWidth = 3f;
        private const float StraightLineTolerance = 0.01f;

        [DrawGizmo(GizmoType.Active | GizmoType.Selected | GizmoType.NonSelected | GizmoType.Pickable)]
        private static void DrawEventPoint(EventPoint point, GizmoType gizmoType)
        {
            if (point == null)
                return;

            var position = point.transform.position;
            var color = GetColor(point.Type);

            Handles.color = color;
            Handles.DrawWireDisc(position, Vector3.up, PointRadius);

            if ((gizmoType & GizmoType.Selected) != 0)
                Handles.DrawSolidDisc(position, Vector3.up, PointRadius * 0.35f);

            DrawLabel(point, position, color);
            DrawConnections(point, position);
        }

        private static void DrawConnections(EventPoint point, Vector3 position)
        {
            for (var i = 0; i < point.NextPoints.Count; i++)
            {
                var nextPoint = point.NextPoints[i];

                if (nextPoint == null)
                    continue;

                var nextPosition = nextPoint.transform.position;
                var direction = nextPosition - position;

                if (direction.sqrMagnitude < 0.0001f)
                    continue;

                //  Handles.color = IsStraightConnection(position, nextPosition) ? Color.white : new Color(1f, 0.45f, 0f);
                Handles.color = Color.white;
                Handles.DrawAAPolyLine(LineWidth, position, nextPosition);
                //DrawArrow(position, nextPosition, direction.normalized);
            }
        }

        private static bool IsStraightConnection(Vector3 startPosition, Vector3 endPosition)
        {
            var equalAxes = 0;
            var delta = endPosition - startPosition;

            if (Mathf.Abs(delta.x) <= StraightLineTolerance)
                equalAxes++;

            if (Mathf.Abs(delta.y) <= StraightLineTolerance)
                equalAxes++;

            if (Mathf.Abs(delta.z) <= StraightLineTolerance)
                equalAxes++;

            return equalAxes >= 2;
        }

        private static void DrawArrow(Vector3 startPosition, Vector3 endPosition, Vector3 direction)
        {
            var arrowPosition = Vector3.Lerp(startPosition, endPosition, 0.75f);
            var arrowSize = HandleUtility.GetHandleSize(arrowPosition) * 0.12f;
            var rotation = Quaternion.LookRotation(Vector3.forward, direction);

            Handles.ConeHandleCap(0, arrowPosition, rotation, arrowSize, EventType.Repaint);
        }

        private static void DrawLabel(EventPoint point, Vector3 position, Color color)
        {
            var style = new GUIStyle(EditorStyles.boldLabel)
            {
                normal =
                {
                    textColor = color
                }
            };

            Handles.Label(position + Vector3.up * 0.45f, point.Type.ToString(), style);
        }

        private static Color GetColor(GameEventTypes type)
        {
            switch (type)
            {
                case GameEventTypes.Enemy:
                    return Color.red;
                case GameEventTypes.Award:
                    return Color.yellow;
                case GameEventTypes.Free:
                    return Color.black;
                default:
                    return Color.white;
            }
        }
    }
}