using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    [SerializeField] private int row;
    [SerializeField] private int col;
    [SerializeField] private GameObject ground;
    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void GenerateGrid()
    {
        for (int i = 0; i < row; i++)
        {
            for (int j = 0; j < col; j++)
            {
              var worldCube= Instantiate(ground, new Vector3(j , 0, i ), Quaternion.identity);
                worldCube.name = $"Tile {i} {j}";
            }
        }
    }
}
