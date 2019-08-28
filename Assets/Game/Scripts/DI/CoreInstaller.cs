using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    //public UnitGroup unitGroup;
    [FormerlySerializedAs("controller")] public GameManager manager;
    public TileMap mapPrefab;
    public CursorManager cursorManagerPrefab;
    public override void InstallBindings()
    {
        //Container.Bind<UnitGroup>().FromInstance(unitGroup).AsSingle();
        Container.Bind<GameManager>().FromInstance(manager).AsSingle();
        Container.Bind<TileMap>().FromComponentInNewPrefab(mapPrefab).AsSingle();
        Container.Bind<CursorManager>().FromComponentInNewPrefab(cursorManagerPrefab).AsSingle();
        Container.Bind<ClickableTile>().AsSingle();
        Container.Bind<SelectionManager>().AsSingle();
        Container.Bind<Unit>().AsSingle();
    }
}