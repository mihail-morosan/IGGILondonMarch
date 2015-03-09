﻿using UnityEngine;
using System.Collections;
using Priority_Queue;
using System.Collections.Generic;

public class PQTile : PriorityQueueNode
{
    public GridTile GameTile { get; private set; }
    public PQTile(GridTile GT)
    {
        GameTile = GT;
    }
}

public class GridTile
{
    public string TileType;
    public bool Passable;

    public GameObject VisualTile;

    public Vector2 Location;

    public int CostToPass = 1;
}

public class GridLayer : MonoBehaviour {
    GridTile[,] Grid = new GridTile[12,12];

    public Vector2 EscapeLocation;

    public Vector2 TreasureLocation;

    //public bool TreasureStolen = false;

    public GridTile[,] GetGrid()
    {
        return Grid;
    }

    public GridTile GetTile(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < Grid.GetUpperBound(0) && y < Grid.GetUpperBound(1))
            return Grid[x, y];
        return null;
    }
    public GridTile GetTile(Vector2 loc)
    {
        return GetTile((int)loc.x, (int)loc.y);
    }

    public void CreateGridTile(int x, int y, GameObject VTile, string TType, bool Passable)
    {
        Grid[x, y] = new GridTile();
        Grid[x, y].TileType = TType;
        Grid[x, y].Passable = Passable;
        Grid[x, y].VisualTile = VTile;
        Grid[x, y].Location = new Vector2(x, y);
        //Debug.Log(x + " " + y);
    }

    public void MakeRockGrid(GridTile tile)
    {
        tile.Passable = false;
        GameObject go = GetComponent<CreateGridFromClingo>().CreateVisualTile((int)tile.Location.x, (int)tile.Location.y, "rock");
        //GameObject go = (GameObject)Instantiate(GetComponent<CreateGridFromClingo>().GridTileRock, tile.VisualTile.transform.position, tile.VisualTile.transform.rotation);
        Destroy(tile.VisualTile);
        tile.VisualTile = go;
        tile.TileType = "rock";
    }

    public void MakeGrassGrid(GridTile tile)
    {
        tile.Passable = true;
        GameObject go = GetComponent<CreateGridFromClingo>().CreateVisualTile((int)tile.Location.x, (int)tile.Location.y, "grass");
        //GameObject go = (GameObject)Instantiate(GetComponent<CreateGridFromClingo>().GridTileRock, tile.VisualTile.transform.position, tile.VisualTile.transform.rotation);
        Destroy(tile.VisualTile);
        tile.VisualTile = go;
        tile.TileType = "grass";
    }

    static int[][] Neighbours = new int[][] { new int[] { 0, 1 }, new int[] { 1, 0 }, 
                                                new int[] { 0, -1 }, new int[] { -1, 0 },  };

    public static Vector2 GetNeighbour(Vector2 Coords, int Direction, int Scale = 1)
    {
        int[] _scaledDir = (int[])Neighbours[Direction].Clone();
        _scaledDir[0] *= Scale;
        _scaledDir[1] *= Scale;

        return new Vector3(Coords.x + _scaledDir[0], Coords.y + _scaledDir[1]);
    }

    public List<GridTile> GetBestPathToTile(GridTile FromTile, GridTile OtherTile)
    {
        if (FromTile == null || OtherTile == null)
            return new List<GridTile>();

        if (FromTile == OtherTile)
            return new List<GridTile>();

        //Debug.Log("Path from " + FromTile.Location + " to " + OtherTile.Location);

        HeapPriorityQueue<PQTile> priorityQueue = new HeapPriorityQueue<PQTile>(200);

        //http://www.redblobgames.com/pathfinding/a-star/introduction.html

        priorityQueue.Enqueue(new PQTile(FromTile), 0);

        Dictionary<GridTile, GridTile> came_from = new Dictionary<GridTile, GridTile>();
        Dictionary<GridTile, float> cost_so_far = new Dictionary<GridTile, float>();
        came_from[FromTile] = null;
        cost_so_far[FromTile] = 0;


        PQTile current = null;

        while (!(priorityQueue.Count == 0))
        {
            current = priorityQueue.Dequeue();

            if (current.GameTile == OtherTile)
            {
                break;
            }

            for (int i = 0; i < 4; i++)
            {
                Vector2 neighbourLoc = GetNeighbour(current.GameTile.Location, i);

                GridTile neighbour = GetTile(neighbourLoc);
                if (neighbour != null && neighbour.Passable)
                {
                    float new_cost = cost_so_far[current.GameTile] + neighbour.CostToPass;

                    if ((!cost_so_far.ContainsKey(neighbour)) || (new_cost < cost_so_far[neighbour]))
                    {
                        cost_so_far[neighbour] = new_cost;

                        priorityQueue.Enqueue(new PQTile(neighbour), new_cost + (neighbour.Location - FromTile.Location).magnitude);

                        came_from[neighbour] = current.GameTile;
                    }

                }
            }
        }

        List<GridTile> PathToTake = new List<GridTile>();

        if (cost_so_far.ContainsKey(OtherTile))
        {
            GridTile path = OtherTile;
            do
            {
                PathToTake.Add(path);
                path = came_from[path];
            } while (path != FromTile);
        }


        return PathToTake;
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
