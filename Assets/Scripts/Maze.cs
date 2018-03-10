using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    public int sizeX = 20;
    public int sizeZ = 20; 

    public MazeCell cellPrefab;
    private MazeCell[,] cells;
    public float generationStepDelay;

    //public void Generate()
    //{
    //    cells = new MazeCell[sizeX, sizeZ];
    //    for (int x = 0; x < sizeX; x++)
    //    {
    //        for (int z = 0; z < sizeZ; z++)
    //        {
    //            CreateCell(x, z);
    //        }
    //    }
    //}


    public IEnumerator Generate()
    {
        WaitForSeconds delay = new WaitForSeconds(generationStepDelay);
        cells = new MazeCell[sizeX, sizeZ];
        for (int x = 0; x < sizeX; x++)
        {
            for (int z = 0; z < sizeZ; z++)
            {
                yield return delay;
                CreateCell(x, z);
            }
        }
        Debug.Log("Maze Complete");
    }

    private void CreateCell(int x, int z)
    {
        MazeCell newCell = Instantiate(cellPrefab) as MazeCell;
        cells[x, z] = newCell;
        newCell.name = "Maze Cell " + x + ", " + z;
        newCell.transform.parent = transform;
        newCell.transform.localPosition = new Vector3(x - sizeX * 0.5f + 0.5f, 0f, z - sizeZ * 0.5f + 0.5f);
    }

    void Start () {
        Debug.Log("Creating Maze");
    }

}
