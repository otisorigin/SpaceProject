﻿using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class Unit : MonoBehaviour
{
    //-------------------------------------------
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;
    //-------------------------------------------
    public int travelDistance;
    //Speed and smooth movement on the screen
    public float speed;
    [NonSerialized] public bool isPathSet;
    public List<Node> CurrentPath { get; set; }
    public int tileX { get; set; }
    public int tileY { get; set; }
    // How far this unit can move in one turn. Note that some tiles cost extra.
    private int _remainingMovement;
    //----------------------------------------------------
    [SerializeField] private int maxHealth;
    private int _currentHealth;
    private Text _healthCounter;
    public event Action<float> OnHealthPctChanged = delegate { };
    //----------------------------------------------------
    private LineRenderer _lineRenderer;
    //public IPathFindingGraph Graph { get; set; }

    private void Start()
    {
        _currentHealth = maxHealth;
        InitHealthCounter();
        _remainingMovement = travelDistance;
        //GetComponent<Canvas>().enabled = true;
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.material.color = Constants.Colors.DarkGreen;
        tileX = (int)transform.position.x;
        tileY = (int) transform.position.y;
        SetHealthBarColor();
        _manager.OnNextTurn += NextTurn;
    }
    
    void Update()
    {
        //------------------------------
        if(Input.GetKeyDown(KeyCode.Minus))
            ModifyHealth(-10);
        //---------------------------
        HealthBarPosition();
        if (IsSelected(this))
        {
            if (_manager.CurrentState == GameManager.GameState.UnitMovement)
            {
                UnitMovement();
            }
            if (_manager.CurrentState == GameManager.GameState.UnitAttack)
            {
                UnitAttack();
            }
        }
    }
    
    public int GetScale()
    {
        var xScale = (int)transform.GetChild(0).localScale.x;
        var yScale = (int)transform.GetChild(0).localScale.y;
        return xScale > yScale ? xScale : yScale;
    }
    
    public void ModifyHealth(int amount)
    {
        var updatedHealth = _currentHealth + amount;
        if (updatedHealth >= 0 && updatedHealth <= maxHealth)
        {
            _currentHealth = updatedHealth;
            float currentHealthPct = _currentHealth / (float)maxHealth;
            OnHealthPctChanged(currentHealthPct);
            UpdateHealthCounter();
        }
    }

    private void NextTurn()
    {
        SetHealthBarColor();
    }

    private void InitHealthCounter()
    {
        _healthCounter = transform.GetComponentInChildren<Text>();
        UpdateHealthCounter();
    }

    private void UpdateHealthCounter()
    {
        _healthCounter.text = _currentHealth + "/" + maxHealth; 
    }

    private void SetHealthBarColor()
    {
        transform.GetComponentsInChildren<Image>()[1].color = _manager.IsUnitOfCurrentPlayer(this) ?
            Constants.Colors.LightGreen : Constants.Colors.Red;
    }

    private void HealthBarPosition()
    {
        var unitRotation = transform.GetChild(0).transform.rotation;
        if (unitRotation.z <= 1.0f && unitRotation.z >= 0.7 || unitRotation.z <= -0.7f)
        {
            ChangeHealthBarPosition(2.0f);
        }
        if (unitRotation.z <= 0.7f && unitRotation.z >= 0.0f || unitRotation.z >= -0.7f && unitRotation.z <= 0.0f)
        {
           ChangeHealthBarPosition(-2.0f);
        }
    }

    private void ChangeHealthBarPosition(float delta)
    {
        var healthBarTransform = transform.GetChild(1).transform;
        if (!healthBarTransform.position.y.Equals(transform.position.y + delta))
        {
            healthBarTransform.position = new Vector3(transform.position.x, transform.position.y + delta, transform.position.z-1);
        }
    }

    private void UnitAttack()
    {
        
    }

    private void UnitMovement()
    {
        if (CurrentPath != null)
        {
            _lineRenderer.positionCount = CurrentPath.Count;
            _lineRenderer.enabled = true;

            for (var i = 0; i < CurrentPath.Count; i++)
            {
                _lineRenderer.SetPosition(i,
                    _map.TileCoordToWorldCoord((int)CurrentPath[i].x, (int)CurrentPath[i].y) + new Vector3(0, 0, Constants.Coordinates.ZAxisUI));
            }
        }

        var target = _map.TileCoordToWorldCoord(tileX, tileY);
        var position = transform.position;
        
        if (Vector3.Distance(position, target) > 0.1f)
        {
            var rotation = position.x.Equals(tileX) && position.y.Equals(tileY)
                ? Rotate(position, target,0)
                : Rotate(position, target);
            transform.GetChild(0).rotation = Quaternion.Slerp(transform.GetChild(0).rotation, rotation, Time.deltaTime * speed);
        }
        // Have we moved our visible piece close enough to the target tile that we can
        // advance to the next step in our pathfinding?
        if (Vector3.Distance(position, target) < 0.1f && isPathSet)
        {
            AdvancePathing();
            // Smoothly animate towards the correct map tile.
        }
 
        transform.position = Vector3.Lerp(position, target, speed / 3.5f * Time.deltaTime);

        if (CurrentPath == null)
        {
            isPathSet = false;
        }
    }

    private Quaternion Rotate(Vector3 position, Vector3 target, int zRotation = -90)
    {
        var relativePos = target - position;
        var angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rotation *= Quaternion.Euler(0, 0, zRotation); // this adds a 90 degrees Y rotation
        return rotation;
    }

    // Advances our pathfinding progress by one tile.
    // Advances our pathfinding progress by one tile.
    private void AdvancePathing()
    {
        if (CurrentPath == null || _remainingMovement <= 0)
        {
            return;
        }

        // Teleport us to our correct "current" position, in case we
        // haven't finished the animation yet.
        transform.position = _map.TileCoordToWorldCoord(tileX, tileY);

        int pathLength = CalculatePathLength();
        //Debug.Log("Path Length: " + pathLength);
        // Get cost from current tile to next tile
        for (int i = 1; i < pathLength + 1; i++)
        {
            var cost = (int) _map.CostToEnterTile(CurrentPath[0], CurrentPath[i]);
            if (_remainingMovement - cost > 0)
            {
                _remainingMovement -= cost;
                continue;
            }

            if (_remainingMovement - cost == 0)
            {
                _remainingMovement -= cost;
                pathLength = i;
                break;
            }

            if (_remainingMovement - cost < 0)
            {
                pathLength = i - 1;
                break;
            }

            //remainingMovement -= (int) map.CostToEnterTile(currentPath[0], currentPath[i] );
        }

        if (pathLength > 0)
        {
            // Move us to the next tile in the sequence
            tileX = (int)CurrentPath[pathLength].x;
            tileY = (int)CurrentPath[pathLength].y;
            // Remove the old "current" tile from the pathfinding list
            CurrentPath.RemoveRange(0, pathLength);
        }
        else
        {
            _remainingMovement = 0;
        }

        if (CurrentPath.Count == 1)
        {
            // We only have one tile left in the path, and that tile MUST be our ultimate
            // destination -- and we are standing on it!
            // So let's just clear our pathfinding info.
            _lineRenderer.enabled = false;
            CurrentPath = null;
        }
    }

    //Find path length before first 
    private int CalculatePathLength()
    {
        var possiblePathLength =
            CurrentPath.Count > _remainingMovement ? (int) _remainingMovement : CurrentPath.Count - 1;
        int pathLength;
        for (pathLength = 1; pathLength <= possiblePathLength - 1; pathLength++)
        {
            var horizontalMoving = CurrentPath[pathLength - 1].x.Equals(CurrentPath[pathLength].x) &&
                                   CurrentPath[pathLength].x.Equals(CurrentPath[pathLength + 1].x) &&
                                   !CurrentPath[pathLength - 1].y.Equals(CurrentPath[pathLength].y) &&
                                   !CurrentPath[pathLength].y.Equals(CurrentPath[pathLength + 1].y);
            var verticalMoving = CurrentPath[pathLength - 1].y.Equals(CurrentPath[pathLength].y) &&
                                 CurrentPath[pathLength].y.Equals(CurrentPath[pathLength + 1].y) &&
                                 !CurrentPath[pathLength - 1].x.Equals(CurrentPath[pathLength].x) &&
                                 !CurrentPath[pathLength].x.Equals(CurrentPath[pathLength + 1].x);
            var diagonalMoving = !CurrentPath[pathLength - 1].y.Equals(CurrentPath[pathLength].y) &&
                                 !CurrentPath[pathLength].y.Equals(CurrentPath[pathLength + 1].y) &&
                                 !CurrentPath[pathLength - 1].x.Equals(CurrentPath[pathLength].x) &&
                                 !CurrentPath[pathLength].x.Equals(CurrentPath[pathLength + 1].x);
            if (!horizontalMoving && !verticalMoving && !diagonalMoving)
            {
                return pathLength;
            }
        }

        return pathLength;
    }

    private bool IsSelected(Unit unit)
    {
        return _unitManager.SelectedUnit != null && unit == _unitManager.SelectedUnit;
    }
}