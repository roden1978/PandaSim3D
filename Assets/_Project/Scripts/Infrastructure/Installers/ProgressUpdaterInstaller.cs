﻿using Zenject;

public class ProgressUpdaterInstaller : MonoInstaller
{
    private PrefabsStorage _prefabsStorage;

    [Inject]
    public void Construct(PrefabsStorage prefabsStorage)
    {
        _prefabsStorage = prefabsStorage;
    }
    public override void InstallBindings()
    {
        BindProgressUpdater();
    }
    private void BindProgressUpdater()
    {
        Container.InstantiatePrefab(_prefabsStorage.Get(typeof(ProgressUpdater)));
    }
}