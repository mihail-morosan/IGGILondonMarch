using UnityEngine;
using System.Collections;


public class GridTile
{
    public string TileType;
    public bool Passable;

    public GameObject VisualTile;
}

public class GridLayer : MonoBehaviour {
    GridTile[,] Grid = new GridTile[12,12];

    public void CreateGridTile(int x, int y, GameObject VTile, string TType, bool Passable)
    {
        Grid[x, y] = new GridTile();
        Grid[x, y].TileType = TType;
        Grid[x, y].Passable = Passable;
        Grid[x, y].VisualTile = VTile;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
