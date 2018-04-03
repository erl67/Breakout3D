using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardMaker : MonoBehaviour {
    public GameObject blockPrefab;
    private int rows = 20, cols = 40;

    public int xStart = 0, yStart = 0;
    
	void Start () {
        Debug.Log("Board: " + transform.position);

        //xStart = (int) transform.position.x;
        //yStart = 0 + (int) transform.position.y;
        xStart = -30;
        yStart = -10;

        for (int x = xStart; x < rows; x+=2)
        {
            for (int y = yStart; y < cols; y+=2)
            {
                var block = Instantiate(blockPrefab) as GameObject;
                block.transform.position = (transform.position + new Vector3(x, y, 0f));
                block.transform.localScale = new Vector3(.7f, .7f, .7f);

                var br = block.GetComponent<Renderer>();        
                br.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
        }
    }
	
}
