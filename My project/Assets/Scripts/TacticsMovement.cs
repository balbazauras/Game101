using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TacticsMovement : MonoBehaviour
{
    public bool turn = false;
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
    bool fallingDown = false;
    bool jumpingUp = false;
    bool movingEdge = false;
    Vector3 jumpTarget;
    public float jumpVelocity = 4.5f;
    public float minimumDistanceToEdge;

    protected void Init()
    {
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        halfheight = GetComponent<Collider>().bounds.extents.y;
        TurnManager.AddUnit(this);
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
            if (Vector3.Distance(transform.position, target) >= 0.1f)
            {
                bool jump = transform.position.y != target.y;
                if (jump)
                {
                    Jump(target);
                }
                else
                {
                    CalculateHeading(target);
                    SetHorizontalVelocity();
                }
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
            TurnManager.EndTurn();
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
    void Jump(Vector3 target)
    {
        if (fallingDown)
        {
            FallingDownward(target);
        }
        else if (jumpingUp)
        {
            JumpUpward(target);
        }
        else if (movingEdge)
        {
            MoveToEdge();
        }
        else
        {
            PrepareJump(target);
        }

    }
    void PrepareJump(Vector3 target)
    {
        float targetY = target.y;
        target.y = transform.position.y;
        CalculateHeading(target);
        if (transform.position.y > targetY)
        {
            fallingDown = false;
            jumpingUp = false;
            movingEdge = true;
            minimumDistanceToEdge = Vector3.Distance(transform.position, jumpTarget);
            jumpTarget = transform.position + (target - transform.position / 2.0f);
            
        }
        else
        {
            fallingDown = false;
            jumpingUp = true;
            movingEdge = false;
            velocity = heading * movespeed / 3.0f;
            float difference = targetY - transform.position.y;
            velocity.y = jumpVelocity * (0.5f + difference / 2.0f);

        }
    }
    void FallingDownward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;
        if (transform.position.y <= target.y)
        {
           
            Vector3 p = transform.position;
            p.y = target.y;
            transform.position = p;
            fallingDown = false;
            jumpingUp = false;
            movingEdge = false;
            velocity = new Vector3();


        }
    }
    void JumpUpward(Vector3 target)
    {
        velocity += Physics.gravity * Time.deltaTime;
        if (transform.position.y > target.y)
        {
            jumpingUp = false;
            fallingDown = true;

        }
    }
    void MoveToEdge()
    {

        float distance = Vector3.Distance(transform.position, jumpTarget);
        if (distance <=minimumDistanceToEdge)
        {
            minimumDistanceToEdge = distance;
            SetHorizontalVelocity();
        }
        else
        {
            movingEdge = false;
            fallingDown = true;

            velocity /= 5.0f;
            velocity.y = 1.5f;
        }
     
    }
    public void BeginTurn()
    {
        turn = true; 
    }
    public void EndTurn()
    {
        turn = false;
    }
}

