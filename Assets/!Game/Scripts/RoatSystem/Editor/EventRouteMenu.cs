using Game;
using UnityEditor;
using UnityEngine;

namespace Game.Editor
{
    public static class EventRouteMenu
    {
        [MenuItem("GameObject/Game/Event Route", false, 10)]
        public static void CreateRoute()
        {
            var routeObject = new GameObject("Event Route");
            var route = routeObject.AddComponent<EventRoute>();

            Undo.RegisterCreatedObjectUndo(routeObject, "Create Event Route");
            Selection.activeGameObject = routeObject;

            CreatePoint(route.transform, GameEventTypes.Free);
        }

        [MenuItem("GameObject/Game/Event Point", false, 11)]
        public static void CreateEventPoint()
        {
            var parent = Selection.activeTransform;
            CreatePoint(parent, GameEventTypes.Free);
        }

        public static EventPoint CreatePoint(Transform parent, GameEventTypes type)
        {
            var pointObject = new GameObject(type + " Point");
            Undo.RegisterCreatedObjectUndo(pointObject, "Create Event Point");

            if (parent != null)
                Undo.SetTransformParent(pointObject.transform, parent, "Parent Event Point");

            pointObject.transform.localPosition = Vector3.zero;

            var point = pointObject.AddComponent<EventPoint>();
            point.Type = type;

            var route = parent != null ? parent.GetComponentInParent<EventRoute>() : null;

            if (route != null)
            {
                Undo.RecordObject(route, "Add Event Point To Route");
                route.AddPoint(point);

                if (route.StartPoint == null)
                    route.SetStartPoint(point);

                EditorUtility.SetDirty(route);
            }

            Selection.activeGameObject = pointObject;
            EditorGUIUtility.PingObject(pointObject);

            return point;
        }
    }
}
