using System.Linq;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private Player _player1;
    private Player _player2;
    public Player CurrentPlayer { get; private set; }
    
    public Unit SelectedUnit { get; private set; }
    public Unit[] firstPlayerUnitGroup;
    public Unit[] secondPlayerUnitGroup;
    
    public enum GameState {UnitSelection, UnitMovement, UnitAttack};

    public GameState CurrentState { get; private set; }

    private void Start()
    {
        _player1 = new Player("1", firstPlayerUnitGroup);
        _player2 = new Player("2", secondPlayerUnitGroup);
        CurrentPlayer = _player1;
        ChangeGameState(GameState.UnitSelection);
    }

    void Update()
    {
        //_currentGameState.
    }

    public void ButtonAttack()
    {
        //TODO Change state from UnitMovement to UnitAttack
    }

    public void ButtonResetPath()
    {
        if (SelectedUnit != null)
        {
            SelectedUnit.CurrentPath = null;
            SelectedUnit.isPathSet = false;
        }
    }
    
    public void ButtonNextTurn()
    {
        Debug.Log("Current player = " + CurrentPlayer.Name);
        CurrentPlayer = CurrentPlayer == _player1 ? _player2 : _player1;
    }
    
    public void UnitSelect(Unit selectedUnit)
    {
        ChangeGameState(GameState.UnitMovement);
        SelectedUnit = selectedUnit;
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
}