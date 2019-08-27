using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    //public UnitGroup unitGroup;
    public GameController controller;
    public TileMap mapPrefab;
    public CursorManager cursorManagerPrefab;
    public override void InstallBindings()
    {
        //Container.Bind<UnitGroup>().FromInstance(unitGroup).AsSingle();
        Container.Bind<GameController>().FromInstance(controller).AsSingle();
        Container.Bind<TileMap>().FromComponentInNewPrefab(mapPrefab).AsSingle();
        Container.Bind<CursorManager>().FromComponentInNewPrefab(cursorManagerPrefab).AsSingle();
        Container.Bind<ClickableTile>().AsSingle();
        Container.Bind<SelectionManager>().AsSingle();
        Container.Bind<Unit>().AsSingle();
    }
}