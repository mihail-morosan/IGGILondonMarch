using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;

[Serializable]
public struct NamedPrefab
{
    public string name;
    public GameObject prefab;
}

public class CreateGridFromClingo : MonoBehaviour {
    float ResolutionX, ResolutionY;

    public int Width, Height;

    public float maxW = 0.95f;
    public float minW = 0.35f;

    public float maxH = 0.95f;
    public float minH = 0.05f;

    public float GridDepth = 0.0f;

    public GameObject GridTileGrass;
    public GameObject GridTileRock;
    public GameObject GridTileGem;

    public List<NamedPrefab> Prefabs; 

    float TileSizeH;
    float TileSizeW;

    float startH, endH, startW, endW;

    GridLayer gridLayer;

    public void CreateGridElements(string GameData)
    {

        string[] Definitions = GameData.Split(' ');

        ResolutionX = Screen.width;
        ResolutionY = Screen.height;

        //Draw grid

        if (Camera.main.orthographic)
        {

            Vector3 t = Camera.main.ScreenToWorldPoint(new Vector3(ResolutionX * minW, ResolutionY * minH));
            startH = t.y;
            startW = t.x;

            t = Camera.main.ScreenToWorldPoint(new Vector3(ResolutionX * maxW, ResolutionY * maxH));
            endW = t.x;
            endH = t.y;
        }

        TileSizeW = (endW - startW) / Width;
        TileSizeH = (endH - startH) / Height;

        //Debug.Log("startH: " + startH + " endH: " + endH + " tileSizeH: " + TileSizeH);

        foreach(string Element in Definitions)
        {
            GameObject go = null;
            Regex r1 = new Regex(@"([a-z0-9]+)\(\(([0-9]+),([0-9]+)\)(,([a-z]+))?\)");
            Match match = r1.Match(Element);

            if(match.Groups[1].Value.Equals("start1"))
            {
                int x, y;
                x = int.Parse(match.Groups[2].Value) - 1;
                y = int.Parse(match.Groups[3].Value) - 1;

                Debug.Log("Start at " + x + " " + y);

                //gridLayer.EscapeLocation = new Vector2(x, y);
            }

            if (match.Groups[1].Value.Equals("start2"))
            {
                int x, y;
                x = int.Parse(match.Groups[2].Value) - 1;
                y = int.Parse(match.Groups[3].Value) - 1;

                Debug.Log("Escape at " + x + " " + y);

                gridLayer.EscapeLocation = new Vector2(x, y);
            }

            if(match.Groups[1].Value.Equals("sprite"))
            {
                int x, y;
                x = int.Parse(match.Groups[2].Value) - 1;
                y = int.Parse(match.Groups[3].Value) - 1;

                string TType = match.Groups[5].Value;

                go = CreateVisualTile(x, y, TType);

                if (TType.Equals("gem"))
                {
                    gridLayer.TreasureLocation = new Vector2(x, y);
                }

                gridLayer.CreateGridTile(x, y, go, TType, !TType.Equals("rock"));
            }
        }

        for (int i = 0; i < gridLayer.GetGrid().GetUpperBound(0); i++)
        {
            for (int y = 0; y < gridLayer.GetGrid().GetUpperBound(1); y++)
            {
                if (gridLayer.GetGrid()[i, y] == null)
                {
                    GameObject go = CreateVisualTile(i, y, "blank");
                    gridLayer.CreateGridTile(i, y, go, "blank", false);
                }
            }
        }

        gridLayer.UpdateAllGridTileCosts();

        foreach(var o in FindObjectsOfType<CharacterBasicBehaviour>())
        {
            o.MoveToLocation(o.Location);
        }
    }

