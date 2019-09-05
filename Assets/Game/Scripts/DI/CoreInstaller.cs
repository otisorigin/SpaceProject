using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    //public UnitGroup unitGroup;
    public GameManager controller;
    public TileMap mapPrefab;
    public CursorManager cursorManagerPrefab;
    public SelectionManager selectionManager;
    
    public override void InstallBindings()
    {
        //Container.Bind<UnitGroup>().FromInstance(unitGroup).AsSingle();
        Container.Bind<GameManager>().FromInstance(controller).AsSingle();
        Container.Bind<TileMap>().FromComponentInNewPrefab(mapPrefab).AsSingle();
        Container.Bind<CursorManager>().FromComponentInNewPrefab(cursorManagerPrefab).AsSingle();
        Container.Bind<ClickableTile>().AsSingle();
        Container.Bind<SelectionManager>().FromInstance(selectionManager).AsSingle();
        Container.Bind<Unit>().AsSingle();
        Container.Bind<SelectedUnitIndicator>().AsSingle();
    }
}