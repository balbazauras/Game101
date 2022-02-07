using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMovement : MonoBehaviour
{
    List<Tile> selectableTiles = new List<Tile>();
    GameObject[] tiles;
    Stack<Tile> path = new Stack<Tile>();
    Tile currentTile;
    public int move = 5;
    public float jumpheight = 2;
    public float movespeed = 2;
    public bool moving;
    Vector3 velocity = new Vector3();
    Vector3 heading = new Vector3();
    float halfheight = 0;
    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfheight = GetComponent<Collider>().bounds.extents.y;
    }
    public void GetCurrenTile()
    {
        currentTile = GetTargetTile(gameObject);
        currentTile.current = true;
    }
    public Tile GetTargetTile(GameObject target)
    {
        RaycastHit hit;
        Tile tile = null;
        if (Physics.Raycast(target.transform.position, -Vector3.up, out hit, 1))
        {
            tile = hit.collider.GetComponent<Tile>();
        }
        return tile;
    }
    public void ComputeAdjancent()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach (GameObject tile in tiles)
        {
            Tile t = tile.GetComponent<Tile>();
            t.FindNeighbours(jumpheight);
        }
    }
    public void FindSeslectableTiles()
    {
        ComputeAdjancent();
        GetCurrenTile();
        Queue<Tile> process = new Queue<Tile>();
        process.Enqueue(currentTile);
        currentTile.visited = true;

        while (process.Count > 0)
        {
            Tile t = process.Dequeue();
            selectableTiles.Add(t);
            t.selectable = true;
            if (t.distance < move)
            {
                foreach (Tile tile in t.adjacent)
                {
                    if (!tile.visited)
                    {
                        tile.parent = t;
                        tile.visited = true;
                        tile.distance = 1 + t.distance;
                        process.Enqueue(tile);
                    }
                }
            }
        }

    }
    public void MoveToTile(Tile tile)
    {
        path.Clear();
        tile.target = true;
        moving = true;
        Tile next = tile;
        while (next != null)
        {
            path.Push(next);
            next = next.parent;
        }
    }
    public void Move()
    {
        if (path.Count > 0)
        {
            Tile t = path.Peek();
            Vector3 target = t.transform.position;
            //Calculate the units positon on  top of the target tile
            target.y += halfheight + t.GetComponent<Collider>().bounds.extents.y;
            if (Vector3.Distance(transform.position, target) >= 0.05f)
            {
                CalculateHeading(target);
                SetHorizontalVelocity();
     
                    transform.forward = heading;
                    transform.position += velocity * Time.deltaTime;
                
       
            }
            else
            {
                //Tile center reached
                transform.position = target;
                path.Pop();
            }
        }
        else
        {
            RemoveSeletableTiles();
            moving = false;
        }
    }
    protected void RemoveSeletableTiles()
    {
        if (currentTile != null)
        {
            currentTile.current = false;
            currentTile = null;
        }
        foreach (Tile tile in selectableTiles)
        {
            tile.Reset();
        }
        selectableTiles.Clear();
    }
    public void CalculateHeading(Vector3 target)
    {
        heading = target - transform.position;
        heading.Normalize();
    }
    public void SetHorizontalVelocity()
    {
        velocity = heading * movespeed;
    }
}
