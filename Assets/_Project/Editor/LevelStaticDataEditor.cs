using System.Linq;
using GameObjectsScripts;
using Infrastructure;
using StaticData;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    [CustomEditor(typeof(LevelStaticData))]
    public class LevelStaticDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelStaticData levelData = (LevelStaticData)target;
            if (GUILayout.Button("Collect"))
            {
                levelData.EnvironmentObjectsSpawnData = FindObjectsOfType<EnvironmentObjectMarker>()
                    .Select(x =>
                        new EnvironmentObjectSpawnData(x.GetComponent<UniqueId>().Id,
                            x.EnvironmentObjectStaticData.GameObjectsTypeId, x.transform.position))
                    .ToList();
                
                levelData.LevelKey = SceneManager.GetActiveScene().name;
                levelData.PlayerSpawnPoint = FindObjectOfType<PlayerSpawnPoint>().transform.position;
            }

            if (GUILayout.Button("Clear level data"))
            {
                levelData.EnvironmentObjectsSpawnData.Clear();
            }

            EditorUtility.SetDirty(target);
        }
    }
}