using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    public UnitGroup unitGroup;
    public TileMap mapPrefab;
    public MouseManager mouseManagerPrefab;
    public override void InstallBindings()
    {
        Container.Bind<UnitGroup>().FromInstance(unitGroup).AsSingle();
        Container.Bind<TileMap>().FromComponentInNewPrefab(mapPrefab).AsSingle();
        Container.Bind<MouseManager>().FromComponentInNewPrefab(mouseManagerPrefab).AsSingle();
        Container.Bind<ClickableTile>().AsSingle();
        Container.Bind<SelectionIndicator>().AsSingle();
        Container.Bind<Unit>().AsSingle();
    }
}