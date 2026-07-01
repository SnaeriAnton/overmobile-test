using Game;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    [CustomEditor(typeof(EventRoute))]
    public class EventRouteEditor : UnityEditor.Editor
    {
        private SerializedProperty _startPointProperty;
        private SerializedProperty _pointsProperty;

        private void OnEnable()
        {
            _startPointProperty = serializedObject.FindProperty("_startPoint");
            _pointsProperty = serializedObject.FindProperty("_points");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(_startPointProperty);
            EditorGUILayout.PropertyField(_pointsProperty, true);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            DrawRouteTools();
        }

        private void DrawRouteTools()
        {
            var route = (EventRoute)target;

            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("Route Tools", EditorStyles.boldLabel);

                if (GUILayout.Button("Collect Child Event Points"))
                {
                    Undo.RecordObject(route, "Collect Event Points");
                    route.CollectChildPoints();
                    EditorUtility.SetDirty(route);
                }

                if (GUILayout.Button("Set Selected Point As Start"))
                {
                    var point = Selection.activeGameObject != null
                        ? Selection.activeGameObject.GetComponent<EventPoint>()
                        : null;

                    if (point != null)
                    {
                        Undo.RecordObject(route, "Set Route Start Point");
                        route.SetStartPoint(point);
                        EditorUtility.SetDirty(route);
                    }
                }

                if (GUILayout.Button("Create Free Turn Point"))
                    EventRouteMenu.CreatePoint(route.transform, GameEventTypes.Free);
            }
        }
    }
}
