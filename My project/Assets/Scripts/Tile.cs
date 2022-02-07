using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool current = false;
    public bool target = false;
    public bool selectable = false;
    public bool walkable = true;
    public List<Tile> adjacent = new List<Tile>();

    public bool visited = false; //proccesed;
    public Tile parent = null;
    public int distance = 0;

    void Update()
    {

        if (current)
        {

            GetComponent<Renderer>().material.color = Color.magenta;
        }
        else if (target)
        {
            GetComponent<Renderer>().material.color = Color.green;
        }
        else if (selectable)
        {
            GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
        }
    }
    public void Reset()
    {
        adjacent.Clear();
        current = false;
        target = false;
        selectable = false;
        walkable = true;


        visited = false; //proccesed;
        parent = null;
        distance = 0;
    }
    public void FindNeighbours(float jumpheight)
    {
        Reset();
        CheckTile(Vector3.forward, jumpheight);
        CheckTile(-Vector3.forward, jumpheight);
        CheckTile(Vector3.right, jumpheight);
        CheckTile(-Vector3.right, jumpheight);
    }
    public void CheckTile(Vector3 direction, float jumpheight)
    {
        Vector3 halfExtends = new Vector3(0.25f, (1 + jumpheight) / 2f, 0.25f);
        Collider[] colliders = Physics.OverlapBox(transform.position + direction, halfExtends);
        foreach (Collider item in colliders)
        {
            Tile tile = item.GetComponent<Tile>();
            if (tile != null && tile.walkable)
            {
                RaycastHit hit;

                if (!Physics.Raycast(tile.transform.position, Vector3.up, out hit, 1))
                {
                    adjacent.Add(tile);
                }
            }
        }
    }
}
