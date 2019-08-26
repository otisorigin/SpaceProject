using UnityEngine;

public class GameController : MonoBehaviour
{
    private Player _player1;
    private Player _player2;
    public Player CurrentPlayer { get; set; }
    
    public Unit SelectedUnit { get; set; }
    public Unit[] firstPlayerUnitGroup;
    public Unit[] secondPlayerUnitGroup;

    private IGameState _gameState;

    private void Start()
    {
        _player1 = new Player("Player_1", firstPlayerUnitGroup);
        _player2 = new Player("Player_2", secondPlayerUnitGroup);
        CurrentPlayer = _player1;
        ChangeGameState(new UnitSelection());
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
        CurrentPlayer = CurrentPlayer == _player1 ? _player2 : _player1;
    }
    
    public void UnitSelect(Unit selectedUnit)
    {
        SelectedUnit = selectedUnit;
    }

    private void ChangeGameState(IGameState newState)
    {
        _gameState = newState;
    }
}