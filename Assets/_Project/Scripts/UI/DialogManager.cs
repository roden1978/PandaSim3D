using System;
using System.Collections.Generic;
using Infrastructure.AssetManagement;
using UnityEngine;

namespace UI
{
    public class DialogManager
    {
        private readonly GuiHolder _guiHolder;
        private readonly IPrefabsStorage _prefabsStorage;

        public DialogManager(GuiHolder guiHolder, IPrefabsStorage prefabsStorage)
        {
            _guiHolder = guiHolder;
            _prefabsStorage = prefabsStorage;
        }

        // При создании новых окон добавлять их сюда
        private readonly Dictionary<Type, string> PrefabsDictionary = new()
        {
            { typeof(ShopDialog), AssetPaths.ShopDialog },
            { typeof(MealInventoryDialog), AssetPaths.MealInventoryDialog },
            { typeof(ClothsInventoryDialog), AssetPaths.ClothsInventoryDialog },
            { typeof(BackpackInventoryDialog), AssetPaths.BackpackInventoryDialog },
            { typeof(ToysInventoryDialog), AssetPaths.ToysInventoryDialog },
            { typeof(InputNameDialog), AssetPaths.InputNameDialog },
            { typeof(MenuDialog), AssetPaths.MenuDialog },
            { typeof(GameOverDialog), AssetPaths.GameOverDialog },
        };

        public T ShowDialog<T>() where T : Dialog
        {
            T component = _guiHolder.GetComponentInChildren<T>(true);

            if (component is not null)
            {
                Canvas canvas = component.GetComponentInChildren<Canvas>(true);
                if(canvas is not null)
                    canvas.gameObject.SetActive(true);
                return component;
            }

            return null;
        }

        private T GetPrefabByType<T>() where T : Dialog
        {
            string prefabName = PrefabsDictionary[typeof(T)];
            if (string.IsNullOrEmpty(prefabName))
            {
                Debug.LogError($"Cant find prefab type of {typeof(T)} Do you added it in PrefabsDictionary?");
            }

            string path = PrefabsDictionary[typeof(T)];
            T dialog = _prefabsStorage.Get(Type.GetType(path)).GetComponent<T>();
            if (dialog is null)
                Debug.LogError($"Cant find prefab at path {path}");

            return dialog;
        }
    }
}