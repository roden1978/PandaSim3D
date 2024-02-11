using System;
using StaticData;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(EnvironmentObjectMarker))]
    public class EnvironmentObjectSpawnEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.NonSelected)]
        public static void RenderCustomGizmo(EnvironmentObjectMarker environmentObjectMarker, GizmoType gizmo)
        {
            Vector3 scale;
            MeshFilter meshFilter =
                environmentObjectMarker.EnvironmentObjectStaticData.Prefab.GetComponentInChildren<MeshFilter>(true);
            MeshRenderer meshRenderer =
                environmentObjectMarker.EnvironmentObjectStaticData.Prefab.GetComponentInChildren<MeshRenderer>();
            Gizmos.color = environmentObjectMarker.Color;
            Vector3 position = environmentObjectMarker.transform.position;
            float size = environmentObjectMarker.Size;
            GameObject gameObject = meshFilter.gameObject;
            
            scale = meshFilter is not null ? gameObject.transform.localScale : new Vector3(size, size, size);
            
            Gizmos.DrawMesh(meshFilter.sharedMesh, 
                new Vector3(position.x, position.y + gameObject.transform.position.y, position.z), 
                gameObject.transform.rotation, scale);

            Vector3 labelPosition = new(position.x, position.y + gameObject.transform.position.y + 2f, position.z);
            string text =
                $"{Enum.GetName(typeof(GameObjectsTypeId), environmentObjectMarker.EnvironmentObjectStaticData.GameObjectsTypeId)?.ToUpper()}";
            Handles.Label(labelPosition, text);
        }
    }
}