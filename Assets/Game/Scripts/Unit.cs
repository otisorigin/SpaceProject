using System;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int travelDistance;
    //Speed and smooth movement on the screen
    public float speed;
    public TileMap map;
    [NonSerialized]
    public bool isPathSet;
    [NonSerialized]
    public List<Node> currentPath;
    [NonSerialized]
    public int tileX;
    [NonSerialized]
    public int tileY;

    private LineRenderer lineRenderer;
    // How far this unit can move in one turn. Note that some tiles cost extra.
    float remainingMovement;

    private void Start()
    {
        remainingMovement = travelDistance;
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;
        lineRenderer.useWorldSpace = true;
        lineRenderer.material.color = Color.green;
    }

    void Update()
    {   
        if (currentPath != null)
        {
            lineRenderer.positionCount = currentPath.Count;
            lineRenderer.enabled = true;
            
            for (var i = 0; i < currentPath.Count; i++)
            {
                lineRenderer.SetPosition(i,  map.TileCoordToWorldCoord(currentPath[i].x, currentPath[i].y) + new Vector3(0, 0, -0.75f));
            }
            
        }
        var target = map.TileCoordToWorldCoord(tileX, tileY);
        var position = transform.position;
        // Have we moved our visible piece close enough to the target tile that we can
        // advance to the next step in our pathfinding?
        if (Vector3.Distance(position, target) < 0.1f && isPathSet)
        {
            AdvancePathing();
        }
        if (currentPath == null)
        {
            isPathSet = false;
        }
        // Smoothly animate towards the correct map tile.
        transform.rotation = Quaternion.Slerp(transform.rotation, Rotate(position, target),  Time.deltaTime * speed);
        transform.position = Vector3.Lerp(position, target, speed/3.5f * Time.deltaTime);
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
    private void AdvancePathing() {
        if (currentPath == null || remainingMovement <= 0)
        {
            return;
        }
        // Teleport us to our correct "current" position, in case we
        // haven't finished the animation yet.
        transform.position = map.TileCoordToWorldCoord( tileX, tileY );

        int pathLength = CalculatePathLength();
        //Debug.Log("Path Length: " + pathLength);
        // Get cost from current tile to next tile
        for (int i = 1; i < pathLength+1; i++)
        {
            remainingMovement -= map.CostToEnterTile(currentPath[0], currentPath[i] );
        }
       
        // Move us to the next tile in the sequence
        tileX = currentPath[pathLength].x;
        tileY = currentPath[pathLength].y;
        // Remove the old "current" tile from the pathfinding list
        currentPath.RemoveRange(0, pathLength);
		
        if(currentPath.Count == 1) {
            // We only have one tile left in the path, and that tile MUST be our ultimate
            // destination -- and we are standing on it!
            // So let's just clear our pathfinding info.
            lineRenderer.enabled = false;
            currentPath = null;
        }
    }
    
    //Find path length before first 
    private int CalculatePathLength() 
    {
        var possiblePathLength = currentPath.Count > remainingMovement ? (int) remainingMovement : currentPath.Count - 1;
        int pathLength;
        for (pathLength = 1; pathLength <= possiblePathLength-1; pathLength++)
        {
            var horizontalMoving = currentPath[pathLength - 1].x == currentPath[pathLength].x &&
                                   currentPath[pathLength].x == currentPath[pathLength + 1].x &&
                                   currentPath[pathLength - 1].y != currentPath[pathLength].y &&
                                   currentPath[pathLength].y != currentPath[pathLength + 1].y;
            var verticalMoving = currentPath[pathLength - 1].y == currentPath[pathLength].y &&
                                 currentPath[pathLength].y == currentPath[pathLength + 1].y &&
                                 currentPath[pathLength - 1].x != currentPath[pathLength].x &&
                                 currentPath[pathLength].x != currentPath[pathLength + 1].x;
            var diagonalMoving = currentPath[pathLength - 1].y != currentPath[pathLength].y &&
                                 currentPath[pathLength].y != currentPath[pathLength + 1].y &&
                                 currentPath[pathLength - 1].x != currentPath[pathLength].x &&
                                 currentPath[pathLength].x != currentPath[pathLength + 1].x;
            if(!horizontalMoving && !verticalMoving && !diagonalMoving)
            {
                return pathLength;
            }
        }
        return pathLength;
    }

    // The "Next Turn" button calls this.
    public void NextTurn() {
        // Make sure to wrap-up any outstanding movement left over.
        while(currentPath!=null && remainingMovement > 0) {
            AdvancePathing();
        }
        // Reset our available movement points.
        remainingMovement = travelDistance;
    }
}
