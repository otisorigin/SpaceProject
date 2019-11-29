using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class UnitManager : MonoBehaviour
{
    public Unit[] units;
    public Unit[] firstPlayerUnitGroup { get; set; }
    public Unit[] secondPlayerUnitGroup { get; set; }
    public Unit SelectedUnit { get; private set; }
    
    [Inject] private GameManager _manager;
    [Inject] private TileMap _map;
    
    private Unit previousSelectedUnit;
    // Start is called before the first frame update
    void Start()
    {
        //PathGraphInjection();
        //SetPlayersUnits();
    }

    public void InitPlayerUnits()
    {
        SetPlayersUnits();
    }

    private void SetPlayersUnits()
    {
        //TODO логика выбора юнитов игроками
        firstPlayerUnitGroup = new Unit[10];
        secondPlayerUnitGroup = new Unit[10];
        firstPlayerUnitGroup[0] = units[0];
        //var middleUnit = units[1];
        //middleUnit.tileX = middleUnit.tileX + 0.5f;
        firstPlayerUnitGroup[1] = units[1];
        firstPlayerUnitGroup[2] = units[2];
        //secondPlayerUnitGroup[0] = units[2];

        //secondPlayerUnitGroup[0] = units[2];
        //firstPlayerUnitGroup[0] = Instantiate(units[0]);
        //firstPlayerUnitGroup[1] = Instantiate(units[1]);
        //firstPlayerUnitGroup[2] = Instantiate(units[2]);
    }

//    private void PathGraphInjection()
//    {
//        _graph1x1 = new PathFindingGraph1x1();
//        _graph2x2 = new PathFindingGraph2x2();
//        _graph2x2 = new PathFindingGraph3x3();
//        units[0].Graph = _graph1x1;
//        units[1].Graph = _graph2x2;
//        units[2].Graph = _graph3x3;
//    }

    // Update is called once per frame
    void Update()
    {
        if (_manager.CurrentState == GameManager.GameState.UnitMovement)
        {
            if (previousSelectedUnit == null && SelectedUnit != null)
            {
                previousSelectedUnit = SelectedUnit;
                _map.CurrentGraph.SetDynamicObstacleNodes();
            }
            if (previousSelectedUnit != null && !previousSelectedUnit.Equals(SelectedUnit))
            {
                _map.CurrentGraph.SetDynamicObstacleNodes();
                previousSelectedUnit = SelectedUnit;
            } 
        }
    }
    
    public bool IsThisUnitSelected(Unit unit)
    {
        return SelectedUnit == unit;
    }
    
    public IEnumerable<GameObject> GetUnitObjects()
    {
        return FindObjectsOfType<GameObject>()
            .ToList()
            .Where(obj => obj.tag.Equals("Unit"))
            .Where(unit => !unit.GetComponent<Unit>().Equals(SelectedUnit));
    }
    
    public void UnitSelect(Unit selectedUnit)
    {
        _manager.ChangeGameState(GameManager.GameState.UnitMovement);
        _map.SetCurrentGraph(selectedUnit.GetScale());
        SelectedUnit = selectedUnit;
    }

    public void GeneratePathTo(int x, int y)
    {
        _map.CurrentGraph.GeneratePathTo(x,y);
    }
}
