using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Unit : MonoBehaviour
{
    //----------------Injections-----------------
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;

    //-----------------Parameters-----------------
    public int travelDistance;
    //Speed and smooth movement on the screen
    public float speed;
    [SerializeField] private int maxHealth;
    [SerializeField] private int scale;

    private LineRenderer _lineRenderer;

    private void Start()
    {
        transform.GetComponentInChildren<HealthSystem>().InitHealth(maxHealth);
        var lineRenderer = GetComponent<LineRenderer>();
        transform.GetComponentInChildren<MovementSystem>().InitMovement(travelDistance, speed, lineRenderer);
        //_manager.OnNextTurn += NextTurn;
    }

    void Update()
    {
        if (IsSelected(this))
        {
            if (_manager.CurrentState == GameManager.GameState.UnitAttack)
            {
                UnitAttack();
            }
        }
    }

    public int GetScale()
    {
        return scale;
    }

    private void NextTurn()
    {
    }

    private void UnitAttack()
    {
    }

    private bool IsSelected(Unit unit)
    {
        return _unitManager.SelectedUnit != null && unit == _unitManager.SelectedUnit;
    }
}