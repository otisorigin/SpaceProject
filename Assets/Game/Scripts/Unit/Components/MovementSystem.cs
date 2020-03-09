using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class MovementSystem : MonoBehaviour
{
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [Inject] private UnitManager _unitManager;
    [NonSerialized] public bool isPathSet;
    public List<Node> CurrentPath { get; set; }
    public float tileX { get; set; }
    public float tileY { get; set; }
    public HashSet<Node> availableNodesToMove { get; set; }

    public bool IsMoving { get; set; }

    // How far this unit can move in one turn. Note that some tiles cost extra.
    public int RemainingMovement { get; set; }

    private int _travelDistance;
    private float _speed;
    private LineRenderer _lineRenderer;

    public void InitMovement(int travelDistance, float speed, LineRenderer lineRenderer)
    {
        _travelDistance = travelDistance;
        _speed = speed;
        _lineRenderer = lineRenderer;
        _lineRenderer.enabled = false;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.material.color = Constants.Colors.DarkGreen;
        RemainingMovement = travelDistance;
        IsMoving = false;
        tileX = GetUnitTransform().position.x;
        tileY = GetUnitTransform().position.y;
    }

    public void Update()
    {
        if (_manager.CurrentState == GameManager.GameState.UnitMovement &&
            _unitManager.IsThisUnitSelected(transform.GetComponentInParent<Unit>()))
        {
            UnitMovement();
        }
    }

    public void ClearCurrentPath()
    {
        CurrentPath = null;
        isPathSet = false;
        _lineRenderer.enabled = false;
    }

    public void ShowAvailableTilesToMove()
    {
        var nodes = _map.CurrentGraph.GetAvailableNodes();
        availableNodesToMove = nodes;
        _map.ShowAvailablePathTiles(nodes);
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
                    _map.TileCoordToWorldCoord(CurrentPath[i].x, CurrentPath[i].y) +
                    new Vector3(0, 0, Constants.Coordinates.ZAxisUI));
            }
        }

        var target = _map.TileCoordToWorldCoord(tileX, tileY);
        var position = GetUnitTransform().position;

        if (Vector3.Distance(position, target) < 0.1f && !isPathSet && CurrentPath == null && IsMoving)
        {
            // Debug.Log("isMoving = false");
            IsMoving = false;
            ShowAvailableTilesToMove();

            if (RemainingMovement == 0 && tileX.Equals(target.x) && tileY.Equals(target.y))
            {
                _unitManager.Invoke(nameof(UnitManager.UnitFinishMovementPerTurn), 1);
            }
        }

        if (Vector3.Distance(position, target) > 0.1f)
        {
            var rotation = position.x.Equals(tileX) && position.y.Equals(tileY)
                ? Rotate(position, target, 0)
                : Rotate(position, target);
            GetUnitTransform().GetChild(0).rotation =
                Quaternion.Slerp(GetUnitTransform().GetChild(0).rotation, rotation, Time.deltaTime * _speed);
        }

        // Have we moved our visible piece close enough to the target tile that we can
        // advance to the next step in our pathfinding?
        if (Vector3.Distance(position, target) < 0.1f && isPathSet)
        {
            AdvancePathing();
            // Smoothly animate towards the correct map tile.
        }

        GetUnitTransform().position = Vector3.Lerp(position, target, _speed / 3.5f * Time.deltaTime);

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
    private void AdvancePathing()
    {
        if (CurrentPath == null || RemainingMovement <= 0)
        {
            Debug.Log("isMoving = false");
            IsMoving = false;
            ShowAvailableTilesToMove();
            return;
        }

        // Teleport us to our correct "current" position, in case we
        // haven't finished the animation yet.
        GetUnitTransform().position = _map.TileCoordToWorldCoord(tileX, tileY);

        int pathLength = CalculatePathLength();
        //Debug.Log("Path Length: " + pathLength);
        // Get cost from current tile to next tile
        for (int i = 1; i < pathLength + 1; i++)
        {
            var cost = (int) _map.CostToEnterTile(CurrentPath[0], CurrentPath[i]);
            if (RemainingMovement - cost > 0)
            {
                RemainingMovement -= cost;
                continue;
            }

            if (RemainingMovement - cost == 0)
            {
                RemainingMovement -= cost;
                pathLength = i;
                break;
            }

            if (RemainingMovement - cost < 0)
            {
                pathLength = i - 1;
                break;
            }

            //remainingMovement -= (int) map.CostToEnterTile(currentPath[0], currentPath[i] );
        }

        if (pathLength > 0)
        {
            // Move us to the next tile in the sequence
            tileX = CurrentPath[pathLength].x;
            tileY = CurrentPath[pathLength].y;
            // Remove the old "current" tile from the pathfinding list
            CurrentPath.RemoveRange(0, pathLength);
        }
        else
        {
            RemainingMovement = 0;
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
            CurrentPath.Count > RemainingMovement ? (int) RemainingMovement : CurrentPath.Count - 1;
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

    private Transform GetUnitTransform()
    {
        return transform.parent;
    }
}