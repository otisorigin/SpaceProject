using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public int tileX;
    public int tileY;
    public TileMap map;
    private LineRenderer lineRenderer;
    public bool isPathSet;
    
    public List<Node> currentPath;

    // How far this unit can move in one turn. Note that some tiles cost extra.
    private int moveSpeed = 2;
    float remainingMovement=2;

    private void Start()
    {
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
            
            for (int i = 0; i < currentPath.Count; i++)
            {
//                var cost = map.CostToEnterTile(currentPath[i], currentPath[i < currentPath.Count-1 ? i + 1 : i]);
//                Debug.Log("COST = " + cost);
//                lineRenderer.material.color = cost > remainingMovement ? Color.red : Color.green;
                lineRenderer.SetPosition(i,  map.TileCoordToWorldCoord(currentPath[i].x, currentPath[i].y) + new Vector3(0, 0, -0.75f));
            }
            
        }
        
        // Have we moved our visible piece close enough to the target tile that we can
        // advance to the next step in our pathfinding?
        if (Vector3.Distance(transform.position, map.TileCoordToWorldCoord(tileX, tileY)) < 0.1f && isPathSet)
        {
            AdvancePathing();
        }
        
        if (currentPath == null)
        {
            isPathSet = false;
        }
            
        var position = transform.position;
        var target = Vector3.Lerp(position, map.TileCoordToWorldCoord( tileX, tileY ), 5f * Time.deltaTime);

        // Smoothly animate towards the correct map tile.
        if(position != target && currentPath != null)
        {
            transform.rotation = Rotate(position, target);
        }
        transform.position = target;
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
    void AdvancePathing() {
        if (currentPath == null)
        {
            return;
        }
            
        if (remainingMovement <= 0)
        {
            return;
        }
            

        // Teleport us to our correct "current" position, in case we
        // haven't finished the animation yet.
        transform.position = map.TileCoordToWorldCoord( tileX, tileY );

        // Get cost from current tile to next tile
        remainingMovement -= map.CostToEnterTile(currentPath[0], currentPath[1] );
		
        // Move us to the next tile in the sequence
        tileX = currentPath[1].x;
        tileY = currentPath[1].y;
		
        // Remove the old "current" tile from the pathfinding list
        currentPath.RemoveAt(0);
		
        if(currentPath.Count == 1) {
            // We only have one tile left in the path, and that tile MUST be our ultimate
            // destination -- and we are standing on it!
            // So let's just clear our pathfinding info.
            lineRenderer.enabled = false;
            currentPath = null;
        }
    }

    // The "Next Turn" button calls this.
    public void NextTurn() {
        // Make sure to wrap-up any outstanding movement left over.
        while(currentPath!=null && remainingMovement > 0) {
            AdvancePathing();
        }

        // Reset our available movement points.
        remainingMovement = moveSpeed;
    }
}
