using Zenject;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    public GameManager controller;
    public TileMap mapPrefab;
    public CursorManager cursorManagerPrefab;
    public SelectionManager selectionManager;
    public UnitManager unitManager;
    public PathFindingGraph1x1 graph1X1;
    public PathFindingGraph2x2 graph2X2;
    public PathFindingGraph3x3 graph3X3;

    public override void InstallBindings()
    {
        Container.Bind<GameManager>().FromInstance(controller).AsSingle();
        Container.Bind<Unit>().AsSingle();
        Container.Bind<HealthSystem>().AsSingle();
        Container.Bind<ClickableTile>().AsSingle();
        Container.Bind<SelectedUnitIndicator>().AsSingle();
        Container.Bind<TileMap>().FromComponentInNewPrefab(mapPrefab).AsSingle();
        Container.Bind<CursorManager>().FromComponentInNewPrefab(cursorManagerPrefab).AsSingle();
        Container.Bind<SelectionManager>().FromInstance(selectionManager).AsSingle();
        Container.Bind<UnitManager>().FromInstance(unitManager).AsSingle();
        Container.Bind<PathFindingGraph1x1>().FromComponentInNewPrefab(graph1X1).AsSingle();
        Container.Bind<PathFindingGraph2x2>().FromComponentInNewPrefab(graph2X2).AsSingle();
        Container.Bind<PathFindingGraph3x3>().FromComponentInNewPrefab(graph3X3).AsSingle();
    }
}