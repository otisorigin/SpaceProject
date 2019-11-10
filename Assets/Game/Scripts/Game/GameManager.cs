using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private Player _player1;
    private Player _player2;
    [Inject] private UnitManager _unitManager;
    [Inject] private TileMap _tileMap;
    public Player CurrentPlayer { get; private set; }
    
    public enum GameState {UnitSelection, UnitMovement, UnitAttack};
    
    public event Action OnNextTurn = delegate { };

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        _tileMap.InitTileMap();
        _unitManager.InitPlayerUnits();
        _player1 = new Player("1", _unitManager.firstPlayerUnitGroup);
        _player2 = new Player("2", _unitManager.secondPlayerUnitGroup);
        CurrentPlayer = _player1;
        ChangeGameState(GameState.UnitSelection);
        //SetUnitPosition();
        
    }

    void Update()
    {
        if (CurrentState == GameState.UnitMovement)
        {
            //смотреть не null ли предыдущий выбранный юнит и в зависимости от типа юнита генерить dynamicObstacles
        }
    }

    public void ButtonAttack()
    {
        //TODO Change state from UnitMovement to UnitAttack
    }

    public void ButtonResetPath()
    {
        if (_unitManager.SelectedUnit != null)
        {
            _unitManager.SelectedUnit.CurrentPath = null;
            _unitManager.SelectedUnit.isPathSet = false;
        }
    }
    
    public void ButtonNextTurn()
    {
        Debug.Log("Current player = " + CurrentPlayer.Name);
        CurrentPlayer = CurrentPlayer == _player1 ? _player2 : _player1;
        OnNextTurn();
    }

    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log("Current game state: " + CurrentState);
    }

    public bool IsUnitOfCurrentPlayer(Unit unit)
    {
        return CurrentPlayer.UnitGroup.Contains(unit);
    }


    
//    private void SetUnitPosition()
//    {
//        if (SelectedUnit != null)
//        {
//            var unit = SelectedUnit.GetComponent<Unit>();
//            unit.tileX = (int) unit.transform.position.x;
//            unit.tileY = (int) unit.transform.position.y;
//            // unit.map = this;
//        }
//    }

 
}