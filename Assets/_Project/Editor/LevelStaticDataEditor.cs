using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using GameObjectsScripts;
using Infrastructure.AssetManagement;
using StaticData;
using UnityEditor;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

namespace Editor
{
    [CustomEditor(typeof(LevelStaticData))]
    public class LevelStaticDataEditor : UnityEditor.Editor
    {
        //private List<TemporarySceneObject> _temporarySceneObjects = new();

        public override async void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LevelStaticData levelData = (LevelStaticData)target;
            GUILayout.Label("Collecting");
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

            GUILayout.Space(20);
            GUILayout.Label("Markers position");

            if (GUILayout.Button("Instantiate prefabs"))
            {
                TemporarySceneObject[] list = FindObjectsOfType<TemporarySceneObject>();
                if (list.Length != 0)
                {
                    DestroyTemporary(list);
                }

                if (levelData.EnvironmentObjectsSpawnData.Count == 0)
                {
                }

                if (levelData.StuffSpawnData.Count == 0)
                {
                }

                foreach (EnvironmentObjectSpawnData objectSpawnData in levelData.EnvironmentObjectsSpawnData)
                {
                    string tmpName = objectSpawnData.GameObjectsTypeId.ToString();
                    GameObject tmpGo =
                        await Addressables.InstantiateAsync(tmpName, objectSpawnData.Position, Quaternion.identity);
                    tmpGo.AddComponent<TemporarySceneObject>();
                    tmpGo.name = tmpName.ToUpper();

                    TemporarySceneObject tmp = tmpGo.GetComponent<TemporarySceneObject>();
                    tmp.Id = objectSpawnData.Id;
                    tmp.Type = objectSpawnData.GameObjectsTypeId;
                    Transform[] childs = tmpGo.gameObject.GetComponentsInChildren<Transform>(true);

                    foreach (Transform transform in childs)
                    {
                        transform.gameObject.SetActive(true);
                    }
                }

                /*foreach (StuffSpawnData stuffSpawnData in levelData.StuffSpawnData)
                {
                    var markerView = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    markerView.transform.position = stuffSpawnData.Position;
                    markerView.transform.localScale = new Vector3(.3f, .3f, .3f);
                    
                    var stuffSceneMarker = FindObjectsOfType<UniqueId>()
                        .First(m => m.GetComponent<UniqueId>().Id == stuffSpawnData.Id);

                    markerView.transform.parent = stuffSceneMarker.transform.parent;

                    markerView.AddComponent<StuffTemporaryObject>();
                    var tmpStuff = markerView.GetComponent<StuffTemporaryObject>();
                    tmpStuff.Id = stuffSpawnData.Id;
                    tmpStuff.Species = stuffSpawnData.StuffSpecies;
                }*/
            }
            
            if (GUILayout.Button("Delete prefabs"))
            {
                TemporarySceneObject[] list = FindObjectsOfType<TemporarySceneObject>();
                DestroyTemporary(list);
            }

            if (GUILayout.Button("Set new marker position"))
            {
                List<TemporarySceneObject> list = FindObjectsOfType<TemporarySceneObject>().ToList();
                List<StuffTemporaryObject> listStuff = FindObjectsOfType<StuffTemporaryObject>().ToList();
                
                list.ForEach(x =>
                {
                    EnvironmentObjectSpawnData marker =
                        levelData.EnvironmentObjectsSpawnData.First(z => z.GameObjectsTypeId == x.Type && z.Id == x.Id);
                    Debug.Log($"Marker: {marker.GameObjectsTypeId}");
                    Vector3 tsoPosition = x.transform.position; 
                    marker.Position = tsoPosition;

                    EnvironmentObjectMarker sceneMarker = FindObjectsOfType<EnvironmentObjectMarker>()
                        .First(m => m.GetComponent<UniqueId>().Id == marker.Id);

                    sceneMarker.transform.position = tsoPosition;
                });
                
                /*listStuff.ForEach(x =>
                {
                    StuffSpawnData stuffMarker =
                        levelData.StuffSpawnData.First(z => z.StuffSpecies == x.Species && z.Id == x.Id);
                    Vector3 smPosition = x.transform.position;
                    stuffMarker.Position = smPosition;
                    
                    

                    //stuffSceneMarker.transform.position = smPosition;
                });*/
            }

            EditorUtility.SetDirty(target);
        }

        private void DestroyTemporary(IEnumerable<TemporarySceneObject> temporarySceneObjects)
        {
            foreach (TemporarySceneObject temporarySceneObject in temporarySceneObjects)
            {
                DestroyImmediate(temporarySceneObject.gameObject);
            }
        }
    }
}