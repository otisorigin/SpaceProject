using System;
using System.Linq;
using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    private Player _player1;
    private Player _player2;
    [Inject] private UnitManager _unitManager;
    [Inject] private TileMap _tileMap;
    [Inject] private GameUIController _uiController;
    //[Inject] private CameraManager _camera;
    public Player CurrentPlayer { get; private set; }
    
    public enum GameState {UnitSelection, UnitMovement, UnitAttack, EndOfTurn};
    
    public event Action OnNextTurn = delegate { };
    public event Action OnEndOfTurnState = delegate { };
    public event Action OnUnitMovementState = delegate { };
    public event Action OnUnitSelectionState = delegate { };
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        _tileMap.InitTileMap();
        _unitManager.InitPlayerUnits();
        _player1 = new Player("1", _unitManager.firstPlayerUnitGroup);
        _player2 = new Player("2", _unitManager.secondPlayerUnitGroup);
        CurrentPlayer = _player1;
        _uiController.InitUiController();
       // _camera.InitCamera();
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

    public void NextTurn()
    {
        Debug.Log("Current player = " + CurrentPlayer.Name);
        CurrentPlayer = CurrentPlayer == _player1 ? _player2 : _player1;
        ChangeGameState(GameState.UnitSelection);
        OnNextTurn();
    }

    public void ChangeGameState(GameState newState)
    {
        CurrentState = newState;
        if (newState.Equals(GameState.EndOfTurn))
        {
            OnEndOfTurnState();
        }
        if (newState.Equals(GameState.UnitMovement))
        {
            OnUnitMovementState();
        }
        if (newState.Equals(GameState.UnitSelection))
        {
            OnUnitSelectionState();
        }
        Debug.Log("Current game state: " + CurrentState);
    }

    public bool IsUnitOfCurrentPlayer(Unit unit)
    {
        return CurrentPlayer.UnitGroup.Contains(unit);
    }


}