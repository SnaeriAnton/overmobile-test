
using System.Collections.Generic;
using Game;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(EventPoint))]
    [CanEditMultipleObjects]
    public class EventPointEditor : UnityEditor.Editor
    {
        private SerializedProperty _typeProperty;
        private SerializedProperty _nextPointsProperty;
        private SerializedProperty _scriptProperty;

        private void OnEnable()
        {
            _typeProperty = serializedObject.FindProperty(nameof(EventPoint.Type));
            _nextPointsProperty = serializedObject.FindProperty("_nextPoints");
            _scriptProperty = serializedObject.FindProperty("EventScript");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_typeProperty);
            EditorGUILayout.PropertyField(_scriptProperty);
            EditorGUILayout.PropertyField(_nextPointsProperty, true);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            DrawConnectionTools();
        }

        private void DrawConnectionTools()
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Route Tools", EditorStyles.boldLabel);

                if (GUILayout.Button("Connect Active Point To Selected"))
                    ConnectActivePointToSelectedPoints();

                if (GUILayout.Button("Disconnect Active Point From Selected"))
                    DisconnectActivePointFromSelectedPoints();

                if (GUILayout.Button("Connect Selected Points As Chain"))
                    ConnectSelectedPointsAsChain();

                if (GUILayout.Button("Clear Connections"))
                    ClearTargetConnections();
            }
        }

        private void ConnectActivePointToSelectedPoints()
        {
            var activePoint = GetActiveEventPoint();
            var selectedPoints = GetSelectedEventPoints();

            if (activePoint == null)
                return;

            Undo.RecordObject(activePoint, "Connect Event Points");

            for (var i = 0; i < selectedPoints.Count; i++)
            {
                activePoint.TryAddNextPoint(selectedPoints[i]);
                AddPointToRoute(activePoint, selectedPoints[i]);
            }

            activePoint.NormalizeConnections();
            EditorUtility.SetDirty(activePoint);
        }

        private void DisconnectActivePointFromSelectedPoints()
        {
            var activePoint = GetActiveEventPoint();
            var selectedPoints = GetSelectedEventPoints();

            if (activePoint == null)
                return;

            Undo.RecordObject(activePoint, "Disconnect Event Points");

            for (var i = 0; i < selectedPoints.Count; i++)
                activePoint.RemoveNextPoint(selectedPoints[i]);

            activePoint.NormalizeConnections();
            EditorUtility.SetDirty(activePoint);
        }

        private void ClearTargetConnections()
        {
            foreach (var targetObject in targets)
            {
                var point = (EventPoint)targetObject;
                Undo.RecordObject(point, "Clear Event Point Connections");
                point.ClearNextPoints();
                EditorUtility.SetDirty(point);
            }
        }

        private void ConnectSelectedPointsAsChain()
        {
            var selectedPoints = GetSelectedEventPoints();

            for (var i = 0; i < selectedPoints.Count - 1; i++)
            {
                var point = selectedPoints[i];
                Undo.RecordObject(point, "Connect Event Point Chain");
                point.TryAddNextPoint(selectedPoints[i + 1]);
                AddPointToRoute(point, selectedPoints[i + 1]);
                point.NormalizeConnections();
                EditorUtility.SetDirty(point);
            }
        }

        private List<EventPoint> GetSelectedEventPoints()
        {
            var points = new List<EventPoint>();
            var gameObjects = Selection.gameObjects;

            for (var i = 0; i < gameObjects.Length; i++)
            {
                var point = gameObjects[i].GetComponent<EventPoint>();

                if (point != null && !points.Contains(point))
                    points.Add(point);
            }

            return points;
        }

        private EventPoint GetActiveEventPoint()
        {
            return Selection.activeGameObject != null
                ? Selection.activeGameObject.GetComponent<EventPoint>()
                : null;
        }

        private void AddPointToRoute(EventPoint ownerPoint, EventPoint connectedPoint)
        {
            var route = ownerPoint.GetComponentInParent<EventRoute>();

            if (route == null)
                route = connectedPoint.GetComponentInParent<EventRoute>();

            if (route == null)
                return;

            Undo.RecordObject(route, "Add Event Points To Route");
            route.AddPoint(ownerPoint);
            route.AddPoint(connectedPoint);
            EditorUtility.SetDirty(route);
        }
    }
}
