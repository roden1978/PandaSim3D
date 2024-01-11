using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Infrastructure.AssetManagement;
using PlayerScripts;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace GameObjectsScripts
{
    public class Plate : MonoBehaviour, IPositionAdapter, IPointerClickHandler
    {
        [SerializeField] private Transform _anchorPointTransform;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        private DialogManager _dialogManager;
        private ItemType _itemType = ItemType.None;
        private IAssetProvider _assetProvider;

        private Dictionary<string, GameObject> _cachedMeals = new();

        [Inject]
        public void Contruct(DialogManager dialogManager, IAssetProvider assetProvider)
        {
            _dialogManager = dialogManager;
            _assetProvider = assetProvider;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_itemType != ItemType.None) return;

            Debug.Log("Click on plate");
            InventoryDialog dialog = _dialogManager.ShowDialog<InventoryDialog>();
            dialog.UpdateInventoryView();
        }

        public async void InstantiateMeal(ItemType type)
        {
            string mealName = Enum.GetName(typeof(ItemType), (int)type);
            
            if(GetMealFromCache(mealName) is null)
            {
                UniTask<GameObject> result = _assetProvider.LoadAsync<GameObject>(mealName);

                await UniTask.WaitUntil(() => result.Status != UniTaskStatus.Succeeded);
                GameObject prefab = await result;

                Meal meal = Instantiate(prefab, _anchorPointTransform.position, Quaternion.identity,
                        _anchorPointTransform)
                    .GetComponent<Meal>();
                
                AddToMealCache(meal.Item.Name, meal.gameObject);
                
                meal.Construct(this);
            }
            else
            {
                GameObject go = GetMealFromCache(mealName);
                go.SetActive(true);
            }
            _itemType = type;
        }

        public void RemoveMeal()
        {
            _itemType = ItemType.None;
        }

        private void AddToMealCache(string mealName, GameObject meal)
        {
            _cachedMeals.TryAdd(mealName, meal);
        }

        private GameObject GetMealFromCache(string mealName)
        {
            return _cachedMeals.TryGetValue(mealName, out GameObject go) ? go : null;
        }
    }
}