    public GameObject CreateVisualTile(int x, int y, string TType)
    {
        Vector3 pos = transform.position;
        pos.x = startW + TileSizeW * (x + 0.5f);
        pos.y = startH + TileSizeH * (y + 0.5f);

        //Debug.Log(x + "x" + y + " - pos.x: " + pos.x + " pos.y: " + pos.y);
        pos.z = 0;

        GameObject go = null;

        Vector3 scale = transform.localScale;

        scale.z = 1;

        GameObject prefab = null;
        foreach(var p in Prefabs)
        {
            if(p.name.Equals(TType))
            {
                prefab = p.prefab;
                break;
            }
        }


        go = (GameObject)Instantiate(prefab, pos, transform.rotation);

        

        //go.transform.position = pos;

        if (go.GetComponent<Renderer>() != null)
        {
            scale.x = TileSizeW / go.GetComponent<Renderer>().bounds.size.x;
            scale.y = TileSizeH / go.GetComponent<Renderer>().bounds.size.y;

            //go.GetComponent<OTSprite>().position = new Vector2(pos.x, pos.y);
        }

        go.transform.localScale = scale;


        go.name = "gridTile" + x + "x" + y;

        go.transform.parent = transform;

        return go;
    }

	// Use this for initialization
	void Start () {
        gridLayer = GetComponent<GridLayer>();

	    //Get text from clingo
        string LevelString = "start1((2,2)) start2((10,10)) sprite((8,8),gem) sprite((2,2),grass) sprite((10,10),grass) sprite((1,1),grass) sprite((1,2),rock) sprite((1,4),rock) sprite((1,5),rock) sprite((1,6),grass) sprite((1,7),grass) sprite((1,8),rock) sprite((1,11),rock) sprite((2,1),grass) sprite((2,3),grass) sprite((2,4),grass) sprite((2,5),grass) sprite((2,6),grass) sprite((2,7),grass) sprite((2,8),rock) sprite((2,9),grass) sprite((2,10),rock) sprite((2,11),rock) sprite((3,1),grass) sprite((3,2),rock) sprite((3,3),grass) sprite((3,4),rock) sprite((3,5),grass) sprite((3,6),grass) sprite((3,7),grass) sprite((3,8),grass) sprite((3,9),grass) sprite((3,10),rock) sprite((3,11),grass) sprite((4,1),grass) sprite((4,2),grass) sprite((4,3),grass) sprite((4,4),grass) sprite((4,5),rock) sprite((4,6),rock) sprite((4,7),rock) sprite((4,8),grass) sprite((4,9),grass) sprite((4,10),grass) sprite((4,11),grass) sprite((5,1),grass) sprite((5,3),grass) sprite((5,4),grass) sprite((5,6),grass) sprite((5,7),grass) sprite((5,8),grass) sprite((5,9),rock) sprite((5,10),grass) sprite((6,1),grass) sprite((6,2),grass) sprite((6,3),grass) sprite((6,4),grass) sprite((6,5),grass) sprite((6,6),grass) sprite((6,7),grass) sprite((6,8),rock) sprite((6,9),grass) sprite((6,10),grass) sprite((6,11),rock) sprite((7,1),grass) sprite((7,2),grass) sprite((7,3),rock) sprite((7,4),grass) sprite((7,5),grass) sprite((7,6),grass) sprite((7,7),grass) sprite((7,8),grass) sprite((7,9),grass) sprite((7,10),grass) sprite((7,11),grass) sprite((8,1),grass) sprite((8,2),grass) sprite((8,3),grass) sprite((8,4),grass) sprite((8,5),grass) sprite((8,6),rock) sprite((8,7),grass) sprite((8,9),grass) sprite((8,10),grass) sprite((8,11),grass) sprite((9,2),grass) sprite((9,3),grass) sprite((9,4),rock) sprite((9,5),grass) sprite((9,6),grass) sprite((9,7),grass) sprite((9,8),grass) sprite((9,9),grass) sprite((9,10),grass) sprite((9,11),grass) sprite((10,2),grass) sprite((10,3),grass) sprite((10,4),grass) sprite((10,5),rock) sprite((10,6),grass) sprite((10,7),grass) sprite((10,8),grass) sprite((10,9),grass) sprite((10,11),rock) sprite((11,1),rock) sprite((11,2),rock) sprite((11,3),rock) sprite((11,4),rock) sprite((11,5),rock) sprite((11,6),rock) sprite((11,7),rock) sprite((11,8),grass) sprite((11,9),grass) sprite((11,10),rock)";

        CreateGridElements(LevelString);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
