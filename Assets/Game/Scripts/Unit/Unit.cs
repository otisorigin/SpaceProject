using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class Unit : MonoBehaviour
{
    public int travelDistance;
    //Speed and smooth movement on the screen
    public float speed;
    [Inject] private TileMap _map;
    [Inject] private GameManager _manager;
    [NonSerialized] public bool isPathSet;
    public List<Node> CurrentPath { get; set; }
    [NonSerialized] public int tileX;
    [NonSerialized] public int tileY;

    private LineRenderer lineRenderer;

    // How far this unit can move in one turn. Note that some tiles cost extra.
    int remainingMovement;

    private void Start()
    {
        remainingMovement = travelDistance;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material.color = Constants.Colors.DarkGreen;
    }

    void Update()
    {
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

//    private void OnMouseOver()
//    {
//        Debug.Log("On Mouse Over");
//        var defaultPosition = transform.GetChild(1).transform.position;
//        transform.GetChild(1).transform.position = new Vector3(defaultPosition.x, defaultPosition.y, 0.75f);
//    }
//
//    private void OnMouseExit()
//    {
//        Debug.Log("On Mouse Exit");
//        var defaultPosition = transform.GetChild(1).transform.position;
//        transform.GetChild(1).transform.position = new Vector3(defaultPosition.x, defaultPosition.y, 1f);
//    }

    private void UnitAttack()
    {
        
    }

    private void UnitMovement()
    {
        if (CurrentPath != null)
        {
            lineRenderer.positionCount = CurrentPath.Count;
            lineRenderer.enabled = true;

            for (var i = 0; i < CurrentPath.Count; i++)
            {
                lineRenderer.SetPosition(i,
                    _map.TileCoordToWorldCoord(CurrentPath[i].x, CurrentPath[i].y) + new Vector3(0, 0, -0.75f));
            }
        }

        var target = _map.TileCoordToWorldCoord(tileX, tileY);
        var position = transform.position;
        // Have we moved our visible piece close enough to the target tile that we can
        // advance to the next step in our pathfinding?
        if (Vector3.Distance(position, target) < 0.1f && isPathSet)
        {
            AdvancePathing();
        }

        if (CurrentPath == null)
        {
            isPathSet = false;
        }

        // Smoothly animate towards the correct map tile.
        transform.rotation = Quaternion.Slerp(transform.rotation, Rotate(position, target), Time.deltaTime * speed);
        transform.position = Vector3.Lerp(position, target, speed / 3.5f * Time.deltaTime);
    }

    private Quaternion Rotate(Vector3 position, Vector3 target)
    {
        var relativePos = target - position;
        var angle = Mathf.Atan2(relativePos.y, relativePos.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        rotation *= Quaternion.Euler(0, 0, -90); // this adds a 90 degrees Y rotation
        return rotation;
    }

    // Advances our pathfinding progress by one tile.
    // Advances our pathfinding progress by one tile.
    private void AdvancePathing()
    {
        if (CurrentPath == null || remainingMovement <= 0)
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
            if (remainingMovement - cost > 0)
            {
                remainingMovement -= cost;
                continue;
            }

            if (remainingMovement - cost == 0)
            {
                remainingMovement -= cost;
                pathLength = i;
                break;
            }

            if (remainingMovement - cost < 0)
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
            remainingMovement = 0;
        }

        if (CurrentPath.Count == 1)
        {
            // We only have one tile left in the path, and that tile MUST be our ultimate
            // destination -- and we are standing on it!
            // So let's just clear our pathfinding info.
            lineRenderer.enabled = false;
            CurrentPath = null;
        }
    }

    //Find path length before first 
    private int CalculatePathLength()
    {
        var possiblePathLength =
            CurrentPath.Count > remainingMovement ? (int) remainingMovement : CurrentPath.Count - 1;
        int pathLength;
        for (pathLength = 1; pathLength <= possiblePathLength - 1; pathLength++)
        {
            var horizontalMoving = CurrentPath[pathLength - 1].x == CurrentPath[pathLength].x &&
                                   CurrentPath[pathLength].x == CurrentPath[pathLength + 1].x &&
                                   CurrentPath[pathLength - 1].y != CurrentPath[pathLength].y &&
                                   CurrentPath[pathLength].y != CurrentPath[pathLength + 1].y;
            var verticalMoving = CurrentPath[pathLength - 1].y == CurrentPath[pathLength].y &&
                                 CurrentPath[pathLength].y == CurrentPath[pathLength + 1].y &&
                                 CurrentPath[pathLength - 1].x != CurrentPath[pathLength].x &&
                                 CurrentPath[pathLength].x != CurrentPath[pathLength + 1].x;
            var diagonalMoving = CurrentPath[pathLength - 1].y != CurrentPath[pathLength].y &&
                                 CurrentPath[pathLength].y != CurrentPath[pathLength + 1].y &&
                                 CurrentPath[pathLength - 1].x != CurrentPath[pathLength].x &&
                                 CurrentPath[pathLength].x != CurrentPath[pathLength + 1].x;
            if (!horizontalMoving && !verticalMoving && !diagonalMoving)
            {
                return pathLength;
            }
        }

        return pathLength;
    }

    // The "Next Turn" button calls this.
//    public void NextTurn()
//    {
//        //if (IsSelected(this))
//        //{
//        // Make sure to wrap-up any outstanding movement left over.
//        while (CurrentPath != null && remainingMovement > 0)
//        {
//            AdvancePathing();
//        }
//
//        // Reset our available movement points.
//        remainingMovement = travelDistance;
//        //}
//    }

    private bool IsSelected(Unit unit)
    {
        return _manager.SelectedUnit != null && unit == _manager.SelectedUnit;
    }
}