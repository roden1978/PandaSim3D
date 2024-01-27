using System;
using StaticData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EnvironmentObjectMarker))]
    public class StuffSpawnerMarkersEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(StuffSpawnerMarker stuffSpawnerMarker, GizmoType gizmo)
        {
            Vector3 position = stuffSpawnerMarker.transform.position;
            DrawMarker(stuffSpawnerMarker.Color, stuffSpawnerMarker.Shape, stuffSpawnerMarker.Size, position);
            Vector3 labelPosition = new(position.x - 0.5f, position.y + 1f, position.z);
            string text =
                $"{Enum.GetName(typeof(StuffSpecies), stuffSpawnerMarker.StuffStaticData.StuffSpecies)?.ToUpper()}";
            Handles.Label(labelPosition, text);
        }

        private static void DrawMarker(Color color, MarkerShapes shape, float size, Vector3 position)
        {
            Gizmos.color = color;
            switch (shape)
            {
                case MarkerShapes.Sphere:
                    Gizmos.DrawSphere(position, size);
                    break;
                case MarkerShapes.WireSphere:
                    Gizmos.DrawWireSphere(position, size);
                    break;
                case MarkerShapes.Cube:
                    Gizmos.DrawCube(position, new Vector3(size, size, size));
                    break;
                case MarkerShapes.WireCube:
                    Gizmos.DrawWireCube(position, new Vector3(size, size, size));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(shape), shape, null);
            }
        }
    }
}