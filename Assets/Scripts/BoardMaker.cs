using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BoardMaker : MonoBehaviour {
    public GameObject blockPrefab;

    private int rows, cols, xStart, yStart;
    private int xMod, yMod;
    private float scaleMod, colorMod;
    private Transform board;
    private Color blockColor;
    
	void Start () {
        //Debug.Log("Board: " + transform.position);
        Vector3 offsetY = transform.up * (transform.localScale.y / 2f) * -1f;
        //Debug.Log("offsetY: " + offsetY);
        Vector3 offsetX = transform.right * (transform.localScale.x / 2f) * -1f;
        //Debug.Log("offsetX: " + offsetX);
        xStart = (int)offsetX.x;
        yStart = (int)offsetY.y;
        Debug.Log("x: " + xStart + "  y: " + yStart);

        switch (SceneManager.GetActiveScene().buildIndex)
        {
            case 0:
                rows = Mathf.Abs(yStart * 2);
                cols = Mathf.Abs(xStart * 2);
                blockColor = new Color(0f, 0f, 0f);
                xMod = 3;
                yMod = 2;
                scaleMod = 1f;
                colorMod = 1;
                break;
            case 1:
                rows = Mathf.Abs(yStart * 2);
                cols = Mathf.Abs(xStart * 2);
                blockColor = new Color(.25f, .25f, .25f);
                xMod = 3;
                yMod = 2;
                scaleMod = .9f;
                colorMod = .5f;
                break;
            case 2:
                rows = Mathf.Abs(yStart * 2);
                cols = Mathf.Abs(xStart * 2);
                blockColor = new Color(.5f, .5f, .5f);
                xMod = 3;
                yMod = 2;
                scaleMod = .7f;
                colorMod = .1f;
                break;
            default:
                rows = Mathf.Abs(yStart * 2);
                cols = Mathf.Abs(xStart * 2);
                blockColor = new Color(0f, 0f, 0f);
                xMod = 3;
                yMod = 2;
                scaleMod = Random.Range(.2f, 1.2f);
                colorMod = Random.Range(0f, 1f);
                break;
        }

        for (int y = yStart; y <= rows/2; y++)
        {
            for (int x = xStart; x < cols/2; x++)
            {
                if (y % yMod == 0  && x % xMod == 0)
                {
                    var block = Instantiate(blockPrefab) as GameObject;
                    block.transform.position = (transform.position + new Vector3(x, y, 0f));
                    block.transform.localScale = new Vector3(scaleMod, scaleMod, 1f);
                    block.GetComponent<Rigidbody>().mass = System.Math.Abs(rows + y); //points

                    var mr = block.GetComponent<Renderer>();

                    var g = colorMod - (float)y / (float)rows;
                    var r = colorMod - (float)x / (float)cols;

                    g = Mathf.Abs(g);
                    r = Mathf.Abs(r);

                    blockColor = new Color(r, g, 0f);
                    mr.material.color = blockColor;

                    //mr.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                }

            }
        }
    }
	
}
