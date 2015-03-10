using UnityEngine;
using System.Collections;
using Priority_Queue;
using System.Collections.Generic;
using UnityEngine.UI;

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

    float lastUpdate = 0;

    bool gameIsOver = false;

    public Text DebugPanel;
    public void ClearEverything()
    {
        Debug.LogWarning("Clearing everything in grid");

        Grid = new GridTile[12, 12];
        gameIsOver = false;
        DebugPanel.fontSize = 9;
        lastUpdate = 0;
    }

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

    public void MakeEffect(GridTile tile, string type)
    {
        GetComponent<CreateGridFromClingo>().CreateVisualTile((int)tile.Location.x, (int)tile.Location.y, type);
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
                //if (neighbour != null && neighbour.Passable)
                if (neighbour != null && neighbour.CostToPass < 100000)
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

        //PathToTake.Add(FromTile);


        return PathToTake;
    }

	// Use this for initialization
	void Start () {
	    
	}

    public void UpdateAllGridTileCosts()
    {
        //Vector2 wereChar = (FindObjectOfType<WerewolfBehaviour>() != null) ? 
        //    FindObjectOfType<WerewolfBehaviour>().Location : new Vector2(-1000,-1000) ;
        bool isNotSafe = false;
        for(int i = 0; i < Grid.GetUpperBound(0); i++)
        {
            for(int y=0; y < Grid.GetUpperBound(1); y++)
            {
                if (Grid[i, y] != null)
                {
                    isNotSafe = false;
                    foreach(var w in FindObjectsOfType<WerewolfBehaviour>())
                        if ((Grid[i, y].Location - w.Location).magnitude < 2)
                        {
                            Grid[i, y].CostToPass = 10;
                            isNotSafe = true;
                        }

                    if (Grid[i, y].Passable == false)
                    {
                        Grid[i, y].CostToPass = 1000;
                        isNotSafe = true;
                    }

                    if(Grid[i,y].TileType.Equals("blank"))
                    {
                        Grid[i, y].CostToPass = 1000000;
                    }

                    if (!isNotSafe)
                    {
                        Grid[i, y].CostToPass = 1;
                    }
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
        
        if (lastUpdate == 0)
            lastUpdate = Time.time;



        if (Time.time - lastUpdate > 1)
        {
            UpdateAllGridTileCosts();
            lastUpdate = Time.time;

            if (FindObjectsOfType<MainCharacter>().Length == 0)
            {
                //Utter defeat
                DebugPanel.text = "Spirits have won!";
                DebugPanel.fontSize = 20;

                gameIsOver = true;
            }
            else
            {
                DebugPanel.fontSize = 9;
                gameIsOver = false;
            }
            
        }
        if (gameIsOver)
        {
            return;
        }

        //MainCharacter mainChar = FindObjectOfType<MainCharacter>();

        DebugPanel.text = "";

        foreach (var mainChar in FindObjectsOfType<CharacterBasicBehaviour>())
        {
            DebugPanel.text += mainChar.name + ": " + GoapAgent.prettyPrint(mainChar.currentActions) + "\n\n";
        }

        

        foreach (var mainChar in FindObjectsOfType<MainCharacter>())
        {
            
            if (mainChar.Location.Equals(EscapeLocation) && mainChar.hasTreasure)
            {
                //Victory

                //Debug.Log("Victory " + EscapeLocation + mainChar.Location);
                DebugPanel.text = "Rogues have won!";
                DebugPanel.fontSize = 20;

                gameIsOver = true;

                //Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }

            foreach (var w in FindObjectsOfType<WerewolfBehaviour>())
            {
                if (mainChar.Location.Equals(w.Location))
                {
                    //Defeat

                    MakeEffect(GetTile(mainChar.Location), "death");

                    Destroy(mainChar.gameObject);

                    Debug.LogWarning("Defeat");
                }
            }
        }
	}
}
