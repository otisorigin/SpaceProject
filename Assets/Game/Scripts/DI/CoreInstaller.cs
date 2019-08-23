using UnityEngine;
using Zenject;

public class CoreInstaller : MonoInstaller<CoreInstaller>
{
    public override void InstallBindings()
    {
        Container.Bind<IUnitGroup>().To<UnitGroup>().AsSingle();
        Container.Bind<ITileMap>().To<TileMap>().AsSingle();
        Container.Bind<IMouseManager>().To<MouseManager>().AsSingle();
//        Container.Bind<MouseManager>().AsSingle().NonLazy();
//        Container.Bind<TileMap>().AsSingle().NonLazy();
//        Container.Bind<ClickableTile>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
//        Container.Bind<SelectionIndicator>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        Container.Bind<Unit>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
    }
}