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
                
                levelData.StuffSpawnData = FindObjectsOfType<StuffSpawnerMarker>()
                    .Select(x =>
                        new StuffSpawnData(x.GetComponent<UniqueId>().Id,
                            x.StuffStaticData.StuffSpecies, x.transform.position))
                    .ToList();
                
                levelData.LevelKey = SceneManager.GetActiveScene().name;
                Transform playerSpawnPointTransform = FindObjectOfType<PlayerSpawnPoint>().transform; 
                levelData.PlayerSpawnPoint = playerSpawnPointTransform.position;
                levelData.PlayerRotation = playerSpawnPointTransform.rotation;
            }

            if (GUILayout.Button("Clear level data"))
            {
                levelData.EnvironmentObjectsSpawnData.Clear();
                levelData.StuffSpawnData.Clear();
            }

            EditorUtility.SetDirty(target);
        }
    }
}