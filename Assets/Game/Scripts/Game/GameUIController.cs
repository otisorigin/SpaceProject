using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameUIController : MonoBehaviour
{
    [Inject] private UnitManager _unitManager;
    [Inject] private GameManager _gameManager;

    public Button resetPathButton;
    public Button defModeButton;
    public Button nextTurnButton;
    public Text playerNumber;

    private void Awake()
    {
        _gameManager.OnEndOfTurnState += HandleOnEndOfTurnState;
        _gameManager.OnUnitMovementState += HandleOnUnitMovementState;
        _gameManager.OnUnitSelectionState += HandleOnUnitSelectionState;
    }

    public void InitUiController()
    {
        playerNumber.text = _gameManager.CurrentPlayer.Name;
    }

    public void ButtonAttack()
    {
        //TODO Change state from UnitMovement to UnitAttack
    }

    public void ButtonDefence()
    {
        if (_unitManager.SelectedUnit != null)
        {
            _unitManager.SelectedUnitDefenceMode();
        }
    }

    public void ButtonResetPath()
    {
        if (_unitManager.SelectedUnit != null)
        {
            var unit = _unitManager.GetSelectedUnitMovementSystem();
            unit.CurrentPath = null;
            unit.isPathSet = false;
        }
    }

    public void ButtonNextTurn()
    {
        if (_gameManager.CurrentState.Equals(GameManager.GameState.EndOfTurn))
        {
            //ChangeNextButtonText(Constants.Texts.NextUnit);
            _gameManager.NextTurn();
            playerNumber.text = _gameManager.CurrentPlayer.Name;
        }

        if (_gameManager.CurrentState.Equals(GameManager.GameState.UnitSelection) || _gameManager.CurrentState.Equals(GameManager.GameState.UnitMovement))
        {
            _unitManager.SelectRandomUnit(_unitManager.SelectedUnit);
        }
    }

    private void HandleOnEndOfTurnState()
    {
        SetActiveUnitUI(false);
        ChangeNextButtonText(Constants.Texts.NextTurn);
    }

    private void HandleOnUnitMovementState()
    {
        SetActiveUnitUI(true);
    }

    private void HandleOnUnitSelectionState()
    {
        SetActiveUnitUI(false);
        ChangeNextButtonText(Constants.Texts.NextUnit);
    }

    private void SetActiveUnitUI(bool isActive)
    {
        defModeButton.gameObject.SetActive(isActive);
        resetPathButton.gameObject.SetActive(isActive);
    }

    private void ChangeNextButtonText(String text)
    {
        nextTurnButton.GetComponentInChildren<Text>().text = text;
    }
}